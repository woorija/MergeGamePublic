public class OrderSaveData : BaseSaveData
{
    private OrderManager manager;

    public OrderSaveData(string path, int bufferSize, OrderManager manager) : base(path, bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }

    protected override void Serialize()
    {
        Save(dataVersion);
        Save(manager.ActiveOrderCount);

        OrderData[] orders = manager.GetOrdersForSerialization();

        for (int i = 0; i < manager.ActiveOrderCount; i++)
        {
            WriteOrder(orders[i]);
        }
    }

    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            int activeOrderCount = LoadInt();
            manager.SetActiveOrderCount(activeOrderCount);

            OrderData[] orders = manager.GetOrdersForSerialization();

            for (int i = 0; i < activeOrderCount; i++)
            {
                ReadOrder(orders[i]);
            }

            manager.RestoreOrderState();
        }
    }

    private void WriteOrder(OrderData order)
    {
        Save(order.npcId);
        Save(order.requirementCount);

        for (int i = 0; i < order.requirementCount; i++)
        {
            Save((int)order.requirements[i].category);
            Save(order.requirements[i].level);
        }
    }

    private void ReadOrder(OrderData order)
    {
        int npcId = LoadInt();
        order.npcId = npcId;

        int requirementCount = LoadInt();
        order.requirementCount = requirementCount;

        for (int i = 0; i < order.requirementCount; i++)
        {
            order.requirements[i] = new OrderRequirement
            {
                category = (ItemCategory)LoadInt(),
                level = LoadInt()
            };
        }
    }
}
