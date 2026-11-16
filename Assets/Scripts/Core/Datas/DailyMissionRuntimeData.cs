public class DailyMissionRuntimeData
{
    public MissionType missionType;
    public int count;
    public MissionRewardData reward;
    public bool isComplete;
    public void Init(MissionType missionType, int count, MissionRewardData reward, bool isComplete)
    {
        this.missionType = missionType;
        this.count = count;
        this.reward = reward;
        this.isComplete = isComplete;
    }
}
