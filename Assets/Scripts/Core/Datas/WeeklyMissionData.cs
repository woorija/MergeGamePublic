using System;

[Serializable]
public class WeeklyMissionData
{
    public MissionType missionType;
    public int targetCount;
    public int score;
    public MissionRewardData reward;
}
