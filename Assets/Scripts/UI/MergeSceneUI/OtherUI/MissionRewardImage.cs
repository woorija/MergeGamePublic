using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MissionRewardImage : MonoBehaviour
{
    Image rewardImage;
    private void Awake()
    {
        rewardImage = GetComponent<Image>();
    }
    public async UniTask SetItemImage(ItemCategory category, int level)
    {
        rewardImage.sprite = await ItemImageLoader.Instance.Load(category, level);
    }
    public async UniTask SetClearImage()
    {
        rewardImage.sprite = await ItemImageLoader.Instance.Load(ItemCategory.None, 0);
    }
    public async UniTask SetGoldImage()
    {
        rewardImage.sprite = await ItemImageLoader.Instance.Load(ItemCategory.Gold, 1);
    }
    public async UniTask SetGemImage()
    {
        rewardImage.sprite = await ItemImageLoader.Instance.Load(ItemCategory.Gem, 1);
    }
    public async UniTask SetStaminaImage()
    {
        rewardImage.sprite = await ItemImageLoader.Instance.Load(ItemCategory.Stamina, 4);
    }
    public async UniTask SetXpImage()
    {
        rewardImage.sprite = await ItemImageLoader.Instance.LoadXp();
    }
}
