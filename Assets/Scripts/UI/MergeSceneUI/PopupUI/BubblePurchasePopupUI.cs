using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BubblePurchasePopupUI : BasePopupUI
{
    [SerializeField] TMP_Text priceText;
    [SerializeField] BaseItemImage itemImage;
    ItemData currentBubbleItem;
    int price;
    public void Open(ItemData itemData)
    {
        currentBubbleItem = itemData;
        Open();
        itemImage.SetBaseImage(currentBubbleItem).Forget();
        SetPrice();
    }
    public override void Close()
    {
        currentBubbleItem = null;
        base.Close();
    }
    void SetPrice()
    {
        price = BubblePriceDataBase.BubblePriceTable[(currentBubbleItem.category, currentBubbleItem.level)];
        priceText.text = $"해당 아이템을 구매하는데 {price}젬이 필요합니다.";
    }
    public void Purchase()
    {
        BubblePurchaseResult result = BoardManager.Instance.PurchaseBubbleItem(currentBubbleItem);
        
        Close();
        switch (result)
        {
            case BubblePurchaseResult.Success:
                return;
            case BubblePurchaseResult.NotBubble:
                PopupManager.Instance.popup.SetPopupMessage("이미 사라진 아이템입니다.");
                return;
            case BubblePurchaseResult.NotEnoughGem:
                PopupManager.Instance.popup.SetPopupMessage("젬이 부족해 구매할 수 없습니다.");
                return;
        }
    }
}
