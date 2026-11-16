using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TimeManager : SingletonBehaviour<TimeManager>
{
    public Action<long> OnSecondTick;
    public Action OnDailyReset;
    public Action OnWeeklyReset;
    public long currentTime {  get; private set; }
    public int dayOfWeek {  get; private set; } // 월:0 ~ 일:6

    long nextDailyResetTimestamp;
    long nextWeeklyResetTimestamp;

    private TimeSaveData timeSaveData;
    protected override void Awake()
    {
        base.Awake();

        timeSaveData = new TimeSaveData($"{Application.persistentDataPath}/{SavePath.TimeSavePath}", 128, this);
    }
    public async UniTask InitializeAsync()
    {
        bool loaded = await timeSaveData.LoadFileAsync();

        if (!loaded)
        {
            currentTime = GetCurrentTimestamp();
            dayOfWeek = GetDayOfWeekMondayBased(currentTime);
        }

        nextDailyResetTimestamp = GetNextDailyResetTimestamp(currentTime);
        nextWeeklyResetTimestamp = GetNextWeeklyResetTimestamp(currentTime);

        SaveManager.Instance.RegisterTimeSaveData(timeSaveData);
    }
    private void OnDestroy()
    {
        SaveManager.Instance.UnRegisterTimeSaveData();
    }
    private void Update()
    {
        long now = GetCurrentTimestamp();

        if(now == currentTime)
        {
            return;
        }

        currentTime = now;
        CheckReset();
        OnSecondTick?.Invoke(currentTime);
        //DevelopUtility.Log($"틱{currentTime}");
    }
    private void CheckReset()
    {
        if (currentTime >= nextDailyResetTimestamp)
        {
            OnDailyReset?.Invoke();
            nextDailyResetTimestamp = GetNextDailyResetTimestamp(currentTime);
            dayOfWeek = GetDayOfWeekMondayBased(currentTime);
        }

        if (currentTime >= nextWeeklyResetTimestamp)
        {
            OnWeeklyReset?.Invoke();
            nextWeeklyResetTimestamp = GetNextWeeklyResetTimestamp(currentTime);
        }
    }

    private long GetNextDailyResetTimestamp(long timestamp)
    {
        const long SecondsPerDay = 86400;

        return (timestamp / SecondsPerDay + 1) * SecondsPerDay;
    }

    private int GetDayOfWeekMondayBased(long timestamp)
    {
        const long SecondsPerDay = 86400;

        long days = timestamp / SecondsPerDay;

        int dayOfWeek = (int)((days + 3) % 7);
        return dayOfWeek;
    }

    private long GetNextWeeklyResetTimestamp(long timestamp)
    {
        const long SecondsPerDay = 86400;

        long days = timestamp / SecondsPerDay;
        long daysSinceMonday = GetDayOfWeekMondayBased(timestamp);

        long mondayStart = (days - daysSinceMonday) * SecondsPerDay;

        return mondayStart + 7 * SecondsPerDay;
    }

    private long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
    #region SaveLoad
    public void LoadData(long time)
    {
        currentTime = time;
        dayOfWeek = GetDayOfWeekMondayBased(currentTime);
    }
    #endregion
}
