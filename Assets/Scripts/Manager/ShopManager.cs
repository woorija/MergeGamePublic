using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class ShopManager : SingletonBehaviour<ShopManager>
{
    private const int ShopSlotCount = 6;
    private const int baseRefreshPrice = 10;
    private const int MaxRefreshCount = 10;

    private ShopDataBase shopDataBase;

    private int[] currentItemIndices;
    private bool[] purchasedItems;

    private int refreshCount;
    public bool hasClaimedDailyGem {  get; private set; }
    private ShopSaveData shopSaveData;

    [SerializeField] InBoxManager inBoxManager;

    public Action<int> OnShopPurchase;
    public Action OnDailyReset;
    public Action OnShopStateChanged;

    public int RefreshCount => refreshCount;

    protected override void Awake()
    {
        base.Awake();

        currentItemIndices = new int[ShopSlotCount];
        purchasedItems = new bool[ShopSlotCount];

        shopDataBase = ShopDataBase.Instance;

        shopSaveData = new ShopSaveData($"{Application.persistentDataPath}/{SavePath.ShopSavePath}", 256, this);
    }
    public async UniTask InitializeAsync()
    {
        bool loaded = await shopSaveData.LoadFileAsync();

        if (!loaded)
        {
            InitDefaultData();
        }
        SaveManager.Instance.Register(shopSaveData);
        MarkDirty();
        OnShopStateChanged?.Invoke();
    }
    public void InitDefaultData()
    {
        refreshCount = 0;
        hasClaimedDailyGem = false;
        DailyReset();
    }

    private void OnDestroy()
    {
        SaveManager.Instance.Unregister(shopSaveData);
    }

    public ShopItemData GetShopItem(int index)
    {
        if (index < 0 || index >= ShopSlotCount)
        {
            return null;
        }

        if (index < 3)
        {
            return shopDataBase.GetLowItem(currentItemIndices[index]);
        }

        if (index < 5)
        {
            return shopDataBase.GetMiddleItem(currentItemIndices[index]);
        }

        return shopDataBase.GetHighItem(currentItemIndices[index]);
    }

    public bool IsPurchased(int index)
    {
        if (index < 0 || index >= ShopSlotCount)
        {
            return false;
        }

        return purchasedItems[index];
    }

    public bool RefreshShop()
    {
        int price = GetRefreshPrice();

        if(PlayerDataManager.Instance.Gem < price)
        {
            PopupManager.Instance.popup.SetPopupMessage("젬이 부족해 새로고침할 수 없습니다.");
            return false;
        }

        PlayerDataManager.Instance.UseGem(price);

        SetRandomItemIndices();

        for (int i = 0; i < ShopSlotCount; i++)
        {
            purchasedItems[i] = false;
        }

        if (refreshCount < MaxRefreshCount)
        {
            refreshCount++;
        }
        MarkDirty();
        OnShopStateChanged?.Invoke();
        return true;
    }
    public bool GetDailyGem()
    {
        if (hasClaimedDailyGem)
        {
            return false;
        }

        hasClaimedDailyGem = true;
        PlayerDataManager.Instance.AddGem(5);
        MarkDirty();
        return true;
    }
    public void DailyReset()
    {
        SetRandomItemIndices();

        for (int i = 0; i < ShopSlotCount; i++)
        {
            purchasedItems[i] = false;
        }

        refreshCount = 0;
        hasClaimedDailyGem = false;
        MarkDirty();
        OnDailyReset?.Invoke();
        OnShopStateChanged?.Invoke();
    }

    private void SetRandomItemIndices()
    {
        int lowItemCount = shopDataBase.GetLowItemCount();
        int middleItemCount = shopDataBase.GetMiddleItemCount();
        int highItemCount = shopDataBase.GetHighItemCount();
        // Low
        currentItemIndices[0] = UnityEngine.Random.Range(0, lowItemCount);

        do
        {
            currentItemIndices[1] = UnityEngine.Random.Range(0, lowItemCount);
        }
        while (currentItemIndices[1] == currentItemIndices[0]);

        do
        {
            currentItemIndices[2] = UnityEngine.Random.Range(0, lowItemCount);
        }
        while (
            currentItemIndices[2] == currentItemIndices[0] ||
            currentItemIndices[2] == currentItemIndices[1]);

        // Middle
        currentItemIndices[3] = UnityEngine.Random.Range(0, middleItemCount);

        do
        {
            currentItemIndices[4] = UnityEngine.Random.Range(0, middleItemCount);
        }
        while (currentItemIndices[4] == currentItemIndices[3]);

        // High
        currentItemIndices[5] = UnityEngine.Random.Range(0, highItemCount);
    }

    public ShopPurchaseResult CanBuy(int index)
    {
        if (purchasedItems[index])
        {
            return ShopPurchaseResult.Fail;
        }

        ShopItemData data = GetShopItem(index);

        if (data == null)
        {
            return ShopPurchaseResult.Fail;
        }

        switch (data.priceType)
        {
            case ShopPriceType.Gold:
                if(PlayerDataManager.Instance.Gold < data.price)
                {
                    return ShopPurchaseResult.NotEnoughGold;
                }
                return ShopPurchaseResult.Succese;
            case ShopPriceType.Gem:
                if(PlayerDataManager.Instance.Gem < data.price)
                {
                    return ShopPurchaseResult.NotEnoughGem;
                }
                return ShopPurchaseResult.Succese;
        }

        return ShopPurchaseResult.Succese;
    }

    public void BuyItem(int index)
    {
        ShopItemData data = GetShopItem(index);

        switch (data.priceType)
        {
            case ShopPriceType.Gold:
                PlayerDataManager.Instance.UseGold(data.price);
                break;
            case ShopPriceType.Gem:
                PlayerDataManager.Instance.UseGem(data.price);
                break;
        }

        inBoxManager.AddGift(data.category, data.level);

        purchasedItems[index] = true;
        MarkDirty();
        OnShopStateChanged?.Invoke();
        OnShopPurchase?.Invoke(1);
        return;
    }

    public int GetRefreshPrice()
    {
        return baseRefreshPrice * (1 << refreshCount);
    }
    #region SaveLoad
    public int[] GetItemIndicesForSerialization()
    {
        return currentItemIndices;
    }

    public bool[] GetPurchasedItemsForSerialization()
    {
        return purchasedItems;
    }
    public void SetRefreshCount(int count)
    {
        refreshCount = count;
    }
    public void SetClaimedDailyGem(bool claimed)
    {
        hasClaimedDailyGem = claimed;
    }
    private void MarkDirty()
    {
        shopSaveData.MarkDirty();
    }
    #endregion
}
