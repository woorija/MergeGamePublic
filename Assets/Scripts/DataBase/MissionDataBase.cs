using System.Collections.Generic;

public class MissionDataBase : SingletonBehaviour<MissionDataBase>
{
    public DailyMissionTableSO dailyMissionTableSO {  get; private set; }
    public Dictionary<WeeklyMissionTier, WeeklyMissionTableSO> WeeklyMissionTables {  get; private set; }
    public void Init()
    {
        WeeklyMissionTables = new Dictionary<WeeklyMissionTier, WeeklyMissionTableSO>();
    }
    public void SetDailyMissionTableSO(DailyMissionTableSO dailyMissionTableSO)
    {
        this.dailyMissionTableSO = dailyMissionTableSO;
    }
    public void AddWeeklyMissionTableSO(WeeklyMissionTier tier, WeeklyMissionTableSO weeklyMissionTableSO)
    {
        WeeklyMissionTables.Add(tier, weeklyMissionTableSO);
    }
}
