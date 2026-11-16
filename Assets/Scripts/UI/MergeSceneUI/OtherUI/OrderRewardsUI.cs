using UnityEngine;


public class OrderRewardsUI : MonoBehaviour
{
    [SerializeField] OrderRewardTooltip xpRewardTooltip;
    [SerializeField] OrderRewardTooltip goldRewardTooltip;

    public void SetTooltip(RewardWeight rewardWeight)
    {
        SetXpTooltip(rewardWeight.xp);
        SetGoldTooltip(rewardWeight.gold);
    }
    void SetXpTooltip(int weight)
    {
        if(weight > 0)
        {
            xpRewardTooltip.SetText(weight);
        }
        else
        {
            xpRewardTooltip.SetActiveFalse();
        }
    }
    void SetGoldTooltip(int weight)
    {
        if (weight > 0)
        {
            goldRewardTooltip.SetText(weight);
        }
        else
        {
            goldRewardTooltip.SetActiveFalse();
        }
    }
}
