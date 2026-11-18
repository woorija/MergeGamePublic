using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BoardManager : SingletonBehaviour<BoardManager>
{
    const int ROWS = 10; //세로
    const int COLS = 8; //가로
    const int MAXDIST = 10; //ROWS와 COLS 중 큰 쪽에 맞추기

    BoardState[] boardStates;
    ItemData[] itemDatas;

    private BoardSaveData boardSaveData;

    public Dictionary<(ItemCategory, int), int> itemCountTable;
    public event Action OnBoardChanged;
    public event Action<int> OnMerge;

    [SerializeField] ItemTriggerManager itemTriggerManager;
    /*
    private void Update()
    {
        PrintItemCountTable();
    }*/
    protected override void Awake()
    {
        base.Awake();
        int totalIndex = ROWS * COLS;
        boardStates = new BoardState[totalIndex];
        itemDatas = new ItemData[totalIndex];
        itemCountTable = new Dictionary<(ItemCategory, int), int>(128);
        for (int i = 0; i < totalIndex; i++)
        {
            itemDatas[i] = new ItemData();
        }
        boardSaveData = new BoardSaveData($"{Application.persistentDataPath}/{SavePath.BoardSavePath}", 4096, this);
    }
    public async UniTask InitializeAsync()
    {
        bool loaded = await boardSaveData.LoadFileAsync();

        if (!loaded)
        {
            InitDefaultData();
        }
        OnBoardChanged?.Invoke();
        SaveManager.Instance.Register(boardSaveData);
        OnBoardChanged += MarkDirty;
        MarkDirty();
    }
    private void OnDestroy()
    {
        SaveManager.Instance.Unregister(boardSaveData);
        OnBoardChanged -= MarkDirty;
    }
    private void InitDefaultData()
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
            boardStates[i] = BoardState.Hidden;
        }

        // 빨간 표시 슬롯은 기존 세팅을 유지하고, 나머지는 거리와 방향에 따라 재배치한다.
        itemDatas[0].SetData(ItemCategory.HealingHerb, 8);
        itemDatas[1].SetData(ItemCategory.ManaHerb, 7);
        itemDatas[2].SetData(ItemCategory.ManaHerb, 6);
        itemDatas[3].SetData(ItemCategory.ManaFruit, 3);
        itemDatas[4].SetData(ItemCategory.ManaHerb, 4);
        itemDatas[5].SetData(ItemCategory.ManaFruit, 4);
        itemDatas[6].SetData(ItemCategory.ManaFruit, 5);
        itemDatas[7].SetData(ItemCategory.ManaHerb, 8);
        itemDatas[8].SetData(ItemCategory.HealingFruit, 5);
        itemDatas[9].SetData(ItemCategory.ManaFruit, 4);
        itemDatas[10].SetData(ItemCategory.ManaHerb, 4);
        itemDatas[11].SetData(ItemCategory.ManaHerb, 3);
        itemDatas[12].SetData(ItemCategory.ManaFactory, 3);
        itemDatas[13].SetData(ItemCategory.ManaFruit, 3);
        itemDatas[14].SetData(ItemCategory.ManaHerb, 6);
        itemDatas[15].SetData(ItemCategory.HealingHerb, 7);
        itemDatas[16].SetData(ItemCategory.HealingHerb, 6);
        itemDatas[17].SetData(ItemCategory.HealingFruit, 3);
        itemDatas[18].SetData(ItemCategory.ManaHerb, 2);
        itemDatas[19].SetData(ItemCategory.ManaFactory, 1);
        itemDatas[20].SetData(ItemCategory.ManaFactory, 2);
        itemDatas[21].SetData(ItemCategory.ManaHerb, 3);
        itemDatas[22].SetData(ItemCategory.ManaHerb, 4);
        itemDatas[23].SetData(ItemCategory.ManaFruit, 4);
        itemDatas[24].SetData(ItemCategory.HealingFruit, 3);
        itemDatas[25].SetData(ItemCategory.HealingHerb, 4);
        itemDatas[26].SetData(ItemCategory.HealingFruit, 1);
        itemDatas[27].SetData(ItemCategory.HealingHerb, 1);
        itemDatas[28].SetData(ItemCategory.HealingHerb, 2);
        itemDatas[29].SetData(ItemCategory.ManaHerb, 2);
        itemDatas[30].SetData(ItemCategory.ManaFruit, 3);
        itemDatas[31].SetData(ItemCategory.StaminaHerb, 4);
        itemDatas[32].SetData(ItemCategory.HealingHerb, 4);
        itemDatas[33].SetData(ItemCategory.HealingFruit, 2);
        itemDatas[34].SetData(ItemCategory.HealingHerb, 1);
        itemDatas[35].SetData(ItemCategory.HealingFactory, 2);
        itemDatas[36].SetData(ItemCategory.HealingFactory, 3);
        itemDatas[37].SetData(ItemCategory.HealingFruit, 1);
        itemDatas[38].SetData(ItemCategory.StaminaHerb, 3);
        itemDatas[39].SetData(ItemCategory.StaminaFruit, 3);
        itemDatas[40].SetData(ItemCategory.HealingFruit, 3);
        itemDatas[41].SetData(ItemCategory.HealingHerb, 3);
        itemDatas[42].SetData(ItemCategory.HealingHerb, 2);
        itemDatas[43].SetData(ItemCategory.HealingFactory, 1);
        itemDatas[44].SetData(ItemCategory.HealingFactory, 1);
        itemDatas[45].SetData(ItemCategory.HealingFruit, 2);
        itemDatas[46].SetData(ItemCategory.StaminaFruit, 2);
        itemDatas[47].SetData(ItemCategory.HealingHerb, 4);
        itemDatas[48].SetData(ItemCategory.HealingHerb, 4);
        itemDatas[49].SetData(ItemCategory.HealingHerb, 4);
        itemDatas[50].SetData(ItemCategory.HealingHerb, 3);
        itemDatas[51].SetData(ItemCategory.HealingHerb, 1);
        itemDatas[52].SetData(ItemCategory.HealingHerb, 2);
        itemDatas[53].SetData(ItemCategory.StaminaFruit, 2);
        itemDatas[54].SetData(ItemCategory.HealingHerb, 4);
        itemDatas[55].SetData(ItemCategory.HealingFruit, 3);
        itemDatas[56].SetData(ItemCategory.HealingFruit, 4);
        itemDatas[57].SetData(ItemCategory.HealingFruit, 3);
        itemDatas[58].SetData(ItemCategory.StaminaHerb, 4);
        itemDatas[59].SetData(ItemCategory.StaminaFactory, 1);
        itemDatas[60].SetData(ItemCategory.StaminaFactory, 2);
        itemDatas[61].SetData(ItemCategory.StaminaHerb, 3);
        itemDatas[62].SetData(ItemCategory.StaminaHerb, 4);
        itemDatas[63].SetData(ItemCategory.ManaHerb, 6);
        itemDatas[64].SetData(ItemCategory.HealingFruit, 5);
        itemDatas[65].SetData(ItemCategory.StaminaFruit, 4);
        itemDatas[66].SetData(ItemCategory.StaminaHerb, 4);
        itemDatas[67].SetData(ItemCategory.StaminaHerb, 3);
        itemDatas[68].SetData(ItemCategory.StaminaFactory, 3);
        itemDatas[69].SetData(ItemCategory.StaminaFruit, 3);
        itemDatas[70].SetData(ItemCategory.StaminaHerb, 6);
        itemDatas[71].SetData(ItemCategory.StaminaHerb, 7);
        itemDatas[72].SetData(ItemCategory.StaminaHerb, 8);
        itemDatas[73].SetData(ItemCategory.StaminaHerb, 7);
        itemDatas[74].SetData(ItemCategory.StaminaHerb, 6);
        itemDatas[75].SetData(ItemCategory.StaminaFruit, 3);
        itemDatas[76].SetData(ItemCategory.StaminaHerb, 9);
        itemDatas[77].SetData(ItemCategory.StaminaFruit, 4);
        itemDatas[78].SetData(ItemCategory.StaminaFruit, 5);
        itemDatas[79].SetData(ItemCategory.StaminaFruit, 6);

        // =========================
        // BoardState
        // =========================
        boardStates[44] = BoardState.Item;
        boardStates[43] = BoardState.Blocked;

        // =========================
        // UI
        // =========================

        RestoreBoardState();
    }
    void SetAllItemCount()
    {
        itemCountTable.Clear();
        for (int i = 0; i < itemDatas.Length; i++)
        {
            if (boardStates[i] == BoardState.Item)
            {
                AddItemCount(itemDatas[i]);
            }
        }
    }
    public void RestoreBoardState()
    {
        BoardUIManager.Instance.RestoreBoardUI(itemDatas, boardStates);
        RegisterAllTrigger();
        SetAllItemCount();
        OnBoardChanged?.Invoke();
    }
    void RegisterAllTrigger()
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
            bool isSpecialType = itemDatas[i].type == ItemType.Bubble || itemDatas[i].type == ItemType.Ice;
            bool hasFactoryTrigger = itemDatas[i].type == ItemType.Factory && itemDatas[i].triggerTimestamp != 0;

            if (isSpecialType || hasFactoryTrigger)
            {
                itemTriggerManager.Register(itemDatas[i], itemDatas[i].triggerTimestamp);
            }
        }
    }
    public void PrintItemCountTable()
    {
        StringBuilder sb = new StringBuilder();
        sb.Clear();

        int totalCount = 0;

        sb.AppendLine("===== Item Count Table =====");

        foreach (var pair in itemCountTable)
        {

            totalCount += pair.Value;

            sb.Append(pair.Key.Item1);
            sb.Append(" Level ");
            sb.Append(pair.Key.Item2);
            sb.Append(" : ");
            sb.Append(pair.Value);
            sb.AppendLine("개");
        }

        sb.Append("Total : ");
        sb.Append(totalCount);
        sb.AppendLine("개");

        UnityEngine.Debug.Log(sb.ToString());
    }
    public void RefreshHighlightFromOrders()
    {
        var orderTable = OrderManager.Instance.orderItemTable;
        for (int i = 0; i < itemDatas.Length; i++)
        {
            if(boardStates[i] != BoardState.Item)
            {
                continue;
            }
            if (itemDatas[i].type != ItemType.Normal)
            {
                continue;
            }
            var key = (itemDatas[i].category, itemDatas[i].level);
            bool hasOrder = orderTable.ContainsKey(key);

            BoardUIManager.Instance.RefreshHighLight(i, hasOrder);
        }
    }
    public void RefreshTimerUI(int index, bool active)
    {
        BoardUIManager.Instance.RefreshTimer(index, active);
    }
    public void RefreshTimerUI(ItemData itemData, bool active)
    {
        int index = GetItemIndex(itemData);
        RefreshTimerUI(index, active);
    }
    public void ResolveDrag(int from, int to)
    {
        if (boardStates[from] == BoardState.Empty)
        {
            return;
        }
        if (from == to)
        {
            BoardUIManager.Instance.RefreshUI(from, itemDatas[from]).Forget();
            return;
        }
        TryMoveOrMerge(from, to);
    }
    public void Spawn(int slotIndex, int spawnIndex, ItemStatData item) // 팩토리 클릭으로 아이템 획득
    {
        itemDatas[spawnIndex].SetData(item);
        boardStates[spawnIndex] = BoardState.Item;
        BoardUIManager.Instance.Spawn(slotIndex, spawnIndex, itemDatas[spawnIndex]).Forget();
        AddItemCount(itemDatas[spawnIndex]);
        OnBoardChanged?.Invoke();
    }
    void TrySpawnSpecialItem(ItemStatData item, int slotIndex)
    {
        TrySpawnBubble(item, slotIndex);
        TrySpawnIce(item, slotIndex);
    }
    void TrySpawnBubble(ItemStatData item, int slotIndex)
    {
        int bubbleSpawnRate = item.itemType switch
        {
            ItemType.Reward => 0,
            ItemType.Factory => 5,
            ItemType.Normal => 10,
            _ => 0
        };

        if(UnityEngine.Random.Range(0,100) < bubbleSpawnRate)
        {
            int spawnIndex = FindNearEmptyIndex(slotIndex);
            ItemStatData bubbleItem = SpecialItemDataBase.GetItemStatData(ItemType.Bubble, item.category, item.level);
            ItemData itemData = itemDatas[spawnIndex];

            itemData.SetData(bubbleItem);
            boardStates[spawnIndex] = BoardState.Item;

            long triggerTimestamp = TimeManager.Instance.currentTime + itemData.statData.triggerTime;
            itemTriggerManager.Register(itemData, triggerTimestamp);
            BoardUIManager.Instance.Spawn(slotIndex, spawnIndex, itemDatas[spawnIndex]).Forget();
            OnBoardChanged?.Invoke();
        }
    }
    void TrySpawnIce(ItemStatData item, int slotIndex)
    {

    }
    public bool TryGetStorageItem(ItemData itemData)
    {
        int emptyIndex = FindEmptyIndex();
        if(emptyIndex == -1)
        {
            return false;
        }
        itemDatas[emptyIndex].CopyFrom(itemData);

        boardStates[emptyIndex] = BoardState.Item;
        AddItemCount(itemDatas[emptyIndex]);
        BoardUIManager.Instance.RefreshUI(emptyIndex, itemDatas[emptyIndex]).Forget();
        OnBoardChanged?.Invoke();

        return true;
    }
    public bool TryAcquireItem(ItemCategory category, int level) // InBox에서 아이템 획득
    {
        int emptyIndex = FindEmptyIndex();
        if (emptyIndex == -1)
        {
            return false;
        }

        ItemData itemData = itemDatas[emptyIndex];
        itemData.SetData(category, level);

        boardStates[emptyIndex] = BoardState.Item;
        AddItemCount(itemData);
        BoardUIManager.Instance.Spawn(emptyIndex, itemData).Forget();
        OnBoardChanged?.Invoke();

        return true;
    }
    public void AddItemCount(ItemData itemData)
    {
        if (!IsCountable(itemData))
        {
            return;
        }

        var key = (itemData.category, itemData.level);

        itemCountTable.TryGetValue(key, out int currentCount);
        itemCountTable[key] = currentCount + 1;
    }
    public void RemoveItemCount(ItemData itemData)
    {
        if (!IsCountable(itemData))
        {
            return;
        }

        var key = (itemData.category, itemData.level);

        itemCountTable.TryGetValue(key, out int currentCount);

        currentCount--;

        if (currentCount <= 0)
        {
            itemCountTable.Remove(key);
        }
        else
        {
            itemCountTable[key] = currentCount;
        }
    }
    bool IsCountable(ItemData itemData)
    {
        return itemData.type == ItemType.Normal;
    }
    void TryMoveOrMerge(int from, int to)
    {
        BoardSlot.ResetTarget();
        if (boardStates[to] == BoardState.Hidden)
        {
            BoardUIManager.Instance.RefreshUI(from, itemDatas[from]).Forget();
            return;
        }
        else if (boardStates[to] == BoardState.Empty)
        {
            BoardUIManager.Instance.Swap(from, to, itemDatas[from], itemDatas[to]).Forget();
            Swap(from, to);
        }
        else if (boardStates[to] == BoardState.Blocked)
        {
            if (CanMerge(from, to))
            {
                // Blocked items are not included in itemCountTable. Capture this before
                // changing the state to Item so the merge does not remove an uncounted target.
                bool targetWasCounted = IsTargetCounted(to);
                ItemData beforeItemData = new ItemData(itemDatas[to]);
                boardStates[to] = BoardState.Item;
                BoardUIManager.Instance.RefreshBoardOverlayImage(to, boardStates[to]).Forget();
                BoardUIManager.Instance.Merge(to, beforeItemData, itemDatas[to]).Forget();
                Merge(from, to, targetWasCounted);
                UnlockAdjacentHidden(to);
            }
            else
            {
                BoardUIManager.Instance.RefreshUI(from, itemDatas[from]).Forget();
            }
        }
        else if (boardStates[to] == BoardState.Item)
        {
            if (CanMerge(from, to))
            {
                bool targetWasCounted = IsTargetCounted(to);
                ItemData beforeItemData = new ItemData(itemDatas[to]);
                BoardUIManager.Instance.Merge(to, beforeItemData, itemDatas[to]).Forget();
                Merge(from, to, targetWasCounted);
            }
            else
            {
                BoardUIManager.Instance.Swap(from, to, itemDatas[from], itemDatas[to]).Forget();
                Swap(from, to);
            }
        }
    }
    bool CanMerge(int from, int to)
    {
        ItemData fromItem = itemDatas[from];
        ItemData toItem = itemDatas[to];
        if (itemDatas[from].type is ItemType.Bubble or ItemType.Ice || itemDatas[to].type == ItemType.Bubble) return false;
        if (itemDatas[from].category != itemDatas[to].category) return false;
        if (itemDatas[from].level != itemDatas[to].level) return false;
        if (itemDatas[from].level == ItemMaxLevelDataBase.GetMaxLevel(itemDatas[from].category)) return false;
        if (itemDatas[from].type == ItemType.Ice) return false;
        return true;
    }
    void Merge(int from, int to, bool targetWasCounted)
    {
        ClearIndex(from);

        ItemData toItem = itemDatas[to];

        if (targetWasCounted)
        {
            RemoveItemCount(toItem);
        }

        toItem.SetData(toItem.category, toItem.level + 1);

        AddItemCount(toItem);

        TrySpawnSpecialItem(toItem.statData, to);

        OnMerge?.Invoke(1);
        OnBoardChanged?.Invoke();
    }
    bool IsTargetCounted(int index)
    {
        return boardStates[index] == BoardState.Item && IsCountable(itemDatas[index]);
    }
    void Swap(int from, int to)
    {
        ItemData tempData = itemDatas[from];
        itemDatas[from] = itemDatas[to];
        itemDatas[to] = tempData;
        BoardState tempState = boardStates[from];
        boardStates[from] = boardStates[to];
        boardStates[to] = tempState;
    }
    public void RemoveOrderItems(OrderData orderData)
    {
        for (int i = 0; i < orderData.requirementCount; i++)
        {
            var requirement = orderData.requirements[i];

            for (int j = 0; j < itemDatas.Length; j++)
            {
                if (itemDatas[j].category != requirement.category)
                {
                    continue;
                }
                if (itemDatas[j].level != requirement.level)
                {
                    continue;
                }
                if (itemDatas[j].type != ItemType.Normal)
                {
                    continue;
                }
                if (boardStates[j] != BoardState.Item)
                {
                    continue;
                }
                ClearIndex(j);
                break;
            }
        }
        OnBoardChanged.Invoke();
    }
    public BubblePurchaseResult PurchaseBubbleItem(ItemData itemData)
    {
        if(itemData.type != ItemType.Bubble)
        {
            return BubblePurchaseResult.NotBubble;
        }
        ItemCategory category = itemData.category;
        int level = itemData.level;
        int price = BubblePriceDataBase.BubblePriceTable[(category, level)];

        if(!PlayerDataManager.Instance.TryUseGem(price))
        {
            return BubblePurchaseResult.NotEnoughGem;
        }
        itemData.SetData(category, level);
        AddItemCount(itemData);
        RefreshBoardSlot(itemData);
        OnBoardChanged?.Invoke();
        return BubblePurchaseResult.Success;
    }
    public ItemSellType GetSellType(int index)
    {
        ItemData itemData = GetItemData(index);
        if(itemData == null)
        {
            return ItemSellType.None;
        }
        else if(itemData.type == ItemType.Bubble || itemData.type == ItemType.Ice)
        {
            return ItemSellType.ToGoldItem;
        }
        else if(itemData.level >= 3)
        {
            return ItemSellType.RewardGold;
        }
        else
        {
            return ItemSellType.NoReward;
        }
    }
    public ItemSellResult SellItem(int index)
    {
        ItemSellType sellType = GetSellType(index);
        if (sellType == ItemSellType.None)
        {
            return ItemSellResult.NotSellable;
        }
        ItemData itemData = GetItemData(index);
        if(itemData.type == ItemType.Reward)
        {
            return ItemSellResult.StateChanged;
        }

        switch (sellType)
        {
            case ItemSellType.ToGoldItem:
                ConvertToGold(itemData);
                break;
            case ItemSellType.RewardGold:
                PlayerDataManager.Instance.AddGold(1);
                ClearIndex(index);
                break;
            case ItemSellType.NoReward:
                ClearIndex(index);
                break;
        }
        BoardSlot.ResetTarget();
        OnBoardChanged?.Invoke();
        return ItemSellResult.Success;
    }
    public void ConvertToGold(ItemData itemData)
    {
        itemData.SetData(ItemCategory.Gold, 1);
        RefreshBoardSlot(itemData);
        OnBoardChanged?.Invoke();
    }
    public void RefreshBoardSlot(ItemData itemData)
    {
        int index = GetItemIndex(itemData);
        BoardUIManager.Instance.RefreshUI(index, itemData).Forget();
        BoardSlot.ResetTarget();
    }
    public ItemData GetItemData(int _index)
    {
        return itemDatas[_index];
    }

    private int GetItemIndex(ItemData itemData)
    {
        int index = -1;
        
        for (int i = 0; i < itemDatas.Length; i++)
        {
            if(ReferenceEquals(itemDatas[i], itemData))
            {
                index = i;
                break;
            }
        }
        return index;
    }
    public void ClearIndex(int _index)
    {
        ItemData itemData = itemDatas[_index];

        RemoveItemCount(itemData);
        itemData.Clear();
        boardStates[_index] = BoardState.Empty;
        BoardUIManager.Instance.RefreshHighLight(_index, false);
        BoardUIManager.Instance.ClearSlotImage(_index);
        OnBoardChanged?.Invoke();
    }
    public BoardState GetBoardState(int _index)
    {
        return boardStates[_index];
    }
    public ItemType GetItemType(int _index)
    {
        return itemDatas[_index].type;
    }
    public bool CanDrag(int _index)
    {
        if(boardStates[_index] == BoardState.Item)
        {
            return true;
        }
        return false;
    }
    public int FindEmptyIndex()
    {
        for (int i = 0; i < boardStates.Length; i++)
        {
            if (boardStates[i] == BoardState.Empty)
            {
                return i;
            }
        }
        return -1;
    }
    public int FindNearEmptyIndex(int _index)
    {
        int fx = _index % COLS;
        int fy = _index / COLS;

        for (int dist = 1; dist < MAXDIST; dist++)
        {
            int seg1 = dist + 1;
            int seg2 = dist * 2;
            int seg3 = dist * 2;
            int seg4 = dist * 2;
            int seg5 = dist - 1;

            int sum2 = seg1 + seg2;
            int sum3 = sum2 + seg3;
            int sum4 = sum3 + seg4;
            int total = sum4 + seg5;

            for (int i = 0; i < total; i++)
            {
                int dx, dy;

                if (i < seg1)
                {
                    dx = i;
                    dy = -dist;
                }
                else if (i < sum2)
                {
                    int j = i - seg1;
                    dx = dist;
                    dy = -dist + 1 + j;
                }
                else if (i < sum3)
                {
                    int j = i - sum2;
                    dx = dist - 1 - j;
                    dy = dist;
                }
                else if(i < sum4)
                {
                    int j = i - sum3;
                    dx = -dist;
                    dy = dist - 1 - j;
                }
                else
                {
                    int j = i - sum4;
                    dx = -dist + 1 + j;
                    dy = -dist;
                }

                int nx = fx + dx;
                int ny = fy + dy;

                if (nx < 0 || nx >= COLS || ny < 0 || ny >= ROWS)
                {
                    continue;
                }

                int index = ny * COLS + nx;

                if (boardStates[index] == BoardState.Empty)
                {
                    return index;
                }
            }
        }
        return -1;
    }
    void UnlockAdjacentHidden(int _index)
    {
        int x = _index % COLS;
        int y = _index / COLS;

        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                if (nx < 0 || nx >= COLS || ny < 0 || ny >= ROWS) continue;

                int adjacentIndex = ny * COLS + nx;
                if (boardStates[adjacentIndex] == BoardState.Hidden)
                {
                    boardStates[adjacentIndex] = BoardState.Blocked;
                    BoardUIManager.Instance.RefreshBoardOverlayImage(adjacentIndex, boardStates[adjacentIndex]).Forget();
                }
            }
        }
    }
    #region SaveLoad
    public BoardState[] GetBoardStatesForSerialization()
    {
        return boardStates;
    }
    public ItemData[] GetItemDatasForSerialization()
    {
        return itemDatas;
    }
    private void MarkDirty()
    {
        boardSaveData.MarkDirty();
    }
    #endregion
}
