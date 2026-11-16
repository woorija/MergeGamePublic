using System.Collections.Generic;

public class InBoxSaveData : BaseSaveData
{
    InBoxManager manager;
    public InBoxSaveData(string path, int bufferSize, InBoxManager manager) : base(path, bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }

    protected override void Serialize()
    {
        Save(dataVersion);

        List<(ItemCategory category, int level)> gifts = manager.GetGiftsForSerialization();

        Save(gifts.Count);

        for (int i = 0; i < gifts.Count; i++)
        {
            WriteInBoxData(gifts[i]);
        }
    }

    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            List<(ItemCategory category, int level)> gifts = manager.GetGiftsForSerialization();
            int count = LoadInt();

            gifts.Clear();
            if (gifts.Capacity < count)
            {
                gifts.Capacity = count;
            }

            for (int i = 0; i < count; i++)
            {
                gifts.Add(ReadInBoxData());
            }
        }

        manager.RestoreInBox();
    }

    private void WriteInBoxData((ItemCategory category, int level) inBoxData)
    {
        Save((int)inBoxData.category);
        Save(inBoxData.level);
    }

    private (ItemCategory category, int level) ReadInBoxData()
    {
        ItemCategory category = (ItemCategory)LoadInt();
        int level = LoadInt();

        return (category, level);
    }
}
