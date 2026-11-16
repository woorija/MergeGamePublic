using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeeklyMissionManager : MonoBehaviour
{
    private const int missionCount = 35;
    private const int scoreRewardCount = 5;

    [SerializeField] InBoxManager inBoxManager;
    private WeeklyMissionTableSO tableSO;

    private WeeklyMissionSaveData weeklyMissionSaveData;

    public WeeklyMissionTier weeklyMissionTier { get; private set; }
    public int[] missionCounts { get; private set; }
    public bool[] isCompletes { get; private set; }
    public int totalScore { get; private set; }
    public bool[] claimedScoreRewards {  get; private set; }
    public bool missionResetFlag { get; private set; }
    public bool missionProgressFlag { get; private set; }

    public Action OnScoreChanged;
    public Action<int> OnScoreRewardClaimed;
    private void Awake()
    {
        missionCounts = new int[(int)MissionType.Count];
        isCompletes = new bool[missionCount];
        claimedScoreRewards = new bool[scoreRewardCount];

        weeklyMissionSaveData = new WeeklyMissionSaveData($"{Application.persistentDataPath}/{SavePath.WeeklyMissionSavePath}", 1024, this);
    }
    public async UniTask InitializeAsync()
    {
        bool loaded = await weeklyMissionSaveData.LoadFileAsync();

        if (loaded)
        {
            tableSO = MissionDataBase.Instance.WeeklyMissionTables[weeklyMissionTier];
        }
        else
        {
            tableSO = MissionDataBase.Instance.WeeklyMissionTables[WeeklyMissionTier.Tier1];
        }

        missionResetFlag = true;
        SaveManager.Instance.Register(weeklyMissionSaveData);
        MarkDirty();
    }
    private void OnDestroy()
    {
        SaveManager.Instance.Unregister(weeklyMissionSaveData);
    }
    public void AddCount(MissionType type, int count)
    {
        missionCounts[(int)type] += count;

        for (int i = 0; i < tableSO.table.Count; i++)
        {
            if (tableSO.table[i].missionType == type)
            {
                missionProgressFlag = true;
                break;
            }
        }
        MarkDirty();
    }
    public void ResetMissions()
    {
        for (int i = 0; i < missionCounts.Length; i++)
        {
            missionCounts[i] = 0;
        }
        for (int i = 0; i < isCompletes.Length; i++)
        {
            isCompletes[i] = false;
        }
        totalScore = 0;
        for (int i = 0; i < claimedScoreRewards.Length; i++)
        {
            claimedScoreRewards[i] = false;
        }
        missionResetFlag = true;
        MarkDirty();
    }
    public void GetMissionReward(int index)
    {
        isCompletes[index] = true;
        var rewardData = tableSO.table[index].reward;
        AddScore(tableSO.table[index].score);
        GetReward(rewardData);
        MarkDirty();
    }
    public void GetScoreReward(int index)
    {
        claimedScoreRewards[index] = true;
        var rewardData = tableSO.scoreRewardTable[index].reward;
        OnScoreRewardClaimed?.Invoke(index);
        GetReward(rewardData);
        MarkDirty();
    }
    void AddScore(int score)
    {
        totalScore += score;
        OnScoreChanged?.Invoke();
    }
    void GetReward(MissionRewardData rewardData)
    {
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
            case MissionRewardType.BoardItem:
                inBoxManager.AddGift(rewardData.item.category, rewardData.item.level);
                return;
        }
    }
    public void ClearResetFlag()
    {
        missionResetFlag = false;
    }
    public void ClearProgressFlag()
    {
        missionProgressFlag = false;
    }
    public WeeklyMissionData GetMissionData(int index)
    {
        return tableSO.table[index];
    }
    public int GetCount(MissionType missionType)
    {
        return missionCounts[(int)missionType];
    }
    public IReadOnlyList<WeeklyMissionScoreRewardData> GetScoreRewardTable()
    {
        return tableSO.scoreRewardTable;
    }
    #region SaveLoad
    public void SetWeeklyMissionTier(WeeklyMissionTier tier)
    {
        weeklyMissionTier = tier;
    }
    public int[] GetMissionCountsForSerialization()
    {
        return missionCounts;
    }
    public bool[] GetIsCompletesForSerialization()
    {
        return isCompletes;
    }
    public void SetTotalScore(int totalScore)
    {
        this.totalScore = totalScore;
    }
    public bool[] GetClaimedScoreRewardsForSerialization()
    {
        return claimedScoreRewards;
    }
    private void MarkDirty()
    {
        weeklyMissionSaveData.MarkDirty();
    }
    #endregion
}
