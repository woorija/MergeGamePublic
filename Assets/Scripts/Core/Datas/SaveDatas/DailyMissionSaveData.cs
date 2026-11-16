public class DailyMissionSaveData : BaseSaveData
{
    private DailyMissionManager manager;
    public DailyMissionSaveData(string path, int bufferSize, DailyMissionManager manager) : base(path, bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }
    protected override void Serialize()
    {
        Save(dataVersion);

        Save(manager.missionCounts);

        DailyMissionRuntimeData[] missionDatas = manager.GetActiveMissionsForSerialization();
        DailyMissionSaveEntry[] saveEntries = manager.GetMissionSaveEntriesForSerialization();

        Save(missionDatas.Length);

        for (int i = 0; i < missionDatas.Length; i++)
        {
            Save(saveEntries[i].templateIndex);
            Save(saveEntries[i].rewardIndex);
            Save(missionDatas[i].isComplete);
        }
    }

    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            int[] missionCounts = manager.GetMissionCountsForSerialization();
            LoadInto(missionCounts);

            DailyMissionRuntimeData[] activeMission = manager.GetActiveMissionsForSerialization();
            DailyMissionSaveEntry[] saveEntries = manager.GetMissionSaveEntriesForSerialization();

            int count = LoadInt();

            for (int i = 0; i < count; i++)
            {
                int templateIndex = LoadInt();
                int rewardIndex = LoadInt();
                bool isComplete = LoadBool();

                saveEntries[i].templateIndex = templateIndex;
                saveEntries[i].rewardIndex = rewardIndex;

                var t = manager.GetTableSOForSerialization().templates[templateIndex];
                var reward = t.rewardPool[rewardIndex];

                activeMission[i].Init(t.missionType, t.targetCount, reward, isComplete);
            }
        }
    }
}
