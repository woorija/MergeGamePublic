using System;
using System.Collections.Generic;
[Serializable]
public class DailyMissionTemplate
{
    public MissionType missionType;
    public int targetCount;
    public List<MissionRewardData> rewardPool; // 이 중 하나를 랜덤으로 뽑음
}
