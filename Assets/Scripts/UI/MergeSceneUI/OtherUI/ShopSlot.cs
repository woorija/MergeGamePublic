using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] BaseItemImage baseItemImage;
    [SerializeField] ShopItemPurchaseButton shopItemPurchaseButton;
    [SerializeField] GameObject soldoutOverlay;

    public void SetSlot(ShopItemData data)
    {
        baseItemImage.SetBaseImage(data.category, data.level).Forget();
        shopItemPurchaseButton.SetUI(data);
        soldoutOverlay.SetActive(false);
    }
    public void SetSoldOutOverlay(bool active)
    {
        soldoutOverlay.SetActive(active);
    }
}
