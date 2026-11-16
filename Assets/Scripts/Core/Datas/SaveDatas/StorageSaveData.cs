public class StorageSaveData : BaseSaveData
{
    private StorageManager manager;

    public StorageSaveData(string path, int bufferSize, StorageManager manager) : base(path, bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }

    protected override void Serialize()
    {
        Save(dataVersion);

        int capacity = manager.capacity;
        ItemData[] itemDatas = manager.GetItemDatasForSerialization();
        Save(capacity);

        for (int i = 0; i < capacity; i++)
        {
            WriteItemData(itemDatas[i]);
        }
    }

    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            int capacity = LoadInt();
            manager.SetCapacity(capacity);

            ItemData[] itemDatas = manager.GetItemDatasForSerialization();

            for (int i = 0; i < capacity; i++)
            {
                ReadItemData(itemDatas[i]);
            }

            manager.RestoreStorageState();
        }
    }

    private void WriteItemData(ItemData itemData)
    {
        Save((int)itemData.category);
        Save(itemData.level);

        ItemSaveFlag flag = ItemSaveFlag.Basic;

        if (itemData.type == ItemType.Factory && !itemData.isMaxSpawnCount)
        {
            flag = ItemSaveFlag.WithSpawnCount;
        }

        Save((int)flag);

        if (flag == ItemSaveFlag.WithSpawnCount)
        {
            Save(itemData.remainSpawnCount);
            Save(itemData.triggerTimestamp);
        }
    }

    private void ReadItemData(ItemData itemData)
    {
        ItemCategory category = (ItemCategory)LoadInt();
        int level = LoadInt();

        ItemSaveFlag flag = (ItemSaveFlag)LoadInt();

        switch (flag)
        {
            case ItemSaveFlag.WithSpawnCount:
                int remainSpawnCount = LoadInt();
                long timestamp = LoadLong();
                itemData.LoadData(category, level, timestamp, remainSpawnCount);
                break;
            default:
                itemData.LoadData(category, level);
                break;
        }
    }
}
