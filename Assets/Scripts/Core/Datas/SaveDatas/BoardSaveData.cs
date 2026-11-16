public class BoardSaveData : BaseSaveData
{
    BoardManager manager;
    const int slotCount = 80;
    public BoardSaveData(string _path, int _bufferSize, BoardManager manager) : base(_path, _bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }
    protected override void Serialize()
    {
        Save(dataVersion);

        BoardState[] boardStates = manager.GetBoardStatesForSerialization();

        for (int i = 0; i < slotCount; i++)
        {
            WriteBoardState(boardStates[i]);
        }

        ItemData[] itemDatas = manager.GetItemDatasForSerialization();

        for (int i = 0; i < slotCount; i++)
        {
            WriteItemData(itemDatas[i]);
        }
    }

    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            BoardState[] boardStates = manager.GetBoardStatesForSerialization();

            for (int i = 0; i < slotCount; i++)
            {
                boardStates[i] = ReadBoardState();
            }

            ItemData[] itemDatas = manager.GetItemDatasForSerialization();

            for (int i = 0; i < slotCount; i++)
            {
                ReadItemData(itemDatas[i]);
            }
        }

        manager.RestoreBoardState();
    }

    private void WriteBoardState(BoardState boardState)
    {
        Save((int)boardState);
    }

    private BoardState ReadBoardState()
    {
        return (BoardState)LoadInt();
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
        else if (itemData.type == ItemType.Bubble || itemData.type == ItemType.Ice)
        {
            flag = ItemSaveFlag.WithSpecialType;
        }

        Save((int)flag);

        if (flag == ItemSaveFlag.WithSpawnCount)
        {
            Save(itemData.remainSpawnCount);
            Save(itemData.triggerTimestamp);
        }

        else if (flag == ItemSaveFlag.WithSpecialType)
        {
            Save((int)itemData.type);
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
                {
                    int remainSpawnCount = LoadInt();
                    long timestamp = LoadLong();
                    itemData.LoadData(category, level, timestamp, remainSpawnCount);
                }
                break;
            case ItemSaveFlag.WithSpecialType:
                {
                    ItemType itemType = (ItemType)LoadInt();
                    long timestamp = LoadLong();
                    itemData.LoadData(itemType, category, level, timestamp, 0);
                }
                break;
            default:
                itemData.LoadData(category, level);
                break;
        }
    }
}
