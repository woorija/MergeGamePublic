using UnityEngine;

public class ShopPopupUI : BasePopupUI
{
    [SerializeField] ShopSlot[] shopSlots;
    [SerializeField] ShopRefreshButton shopRefreshButton;
    [SerializeField] GetFreeGemButton freeGemButton;
    [SerializeField] ShopManager shopManager;
    public void Init()
    {
        shopManager.OnShopStateChanged += RefreshShopState;
        RefreshShopState();
    }

    private void OnDestroy()
    {
        shopManager.OnShopStateChanged -= RefreshShopState;
    }
    public void OnClickRefresh()
    {
        if (shopManager.RefreshShop())
        {
            RefreshAllSlot();
            SetRefreshButtonText();
        }
    }
    public void OnClickFreeGem()
    {
        if (shopManager.GetDailyGem())
        {
            SetFreeGemButton();
        }
    }
    public void OnClickBuy(int index)
    {
        switch (shopManager.CanBuy(index))
        {
            case ShopPurchaseResult.Succese:
                shopManager.BuyItem(index);
                SetOverlay(index);
                return;
            case ShopPurchaseResult.NotEnoughGold:
                PopupManager.Instance.popup.SetPopupMessage("골드가 부족해 아이템을 구매할 수 없습니다.");
                return;
            case ShopPurchaseResult.NotEnoughGem:
                PopupManager.Instance.popup.SetPopupMessage("젬이 부족해 아이템을 구매할 수 없습니다.");
                return;
            case ShopPurchaseResult.Fail:
                PopupManager.Instance.popup.SetPopupMessage("알 수 없는 이유로 아이템을 구매할 수 없습니다.");
                return;
        }
    }
    void SetFreeGemButton()
    {
        freeGemButton.SetSoldOutOverlay(shopManager.hasClaimedDailyGem);
    }
    void SetRefreshButtonText()
    {
        shopRefreshButton.SetRefreshPriceText(shopManager.GetRefreshPrice());
    }
    void RefreshSlot(int index)
    {
        shopSlots[index].SetSlot(shopManager.GetShopItem(index));
    }
    public void RefreshAllSlot()
    {
        for(int i = 0; i < shopSlots.Length; i++)
        {
            RefreshSlot(i);
        }
    }
    void SetOverlay(int index)
    {
        shopSlots[index].SetSoldOutOverlay(shopManager.IsPurchased(index));
    }
    public void OnReset()
    {
        if (gameObject.activeInHierarchy)
        {
            RefreshShopState();
        }
    }

    private void RefreshShopState()
    {
        RefreshAllSlot();
        SetFreeGemButton();
        SetRefreshButtonText();
        for (int i = 0; i < shopSlots.Length; i++)
        {
            SetOverlay(i);
        }
    }
}
