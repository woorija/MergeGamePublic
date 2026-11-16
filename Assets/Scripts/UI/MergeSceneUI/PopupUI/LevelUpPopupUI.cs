using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelUpPopupUI : BasePopupUI
{
    [SerializeField] GiftItemImage[] giftImages;

    public void SetUI(int level)
    {
        base.Open();

        var gifts = LevelUpRewardDataBase.Instance.GetRewards(level);

        for(int i = 0; i < gifts.Count; i++)
        {
            giftImages[i].gameObject.SetActive(true);
            giftImages[i].SetImage(gifts[i]);
        }
        for(int i = gifts.Count; i < giftImages.Length; i++)
        {
            giftImages[i].gameObject.SetActive(false);
        }
    }
}
