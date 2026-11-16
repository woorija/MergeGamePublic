using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MissionManager : SingletonBehaviour<MissionManager>
{
    [SerializeField] DailyMissionManager dailyMissionManager;
    [SerializeField] WeeklyMissionManager weeklyMissionManager;

    public Action OnDailyReset;
    public Action OnWeeklyReset;

    public async UniTask InitializeAsync()
    {
        await dailyMissionManager.InitializeAsync();
        await weeklyMissionManager.InitializeAsync();
    }
    public void ResetDailyMission()
    {
        dailyMissionManager.ResetMissions();
        OnDailyReset?.Invoke();
    }
    public void ResetWeeklyMission()
    {
        weeklyMissionManager.ResetMissions();
        OnWeeklyReset?.Invoke();
    }
    public void AddMergeCount(int count)
    {
        dailyMissionManager.AddCount(MissionType.Merge, count);
        weeklyMissionManager.AddCount(MissionType.Merge, count);
    }

    public void AddOrderCompleteCount(int count)
    {
        dailyMissionManager.AddCount(MissionType.OrderComplete, count);
        weeklyMissionManager.AddCount(MissionType.OrderComplete, count);
    }

    public void AddShopPurchaseCount(int count)
    {
        dailyMissionManager.AddCount(MissionType.ShopPurchase, count);
        weeklyMissionManager.AddCount(MissionType.ShopPurchase, count);
    }

    public void AddGemSpendCount(int count)
    {
        dailyMissionManager.AddCount(MissionType.GemSpend, count);
        weeklyMissionManager.AddCount(MissionType.GemSpend, count);
    }

    public void AddGemEarnCount(int count)
    {
        dailyMissionManager.AddCount(MissionType.GemEarn, count);
        weeklyMissionManager.AddCount(MissionType.GemEarn, count);
    }

    public void AddGoldSpendCount(int count)
    {
        dailyMissionManager.AddCount(MissionType.GoldSpend, count);
        weeklyMissionManager.AddCount(MissionType.GoldSpend, count);
    }

    public void AddGoldEarnCount(int count)
    {
        dailyMissionManager.AddCount(MissionType.GoldEarn, count);
        weeklyMissionManager.AddCount(MissionType.GoldEarn, count);
    }

    public void AddXpEarnCount(int count)
    {
        dailyMissionManager.AddCount(MissionType.XpEarn, count);
        weeklyMissionManager.AddCount(MissionType.XpEarn, count);
    }
}
