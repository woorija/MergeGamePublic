using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SaveManager : SingletonBehaviour<SaveManager>
{
    private readonly List<ISaveable> savedatas = new List<ISaveable>();
    private TimeSaveData timeSaveData;

    private const long saveDelaySeconds = 5;
    private const long forceSaveDelaySeconds = 10;

    private long nextSaveTime;
    private long forceSaveTime;
    protected override void Awake()
    {
        base.Awake();
#if UNITY_ANDROID
        Application.lowMemory += OnLowMemory;
#endif
    }
    private void OnDestroy()
    {
#if UNITY_ANDROID
        Application.lowMemory -= OnLowMemory;
#endif
    }
    public void Register(ISaveable savedata)
    {
        if (!savedatas.Contains(savedata))
        {
            savedatas.Add(savedata);
        }
    }
    public void RegisterTimeSaveData(TimeSaveData timeSaveData)
    {
        this.timeSaveData = timeSaveData;
    }
    public void Unregister(ISaveable savedata)
    {
        savedatas.Remove(savedata);
    }
    public void UnRegisterTimeSaveData()
    {
        timeSaveData = null;
    }

    public void NotifyDirty()
    {
        long currentTime = TimeManager.Instance.currentTime;

        nextSaveTime = currentTime + saveDelaySeconds;
        if (forceSaveTime == 0)
        {
            forceSaveTime = currentTime + forceSaveDelaySeconds;
        }
    }
    public void OnSecondTick(long now)
    {
        bool isSaveTime =  (nextSaveTime > 0 && now >= nextSaveTime) || (forceSaveTime > 0 && now >= forceSaveTime);

        if (isSaveTime)
        {
            nextSaveTime = 0;
            forceSaveTime = 0;
            SaveAllDirtyAsync(ct: destroyCancellationToken).Forget();
        }
    }
    public async UniTask SaveAllDirtyAsync(CancellationToken ct = default)
    {
        for(var i = 0; i < savedatas.Count; i++)
        {
            if (!savedatas[i].IsDirty)
            {
                continue;
            }
            await savedatas[i].SaveFileAsync(ct);
            savedatas[i].ClearDirty();
        }
        await timeSaveData.SaveFileAsync(ct);
    }
    #region 생명주기
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            FlushAllDrity();
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            FlushAllDrity();
        }
    }
    private void OnApplicationQuit()
    {
        FlushAllDrity();
    }
    private void OnLowMemory()
    {
        FlushAllDrity();
    }
    private void FlushAllDrity()
    {
        for (var i = 0; i < savedatas.Count; i++)
        {
            if (!savedatas[i].IsDirty)
            {
                continue;
            }
            savedatas[i].SaveFileSyncOnQuit();
            savedatas[i].ClearDirty();
        }
        timeSaveData.SaveFileSyncOnQuit();
    }
    #endregion
}
