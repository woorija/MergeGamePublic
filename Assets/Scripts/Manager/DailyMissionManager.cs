using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DailyMissionManager : MonoBehaviour
{
    private DailyMissionTableSO tableSO;
    private DailyMissionRuntimeData[] activeMissions;
    private DailyMissionSaveEntry[] activeMissionSaveEnries;
    private int[] indexBuffer;

    private DailyMissionSaveData dailyMissionSaveData;

    public int[] missionCounts {  get; private set; }
    int missionCount = 5;
    public bool missionResetFlag { get; private set; }
    public bool missionProgressFlag { get; private set; }

    private void Awake()
    {
        tableSO = MissionDataBase.Instance.dailyMissionTableSO;

        missionCounts = new int[(int)MissionType.Count];
        activeMissions = new DailyMissionRuntimeData[missionCount];
        activeMissionSaveEnries = new DailyMissionSaveEntry[missionCount];

        for (int i = 0; i < missionCount; i++)
        {
            activeMissions[i] = new DailyMissionRuntimeData();
        }

        int templateCount = tableSO.templates.Count;
        indexBuffer = new int[templateCount];
        for (int i = 0; i < templateCount; i++)
        {
            indexBuffer[i] = i;
        }

        dailyMissionSaveData = new DailyMissionSaveData($"{Application.persistentDataPath}/{SavePath.DailyMissionSavePath}", 1024, this);
    }
    public async UniTask InitializeAsync()
    {
        bool loaded = await dailyMissionSaveData.LoadFileAsync();

        if (!loaded)
        {
            GenerateNewMissions();
        }

        missionResetFlag = true;
        SaveManager.Instance.Register(dailyMissionSaveData);
        MarkDirty();
    }
    private void OnDestroy()
    {
        SaveManager.Instance.Unregister(dailyMissionSaveData);
    }
    public void AddCount(MissionType type, int count)
    {
        missionCounts[(int)type] += count;

        for (int i = 0; i < activeMissions.Length; i++)
        {
            if (activeMissions[i].missionType == type)
            {
                missionProgressFlag = true;
                break;
            }
        }
        MarkDirty();
    }
    public void ResetMissions()
    {
        for(int i = 0; i < missionCounts.Length; i++)
        {
            missionCounts[i] = 0;
        }
        GenerateNewMissions();
    }
    void GenerateNewMissions()
    {
        int templateCount = tableSO.templates.Count;

        for (int i = 0; i < activeMissions.Length; i++)
        {
            int rand = UnityEngine.Random.Range(i, templateCount);
            (indexBuffer[i], indexBuffer[rand]) = (indexBuffer[rand], indexBuffer[i]);

            var t = tableSO.templates[indexBuffer[i]];
            int rewardIndex = UnityEngine.Random.Range(0, t.rewardPool.Count);
            var reward = t.rewardPool[rewardIndex];

            activeMissions[i].Init(t.missionType, t.targetCount, reward, false);

            activeMissionSaveEnries[i].templateIndex = indexBuffer[i];
            activeMissionSaveEnries[i].rewardIndex = rewardIndex;
        }
        missionResetFlag = true;
        MarkDirty();
    }
    public void GetMissionReward(int index)
    {
        activeMissions[index].isComplete = true;
        var rewardData = activeMissions[index].reward;

        switch (rewardData.rewardType)
        {
            case MissionRewardType.Xp:
                PlayerDataManager.Instance.AddXp(rewardData.amount);
                return;
            case MissionRewardType.Gold:
                PlayerDataManager.Instance.AddGold(rewardData.amount);
                return;
            case MissionRewardType.Gem:
                PlayerDataManager.Instance.AddGem(rewardData.amount);
                return;
            case MissionRewardType.Stamina:
                PlayerDataManager.Instance.AddStamina(rewardData.amount);
                return;
        }
        MarkDirty();
    }
    public void ClearResetFlag()
    {
        missionResetFlag = false;
    }
    public void ClearProgressFlag()
    {
        missionProgressFlag = false;
    }
    public DailyMissionRuntimeData GetMissionData(int index)
    {
        return activeMissions[index];
    }
    public int GetCount(MissionType missionType)
    {
        return missionCounts[(int)missionType];
    }
    #region SaveLoad
    public int[] GetMissionCountsForSerialization()
    {
        return missionCounts;
    }
    public DailyMissionRuntimeData[] GetActiveMissionsForSerialization()
    {
        return activeMissions;
    }
    public DailyMissionSaveEntry[] GetMissionSaveEntriesForSerialization()
    {
        return activeMissionSaveEnries;
    }
    public DailyMissionTableSO GetTableSOForSerialization()
    {
        return tableSO;
    }
    private void MarkDirty()
    {
        dailyMissionSaveData.MarkDirty();
    }
    #endregion
}
