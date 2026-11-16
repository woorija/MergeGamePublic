using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class OrderManager : SingletonBehaviour<OrderManager>
{
    [SerializeField] OrderUIManager orderUIManager;
    const int MaxOrderCount = 7;
    const int MaxGenerateTryCount = 100;

    OrderData[] orders = new OrderData[MaxOrderCount];

    int activeOrderCount;

    private OrderSaveData orderSaveData;

    [SerializeField] OrderRuleSO orderRuleSO;
    AsyncOperationHandle<OrderRuleSO> currentRuleHandle;

    public Dictionary<(ItemCategory, int), int> orderItemTable;
    public event Action OnOrderChanged;
    public event Action<int> OnOrderComplete;
    int currentRuleLevel;

    public int ActiveOrderCount => activeOrderCount;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < MaxOrderCount; i++)
        {
            orders[i] = new OrderData();
        }
        orderItemTable = new Dictionary<(ItemCategory, int), int>();
        orderSaveData = new OrderSaveData($"{Application.persistentDataPath}/{SavePath.OrderSavePath}", 1024, this);
    }
    public async UniTask InitializeAsync() // 플레이어데이터 초기화 후 실행
    {
        int playerLevel = PlayerDataManager.Instance.Level;
        PlayerDataManager.Instance.OnLevelChanged += OnLevelChanged;
        orderUIManager.Init();
        await LoadOrderRuleForLevelAsync(playerLevel);

        bool loaded = await orderSaveData.LoadFileAsync();

        if (!loaded)
        {
            
        }

        SaveManager.Instance.Register(orderSaveData);
        OnOrderChanged?.Invoke();
        MarkDirty();
    }
    private void OnDestroy()
    {
        PlayerDataManager.Instance.OnLevelChanged -= OnLevelChanged;
        SaveManager.Instance.Unregister(orderSaveData);
        ReleaseCurrentRule();
    }
    public void test()
    {
        RefreshOrderCount();
        for(int i = 0;i < activeOrderCount; i++)
        {
            GenerateOrder(orders[i]);
            InitOrderState(i);
            orderUIManager.RefreshOrder(i, orders[i]);
        }

        StringBuilder sb = new StringBuilder(1024);

        for (int i = 0; i < activeOrderCount; i++)
        {
            OrderData order = orders[i];

            sb.Append("오더번호 : ");
            sb.Append(i);
            sb.AppendLine();

            sb.Append("요구 아이템 개수 : ");
            sb.Append(order.requirementCount);
            sb.AppendLine();

            sb.AppendLine("오더 아이템");

            for (int j = 0; j < order.requirementCount; j++)
            {
                OrderRequirement requirement =
                    order.requirements[j];

                sb.Append('(');
                sb.Append(requirement.category);
                sb.Append(", ");
                sb.Append(requirement.level);
                sb.Append(')');
                sb.AppendLine();
            }

            sb.AppendLine("리워드");

            sb.Append("xp : ");
            sb.Append(order.totalRewardWeight.xp);
            sb.AppendLine();

            sb.Append("gold : ");
            sb.Append(order.totalRewardWeight.gold);
            sb.AppendLine();

            sb.AppendLine("--------------------------------");
        }
        foreach(var orderItem in orderItemTable)
        {
            sb.Append(orderItem.Key);
            sb.Append(":");
            sb.Append(orderItem.Value);
            sb.Append("개");
            sb.AppendLine();
        }
        sb.AppendLine("--------------------------------");
        Debug.Log(sb.ToString());
    }

    private void RefreshOrderCount()
    {
        while (activeOrderCount < orderRuleSO.maxOrderCount)
        {
            GenerateOrder(orders[activeOrderCount]);
            InitOrderState(activeOrderCount);
            orderUIManager.RefreshOrder(activeOrderCount, orders[activeOrderCount]);
            activeOrderCount++;
        }
    }
    public void RefreshOrderCheckFromBoard()
    {
        var itemTable = BoardManager.Instance.itemCountTable;
        for (int i = 0; i < activeOrderCount; i++)
        {
            bool canComplete = true;

            for (int j = 0; j < orders[i].requirementCount; j++)
            {
                var key = (orders[i].requirements[j].category, orders[i].requirements[j].level);
                bool hasItem = itemTable.ContainsKey(key);

                orders[i].requirementSatisfied[j] = hasItem;

                if (!hasItem)
                {
                    canComplete = false;
                }
            }

            orders[i].canComplete = canComplete;
        }

        orderUIManager.RefreshOrderStates(activeOrderCount, orders);
    }
    private void InitOrderState(int index)
    {
        bool canComplete = true;
        var itemTable = BoardManager.Instance.itemCountTable;
        OrderData order = orders[index];

        for (int i = 0; i < order.requirementCount; i++)
        {
            var key = (order.requirements[i].category, order.requirements[i].level);
            bool hasItem = itemTable.ContainsKey(key);

            order.requirementSatisfied[i] = hasItem;

            if (!hasItem)
            {
                canComplete = false;
            }
        }

        order.canComplete = canComplete;
    }
    #region 주문 생성
    private void GenerateOrder(OrderData order)
    {
        UnregisterOrder(order);
        for (int tryCount = 0; tryCount < MaxGenerateTryCount; tryCount++)
        {
            order.Clear();

            CreateRandomOrder(order);

            ApplyLevelLimitRule(order);

            if (HasDuplicateRequirement(order))
            {
                continue;
            }

            if (IsDuplicateOrder(order))
            {
                continue;
            }

            order.npcId = UnityEngine.Random.Range(1, 13);
            RegisterOrder(order);
            CalculateReward(order);
            MarkDirty();
            OnOrderChanged?.Invoke();
            return;
        }

        Debug.LogError("Generate Order Failed");
    }
    private void UnregisterOrder(OrderData order)
    {
        for (int i = 0; i < order.requirementCount; i++)
        {
            AddItemCount(order.requirements[i].category, order.requirements[i].level, -1);
        }
    }
    private void RegisterOrder(OrderData order)
    {
        for (int i = 0; i < order.requirementCount; i++)
        {
            AddItemCount(order.requirements[i].category, order.requirements[i].level, 1);
        }
    }
    private void CreateRandomOrder(OrderData order)
    {
        order.requirementCount = UnityEngine.Random.Range(1, orderRuleSO.maxRequirementCount + 1);

        for (int i = 0; i < order.requirementCount; i++)
        {
            CreateRandomRequirement(ref order.requirements[i]);
        }
    }
    private void CreateRandomRequirement(ref OrderRequirement requirement)
    {
        int randomIndex = UnityEngine.Random.Range(0, orderRuleSO.orderItems.Count);

        OrderItemData itemData = orderRuleSO.orderItems[randomIndex];

        requirement.category = itemData.category;

        requirement.level = UnityEngine.Random.Range(3, itemData.maxLevel + 1);
    }
    private void ApplyLevelLimitRule(OrderData order)
    {
        int highestIndex = -1;
        int highestLevel = int.MinValue;

        for (int i = 0; i < order.requirementCount; i++)
        {
            int level = order.requirements[i].level;

            if (level >= orderRuleSO.itemLevelLimit && level > highestLevel)
            {
                highestLevel = level;
                highestIndex = i;
            }
        }

        if (highestIndex < 0)
        {
            return;
        }

        int maxLevel = orderRuleSO.itemLevelLimit - 3;

        for (int i = 0; i < order.requirementCount; i++)
        {
            if (i == highestIndex)
            {
                continue;
            }

            ref OrderRequirement requirement = ref order.requirements[i];

            if (requirement.level > maxLevel)
            {
                requirement.level = maxLevel;
            }
        }
    }
    private bool HasDuplicateRequirement(OrderData order)
    {
        for (int i = 0; i < order.requirementCount; i++)
        {
            OrderRequirement a = order.requirements[i];

            for (int j = i + 1; j < order.requirementCount; j++)
            {
                OrderRequirement b = order.requirements[j];

                if (a.category == b.category && a.level == b.level)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool IsDuplicateOrder(OrderData targetOrder)
    {
        for (int i = 0; i < activeOrderCount; i++)
        {
            OrderData compareOrder = orders[i];
            if(ReferenceEquals(compareOrder, targetOrder))
            {
                continue;
            }
            if (IsSameOrder(compareOrder, targetOrder))
            {
                return true;
            }
        }
        return false;
    }
    private bool IsSameOrder(OrderData a, OrderData b)
    {
        if (a.requirementCount != b.requirementCount)
        {
            return false;
        }

        int matchCount = 0;

        for (int i = 0; i < a.requirementCount; i++)
        {
            OrderRequirement aRequirement = a.requirements[i];

            for (int j = 0; j < b.requirementCount; j++)
            {
                OrderRequirement bRequirement = b.requirements[j];

                if (aRequirement.category == bRequirement.category && aRequirement.level == bRequirement.level)
                {
                    matchCount++;
                    break;
                }
            }
        }

        return matchCount == a.requirementCount;
    }
    private void CalculateReward(OrderData order)
    {
        order.totalRewardWeight = default;
        for (int i = 0; i < order.requirementCount; i++)
        {
            RewardWeight rewardWeight = OrderRewardDatabase.orderRewardTable[(order.requirements[i].category, order.requirements[i].level)];

            order.totalRewardWeight.gold += rewardWeight.gold;
            order.totalRewardWeight.xp += rewardWeight.xp;
        }
    }
    #endregion
    private void AddItemCount(ItemCategory category, int level, int count)
    {
        var key = (category, level);
        orderItemTable.TryGetValue(key, out int currentCount);
        orderItemTable[key] = currentCount + count;
        if (orderItemTable[key] <= 0)
        {
            orderItemTable.Remove(key);
        }
    }
    public void OrderComplete(int index) //오더완료버튼에 할당
    {
        PlayerDataManager.Instance.AddXp(orders[index].totalRewardWeight.xp);
        PlayerDataManager.Instance.AddGold(orders[index].totalRewardWeight.gold);
        OnOrderComplete?.Invoke(1);
        BoardManager.Instance.RemoveOrderItems(orders[index]);
        GenerateOrder(orders[index]);
        orderUIManager.RefreshOrder(index, orders[index]);
    }
    public void RestoreOrderState()
    {
        orderItemTable.Clear();

        for (int i = 0; i < activeOrderCount; i++)
        {
            RegisterOrder(orders[i]);
            CalculateReward(orders[i]);
            orderUIManager.RefreshOrder(i, orders[i]);
        }

        RefreshOrderCheckFromBoard();
        OnOrderChanged?.Invoke();
    }
    #region SaveLoad
    public void SetActiveOrderCount(int count)
    {
        activeOrderCount = count;
    }
    public OrderData[] GetOrdersForSerialization()
    {
        return orders;
    }
    private void MarkDirty()
    {
        orderSaveData.MarkDirty();
    }
    #endregion
    #region 테이블 세팅
    private void OnLevelChanged(int level)
    {
        CheckOrderRuleChangeAsync(level).Forget();
    }
    private async UniTask LoadOrderRuleForLevelAsync(int playerLevel)
    {
        int ruleLevel = OrderRuleLevelDataBase.Instance.GetRuleLevel(playerLevel);

        await LoadRuleByRuleLevelAsync(ruleLevel);
    }
    private async UniTask CheckOrderRuleChangeAsync(int playerLevel)
    {
        int ruleLevel = OrderRuleLevelDataBase.Instance.GetRuleLevel(playerLevel);

        if (ruleLevel == currentRuleLevel)
        {
            return;
        }

        await LoadRuleByRuleLevelAsync(ruleLevel);
    }
    private async UniTask LoadRuleByRuleLevelAsync(int ruleLevel)
    {
        string key = $"Level{ruleLevel}OrderRuleSO";

        AsyncOperationHandle<OrderRuleSO> handle = Addressables.LoadAssetAsync<OrderRuleSO>(key);

        await handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"OrderRuleSO Load Failed : {key}");
            return;
        }

        ReleaseCurrentRule();

        currentRuleHandle = handle;
        orderRuleSO = handle.Result;
        currentRuleLevel = ruleLevel;

        RefreshOrderCount();

        Debug.Log($"OrderRule Changed : {key}");
    }
    private void ReleaseCurrentRule()
    {
        if (currentRuleHandle.IsValid())
        {
            Addressables.Release(currentRuleHandle);
        }

        orderRuleSO = null;
    }
    #endregion
}
