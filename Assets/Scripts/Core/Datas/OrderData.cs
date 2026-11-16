using System;

[Serializable]
public class OrderData
{
    public int npcId;
    public int requirementCount;

    public readonly OrderRequirement[] requirements = new OrderRequirement[3];
    public readonly bool[] requirementSatisfied = new bool[3];
    public RewardWeight totalRewardWeight;

    public bool canComplete;

    public void Clear()
    {
        npcId = 0;
        requirementCount = 0;
        requirements[0] = default;
        requirements[1] = default;
        requirements[2] = default;
        requirementSatisfied[0] = false;
        requirementSatisfied[1] = false;
        requirementSatisfied[2] = false;
        totalRewardWeight = default;
        canComplete = false;
    }
}