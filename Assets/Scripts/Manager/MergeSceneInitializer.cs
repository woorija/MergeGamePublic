using Cysharp.Threading.Tasks;
using UnityEngine;

public class MergeSceneInitializer : MonoBehaviour
{
    [SerializeField] TimeManager timeManager;
    [SerializeField] BoardManager boardManager;
    [SerializeField] OrderManager orderManager;
    [SerializeField] PlayerDataManager playerDataManager;
    [SerializeField] ItemTriggerManager itemTriggerManager;
    [SerializeField] StorageManager storageManager;
    [SerializeField] StaminaManager staminaManager;
    [SerializeField] LevelUpRewardManager levelUpRewardManager;
    [SerializeField] ContentResetManager contentResetManager;
    [SerializeField] PopupManager popupManager;
    [SerializeField] PlayerDataEventHub playerDataEventHub;
    [SerializeField] NPCPortraitImageLoader npcPortraitImageLoader;
    [SerializeField] SafeAreaPanel[] safeAreaPanels;
    [SerializeField] UIAreaPanel[] uiAreaPanels;
    [SerializeField] TopUILayout topUILayout;
    [SerializeField] BottomUILayout bottomUILayout;
    [SerializeField] OrderAreaLayout orderAreaLayout;
    [SerializeField] MainBoard mainBoard;
    [SerializeField] ItemInformationTooltip itemInformationTooltip;
    [SerializeField] MissionManager missionManager;
    [SerializeField] ShopManager shopManager;
    [SerializeField] InBoxManager inBoxManager;
    [SerializeField] SaveManager saveManager;


    public async UniTask InitializeAsync()
    {
        playerDataManager.OnStaminaChanged += staminaManager.UpdateRecoveryTimer;
        timeManager.OnSecondTick += staminaManager.RecoverStamina;
        timeManager.OnSecondTick += itemTriggerManager.OnSecondTick;
        timeManager.OnSecondTick += itemInformationTooltip.OnSecondTick;
        timeManager.OnSecondTick += saveManager.OnSecondTick;

        await playerDataManager.InitializeAsync();
        await npcPortraitImageLoader.InitializeAsync();

        boardManager.OnBoardChanged += orderManager.RefreshOrderCheckFromBoard;
        boardManager.OnBoardChanged += boardManager.RefreshHighlightFromOrders;
        orderManager.OnOrderChanged += boardManager.RefreshHighlightFromOrders;
        orderManager.OnOrderChanged += orderManager.RefreshOrderCheckFromBoard;

        await boardManager.InitializeAsync();
        playerDataEventHub.Init();
        await inBoxManager.InitializeAsync();
        await storageManager.InitializeAsync();
        await staminaManager.InitializeAsync();
        await shopManager.InitializeAsync();

        playerDataManager.OnLevelChanged += popupManager.levelUp.SetUI;
        playerDataManager.OnLevelChanged += levelUpRewardManager.GetLevelUpReward;

        await missionManager.InitializeAsync();

        boardManager.OnMerge += missionManager.AddMergeCount;
        orderManager.OnOrderComplete += missionManager.AddOrderCompleteCount;
        shopManager.OnShopPurchase += missionManager.AddShopPurchaseCount;
        playerDataManager.OnGemUsed += missionManager.AddGemSpendCount;
        playerDataManager.OnGemAdded += missionManager.AddGemEarnCount;
        playerDataManager.OnGoldUsed += missionManager.AddGoldSpendCount;
        playerDataManager.OnGoldAdded += missionManager.AddGoldEarnCount;
        playerDataManager.OnXpAdded += missionManager.AddXpEarnCount;

        staminaManager.OnDailyReset += popupManager.staminaPurchase.OnReset;
        shopManager.OnDailyReset += popupManager.shop.OnReset;
        missionManager.OnDailyReset += popupManager.dailyMission.OnReset;
        missionManager.OnWeeklyReset += popupManager.weeklyMission.OnReset;

        timeManager.OnDailyReset += contentResetManager.ResetDailyContents;
        timeManager.OnWeeklyReset += contentResetManager.ResetWeeklyContents;

        for (int i = 0; i < safeAreaPanels.Length; i++)
        {
            safeAreaPanels[i].Init();
        }
        for(int i = 0;i < uiAreaPanels.Length; i++)
        {
            uiAreaPanels[i].Init();
        }
        topUILayout.Init();
        bottomUILayout.Init();
        orderAreaLayout.Init();
        mainBoard.Init();
        popupManager.Init();
        await orderManager.InitializeAsync();
        await timeManager.InitializeAsync();
    }
    private void OnDestroy()
    {
        boardManager.OnBoardChanged -= orderManager.RefreshOrderCheckFromBoard;
        boardManager.OnBoardChanged -= boardManager.RefreshHighlightFromOrders;
        orderManager.OnOrderChanged -= boardManager.RefreshHighlightFromOrders;
        orderManager.OnOrderChanged -= orderManager.RefreshOrderCheckFromBoard;

        playerDataManager.OnStaminaChanged -= staminaManager.UpdateRecoveryTimer;
        timeManager.OnSecondTick -= staminaManager.RecoverStamina;
        timeManager.OnSecondTick -= itemTriggerManager.OnSecondTick;
        timeManager.OnSecondTick -= itemInformationTooltip.OnSecondTick;
        timeManager.OnSecondTick -= saveManager.OnSecondTick;

        playerDataManager.OnLevelChanged -= popupManager.levelUp.SetUI;
        playerDataManager.OnLevelChanged -= levelUpRewardManager.GetLevelUpReward;

        boardManager.OnMerge -= missionManager.AddMergeCount;
        orderManager.OnOrderComplete -= missionManager.AddOrderCompleteCount;
        shopManager.OnShopPurchase -= missionManager.AddShopPurchaseCount;
        playerDataManager.OnGemUsed -= missionManager.AddGemSpendCount;
        playerDataManager.OnGemAdded -= missionManager.AddGemEarnCount;
        playerDataManager.OnGoldUsed -= missionManager.AddGoldSpendCount;
        playerDataManager.OnGoldAdded -= missionManager.AddGoldEarnCount;
        playerDataManager.OnXpAdded -= missionManager.AddXpEarnCount;

        staminaManager.OnDailyReset -= popupManager.staminaPurchase.OnReset;
        shopManager.OnDailyReset -= popupManager.shop.OnReset;
        missionManager.OnDailyReset -= popupManager.dailyMission.OnReset;
        missionManager.OnWeeklyReset -= popupManager.weeklyMission.OnReset;

        timeManager.OnDailyReset -= contentResetManager.ResetDailyContents;
        timeManager.OnWeeklyReset -= contentResetManager.ResetWeeklyContents;
    }
}
