public class ShopSaveData : BaseSaveData
{
    private ShopManager manager;

    public ShopSaveData(string path, int bufferSize, ShopManager manager) : base(path, bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }

    protected override void Serialize()
    {
        Save(dataVersion);

        int[] itemIndices = manager.GetItemIndicesForSerialization();
        bool[] purchasedItems = manager.GetPurchasedItemsForSerialization();

        Save(itemIndices);
        Save(purchasedItems);
        Save(manager.RefreshCount);
        Save(manager.hasClaimedDailyGem);
    }

    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            int[] itemIndices = manager.GetItemIndicesForSerialization();
            LoadInto(itemIndices);

            bool[] purchasedItems = manager.GetPurchasedItemsForSerialization();
            LoadInto(purchasedItems);

            int refreshCount = LoadInt();
            manager.SetRefreshCount(refreshCount);

            bool hasClaimedDailyGem = LoadBool();
            manager.SetClaimedDailyGem(hasClaimedDailyGem);
        }
    }
}
