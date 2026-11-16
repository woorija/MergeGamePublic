public class WeeklyMissionSaveData : BaseSaveData
{
    WeeklyMissionManager manager;
    public WeeklyMissionSaveData(string path, int bufferSize, WeeklyMissionManager manager) : base(path, bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }
    protected override void Serialize()
    {
        Save(dataVersion);
        Save((int)manager.weeklyMissionTier);
        Save(manager.missionCounts);
        Save(manager.isCompletes);
        Save(manager.totalScore);
        Save(manager.claimedScoreRewards);
    }

    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            WeeklyMissionTier tier = (WeeklyMissionTier)LoadInt();
            manager.SetWeeklyMissionTier(tier);

            int[] missionCounts = manager.GetMissionCountsForSerialization();
            LoadInto(missionCounts);

            bool[] isCompletes = manager.GetIsCompletesForSerialization();
            LoadInto(isCompletes);

            int totalScore = LoadInt();
            manager.SetTotalScore(totalScore);

            bool[] claimedScoreRewards = manager.GetClaimedScoreRewardsForSerialization();
            LoadInto(claimedScoreRewards);
        }
    }
}
