using System;

[Serializable]
public class MissionRewardData
{
    public MissionRewardType rewardType;
    public int amount; // xp,gold,gem,stamina에서 사용
    public ItemRewardData item; // 아이템에서 사용
}
