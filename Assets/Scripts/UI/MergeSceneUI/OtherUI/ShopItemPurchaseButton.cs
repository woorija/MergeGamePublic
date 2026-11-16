using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemPurchaseButton : MonoBehaviour
{
    [SerializeField] Image priceTypeImage;
    [SerializeField] TMP_Text priceText;

    public void SetUI(ShopItemData data)
    {
        SetPriceTypeImage(data.priceType).Forget();
        priceText.text = data.price.ToString();
    }
    async UniTask SetPriceTypeImage(ShopPriceType shopPriceType)
    {
        switch (shopPriceType) {
            case ShopPriceType.Gold:
                priceTypeImage.sprite = await ItemImageLoader.Instance.Load(ItemCategory.Gold, 1);
                return;
            case ShopPriceType.Gem:
                priceTypeImage.sprite = await ItemImageLoader.Instance.Load(ItemCategory.Gem, 1);
                return;
        }
    }
}
