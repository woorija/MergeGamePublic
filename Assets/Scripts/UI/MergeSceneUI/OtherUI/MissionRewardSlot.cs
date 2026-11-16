using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MissionRewardSlot : MonoBehaviour
{
    [SerializeField] MissionRewardImage rewardImage;
    [SerializeField] TMP_Text rewardAmountText;

    public void SetRewardSlot(MissionRewardData rewardData)
    {
        switch (rewardData.rewardType)
        {
            case MissionRewardType.BoardItem:
                rewardImage.SetItemImage(rewardData.item.category, rewardData.item.level).Forget();
                rewardAmountText.text = string.Empty;
                return;
            case MissionRewardType.Xp:
                rewardImage.SetXpImage().Forget();
                break;
            case MissionRewardType.Gold:
                rewardImage.SetGoldImage().Forget();
                break;
            case MissionRewardType.Gem:
                rewardImage.SetGemImage().Forget();
                break;
            case MissionRewardType.Stamina:
                rewardImage.SetStaminaImage().Forget();
                break;
        }
        rewardAmountText.text = rewardData.amount.ToString();
    }
}
