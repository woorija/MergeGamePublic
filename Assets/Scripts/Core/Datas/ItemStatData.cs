[System.Serializable]
public class ItemStatData : CategoryLevelData
{
    public ItemType itemType;

    /*
     * triggerTime
     * bubble,ice타입: 제거시간
     * factory: 스폰횟수 회복시간
     */
    public long triggerTime;

    //factory 전용변수
    public int maxSpawnCount;
    public ItemStatData()
    {
        category = ItemCategory.None;
        level = 0;
        itemType = ItemType.None;
        triggerTime = 0;
        maxSpawnCount = 0;
    }
    public ItemStatData(ItemStatData _data)
    {
        category = _data.category;
        level = _data.level;
        itemType = _data.itemType;
        triggerTime = _data.triggerTime;
        maxSpawnCount = _data.maxSpawnCount;
    }
}
