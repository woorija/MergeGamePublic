using UnityEngine;

public class PopupManager : SingletonBehaviour<PopupManager>
{
    [SerializeField] GameObject dimArea;
    [SerializeField] BasePopupUI[] popupUIs;

    [SerializeField] PopupUI popupUI;
    [SerializeField] BubblePurchasePopupUI bubblePurchasePopupUI;
    [SerializeField] ItemSellPopupUI itemSellPopupUI;
    [SerializeField] StoragePopupUI storagePopupUI;
    [SerializeField] StorageExpandPopupUI storageExpandPopupUI;
    [SerializeField] LevelUpPopupUI levelUpPopupUI;
    [SerializeField] StaminaPurchasePopupUI staminaPurchasePopupUI;
    [SerializeField] DailyMissionPopupUI dailyMissionPopupUI;
    [SerializeField] WeeklyMissionPopupUI weeklyMissionPopupUI;
    [SerializeField] ShopPopupUI shopPopupUI;

    public PopupUI popup => popupUI;
    public BubblePurchasePopupUI bubblePurchase => bubblePurchasePopupUI;
    public StaminaPurchasePopupUI staminaPurchase => staminaPurchasePopupUI;
    public StoragePopupUI storage => storagePopupUI;
    public StorageExpandPopupUI storageExpand => storageExpandPopupUI;
    public LevelUpPopupUI levelUp => levelUpPopupUI;
    public DailyMissionPopupUI dailyMission => dailyMissionPopupUI;
    public WeeklyMissionPopupUI weeklyMission => weeklyMissionPopupUI;
    public ShopPopupUI shop => shopPopupUI;
    public ItemSellPopupUI itemSell => itemSellPopupUI;

    int openedPopupCount;
    public void Init()
    {
        storage.Init();
        shop.Init();
        weeklyMission.Init();
        CloseAllPopup();
    }
    private void CloseAllPopup()
    {
        for (int i = 0; i < popupUIs.Length; i++)
        {
            popupUIs[i].CloseImmediate();
        }
        openedPopupCount = 0;
        dimArea.SetActive(false);
    }
    public void OnPopupOpened()
    {
        if(openedPopupCount == 0)
        {
            dimArea.SetActive(true);
        }
        openedPopupCount++;
    }
    public void OnPopupClosed()
    {
        openedPopupCount--;
        if (openedPopupCount == 0)
        {
            dimArea.SetActive(false);
        }
    }
}
