using TMPro;
using UnityEngine;

public class StaminaPurchasePopupUI : BasePopupUI
{
    [SerializeField] StaminaManager staminaManager;

    [SerializeField] TMP_Text priceText;
    int price;
    public override void Open()
    {
        base.Open();
        SetPrice();
    }
    void SetPrice()
    {
        price = staminaManager.GetPurchaseCost();
        priceText.text = $"스태미너를 구매하는데\n{price} Gem이 필요합니다.";
    }
    public void Purchase()
    {
        if (staminaManager.TryPurchaseStamina())
        {
            Close();
            return;
        }
        Close();
        PopupManager.Instance.popup.SetPopupMessage("젬이 부족해 구매할 수 없습니다.");
    }
    public void OnReset()
    {
        if (gameObject.activeInHierarchy)
        {
            SetPrice();
        }
    }
}
