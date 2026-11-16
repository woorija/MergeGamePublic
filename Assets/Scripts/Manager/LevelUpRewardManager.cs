using UnityEngine;

public class LevelUpRewardManager : MonoBehaviour
{
    [SerializeField] InBoxManager inBoxManager;
    public void GetLevelUpReward(int level)
    {
        var gifts = LevelUpRewardDataBase.Instance.GetRewards(level);

        for (int i = 0; i < gifts.Count; i++)
        {
            inBoxManager.AddGift(gifts[i].category, gifts[i].level);
        }
    }
}
