using Cysharp.Threading.Tasks;
using UnityEngine;

public class GiftItemImage : MonoBehaviour
{
    [SerializeField] BaseItemImage itemImage;

    public void SetImage(ItemRewardData rewardData)
    {
        gameObject.SetActive(true);
        itemImage.SetBaseImage(rewardData.category, rewardData.level).Forget();
    }

}
