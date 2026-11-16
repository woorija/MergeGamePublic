using UnityEngine;

public class ItemInteractionManager : SingletonBehaviour<ItemInteractionManager>
{
    [SerializeField] ItemTriggerManager itemTriggerManager;
    [SerializeField] StaminaManager staminaManager;

    public void OnClick(int _slotIndex)
    {
        ItemData itemData = BoardManager.Instance.GetItemData(_slotIndex);

        switch (itemData.type)
        {
            case ItemType.Factory:
                OnClickFactory(_slotIndex, itemData);
                break;
            case ItemType.Reward:
                OnClickReward(_slotIndex, itemData);
                break;
            case ItemType.Bubble:
                OnClickBubble(_slotIndex, itemData);
                break;
            default:
                break;
        }
    }

    void OnClickFactory(int _slotIndex, ItemData _itemData)
    {
        if(!_itemData.HasSpawnCount())
        {
            return;
        }

        FactoryRuntimeData factoryData = FactoryDataBase.Instance.GetFactoryData(_itemData.category);

        if (factoryData.useStamina && !staminaManager.HasStamina(1))
        {
            return;
        }
        if (!factoryData.IsSpawnable(_itemData))
        {
            return;
        }

        int spawnIndex = BoardManager.Instance.FindNearEmptyIndex(_slotIndex);
        if (spawnIndex == -1)
        {
            return;
        }

        if (factoryData.autoRecoverSpawnCount && _itemData.IsMaxSpawnCount())
        {
            long triggerTime = TimeManager.Instance.currentTime + _itemData.statData.triggerTime;
            itemTriggerManager.Register(_itemData, triggerTime);
        }

        ItemStatData item = factoryData.GetSpawnItem(_itemData);

        BoardManager.Instance.Spawn(_slotIndex, spawnIndex, item);
        _itemData.UseSpawnCount();

        if (!_itemData.HasSpawnCount())
        {
            BoardManager.Instance.RefreshTimerUI(_itemData, true);
        }

        if (factoryData.useStamina)
        {
            staminaManager.UseStamina(1);
        }

        if(!factoryData.autoRecoverSpawnCount && !_itemData.HasSpawnCount())
        {
            BoardManager.Instance.ClearIndex(_slotIndex);
            BoardUIManager.Instance.SetTargetSlot(-1);
            BoardSlot.ResetTarget();
        }
    }

    void OnClickReward(int _slotIndex, ItemData _itemData)
    {
        var key = (_itemData.category, _itemData.level);
        int value = RewardDataBase.rewardTable[key];

        switch (_itemData.category)
        {
            case ItemCategory.Gold:
                PlayerDataManager.Instance.AddGold(value);
                break;
            case ItemCategory.Stamina:
                PlayerDataManager.Instance.AddStamina(value);
                break;
            case ItemCategory.Gem:
                PlayerDataManager.Instance.AddGem(value);
                break;
        }
        BoardManager.Instance.ClearIndex(_slotIndex);
        BoardUIManager.Instance.SetTargetSlot(-1);
    }

    void OnClickBubble(int _slotIndex, ItemData _itemData)
    {
        PopupManager.Instance.bubblePurchase.Open(_itemData);
    }

    void OnClickIce(int _slotIndex, ItemData _itemData)
    {

    }
}