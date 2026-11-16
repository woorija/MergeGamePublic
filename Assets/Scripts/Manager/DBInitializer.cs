using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DBInitializer : MonoBehaviour
{
    [SerializeField] ItemImageLoader itemImageLoader;
    [SerializeField] FactoryDataBase factoryDataBase;
    [SerializeField] ItemDataBase itemDataBase;
    [SerializeField] SpecialItemDataBase specialItemDataBase;
    [SerializeField] BubblePriceDataBase bubblePriceDataBase;
    [SerializeField] ItemMaxLevelDataBase itemMaxLevelDataBase;
    [SerializeField] RewardDataBase rewardDataBase;
    [SerializeField] ItemNameDataBase itemNameDataBase;
    [SerializeField] OrderRewardDatabase orderRewardDatabase;
    [SerializeField] OrderRuleLevelDataBase orderRuleLevelDataBase;
    [SerializeField] XpDataBase xpDataBase;
    [SerializeField] LevelUpRewardDataBase levelUpRewardDataBase;
    [SerializeField] MissionDataBase missionDataBase;
    [SerializeField] ShopDataBase shopDataBase;

    public async UniTask InitializeAsync(ProgressUI _progressUI)
    {
        _progressUI.SetInfoText("테이블 로딩");
        _progressUI.SetSlider(0f);

        await ItemDataBaseInitAsync();
        _progressUI.SetSlider(0.3f);

        await SpecialItemDataBaseInitAsync();
        _progressUI.SetSlider(0.35f);

        await BubblePriceDataInitAsync();
        _progressUI.SetSlider(0.35f);

        await ItemMaxLevelDataBaseInitAsync();
        _progressUI.SetSlider(0.4f);

        await RewardDataBaseInitAsync();
        _progressUI.SetSlider(0.5f);

        await ItemNameDataBaseInitAsync();
        _progressUI.SetSlider(0.7f);

        await ItemAtlasInitAsync();
        _progressUI.SetSlider(0.9f);

        await OrderRewardDataBaseInitAsync();
        _progressUI.SetSlider(0.95f);

        await OrderRuleLevelDataBaseInitAsync();
        _progressUI.SetSlider(0.95f);

        await XpDataBaseInitAsync();
        _progressUI.SetSlider(0.98f);

        await LevelUpRewardDataBaseInitAsync();
        _progressUI.SetSlider(0.99f);

        await MissionDataBaseInitAsync();
        _progressUI.SetSlider(0.99f);

        await ShopDataBaseInitAsync();
        _progressUI.SetSlider(0.99f);

        await FactoryDataInitAsync();
        _progressUI.SetSlider(1.0f);



        await UniTask.Delay(100);
    }

    async UniTask ItemAtlasInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<ItemAtlasTableSO>("ItemAtlasTableSO");
        ItemAtlasTableSO itemAtlasTableSO = await handle.ToUniTask();
        itemImageLoader.Init(itemAtlasTableSO);
        Addressables.Release(handle);
    }

    async UniTask FactoryDataInitAsync()
    {
        await factoryDataBase.LoadTableAsync();
    }

    async UniTask ItemDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<ItemStatTableSO>("ItemStatTableSO");
        ItemStatTableSO itemStatTableSO = await handle.ToUniTask();
        itemDataBase.Init(itemStatTableSO);
        Addressables.Release(handle);
    }
    async UniTask SpecialItemDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<SpecialItemStatTableSO>("SpecialItemStatTableSO");
        SpecialItemStatTableSO specialItemStatTableSO = await handle.ToUniTask();
        specialItemDataBase.Init(specialItemStatTableSO);
        Addressables.Release(handle);
    }
    async UniTask BubblePriceDataInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<BubblePriceTableSO>("BubblePriceTableSO");
        BubblePriceTableSO bubblePriceTableSO = await handle.ToUniTask();
        bubblePriceDataBase.Init(bubblePriceTableSO);
        Addressables.Release(handle);
    }
    async UniTask ItemMaxLevelDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<ItemMaxLevelTableSO>("ItemMaxLevelTableSO");
        ItemMaxLevelTableSO itemMaxLevelTableSO = await handle.ToUniTask();
        itemMaxLevelDataBase.Init(itemMaxLevelTableSO);
        Addressables.Release(handle);
    }

    async UniTask ItemNameDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<ItemNameTableSO>("ItemNameTableSO");
        ItemNameTableSO itemNameTableSO = await handle.ToUniTask();
        itemNameDataBase.Init(itemNameTableSO); 
        Addressables.Release(handle);
    }

    async UniTask RewardDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<RewardTableSO>("RewardTableSO");
        RewardTableSO rewardTableSO = await handle.ToUniTask();
        rewardDataBase.Init(rewardTableSO);
        Addressables.Release(handle);
    }

    async UniTask OrderRewardDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<OrderRewardTableSO>("OrderRewardTableSO");
        OrderRewardTableSO orderRewardTableSO = await handle.ToUniTask();
        orderRewardDatabase.Init(orderRewardTableSO);
        Addressables.Release(handle);
    }

    async UniTask OrderRuleLevelDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<OrderRuleLevelTableSO>("OrderRuleLevelTableSO");
        OrderRuleLevelTableSO orderRuleLevelTableSO = await handle.ToUniTask();
        orderRuleLevelDataBase.Init(orderRuleLevelTableSO);
    }

    async UniTask XpDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<XpTableSO>("XpTableSO");
        XpTableSO xpTableSO = await handle.ToUniTask();
        xpDataBase.Init(xpTableSO);
    }
    async UniTask LevelUpRewardDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<LevelUpRewardTableSO>("LevelUpRewardTableSO");
        LevelUpRewardTableSO levelUpRewardTableSO = await handle.ToUniTask();
        levelUpRewardDataBase.Init(levelUpRewardTableSO);
    }
    async UniTask MissionDataBaseInitAsync()
    {
        missionDataBase.Init();
        var dailyHandle = Addressables.LoadAssetAsync<DailyMissionTableSO>("DailyMissionTableSO");
        DailyMissionTableSO dailyMissionTableSO = await dailyHandle.ToUniTask();
        missionDataBase.SetDailyMissionTableSO(dailyMissionTableSO);

        var weeklyT1Handle = Addressables.LoadAssetAsync<WeeklyMissionTableSO>("WeeklyMissionTier1TableSO");
        WeeklyMissionTableSO weeklyMissionTier1TableSO = await weeklyT1Handle.ToUniTask();
        missionDataBase.AddWeeklyMissionTableSO(WeeklyMissionTier.Tier1,weeklyMissionTier1TableSO);
    }
    async UniTask ShopDataBaseInitAsync()
    {
        var handle = Addressables.LoadAssetAsync<ShopTableSO>("ShopTableSO");
        ShopTableSO shopTableSO = await handle.ToUniTask();
        shopDataBase.Init(shopTableSO);
    }
}
