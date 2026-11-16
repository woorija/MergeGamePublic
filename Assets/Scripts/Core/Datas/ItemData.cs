/// <summary>
/// 각 슬롯에 배치 될 아이템의 데이터에 대한 정보를 담은 클래스
/// </summary>
public class ItemData
{
    public ItemStatData statData {  get; private set; }

    public ItemType type => statData.itemType;
    public ItemCategory category => statData.category;
    public int level => statData.level;
    public bool isMaxSpawnCount => remainSpawnCount == statData.maxSpawnCount;

    //TimeManager의 DateTimeOffset.UtcNow.ToUnixTimeSeconds()사용
    public long triggerTimestamp { get; private set; }
    //factory 전용변수
    public int remainSpawnCount { get; private set; }

    public ItemData()
    {
        statData = ItemDataBase.GetItemStatData(ItemCategory.None, 0);
        triggerTimestamp = 0;
        remainSpawnCount = 0;
    }
    public ItemData(ItemData _data)
    {
        statData = _data.statData;
        triggerTimestamp = _data.triggerTimestamp;
        remainSpawnCount = _data.remainSpawnCount;
    }
    public void Clear()
    {
        statData = ItemDataBase.GetItemStatData(ItemCategory.None, 0); // DB에서 초기값으로 바꾸기
        remainSpawnCount = 0;
        triggerTimestamp = 0;
    }
    public void SetData(ItemCategory category, int level)
    {
        statData = ItemDataBase.GetItemStatData(category, level);
        remainSpawnCount = statData.maxSpawnCount;
        triggerTimestamp = 0;
    }
    public void SetData(ItemStatData statData, int triggerTimestamp = 0)
    {
        this.statData = statData;
        remainSpawnCount = statData.maxSpawnCount;
        this.triggerTimestamp = triggerTimestamp;
    }

    public void CopyFrom(ItemData itemData)
    {
        statData = itemData.statData;
        remainSpawnCount = itemData.remainSpawnCount;
        triggerTimestamp = itemData.triggerTimestamp;
    }
    #region 트리거체크 함수
    public bool IsMaxSpawnCount()
    {
        return remainSpawnCount == statData.maxSpawnCount;
    }
    public bool HasSpawnCount()
    {
        return remainSpawnCount > 0;
    }
    public void RecoverSpawnCount()
    {
        remainSpawnCount = statData.maxSpawnCount;
    }
    public void UseSpawnCount(int count = 1)
    {
        remainSpawnCount -= count;
    }
    public void SetTriggerTimestamp(long timestamp)
    {
        triggerTimestamp = timestamp;
    }
    public void ClearTriggerTimestamp()
    {
        triggerTimestamp = 0;
    }
    #endregion
    #region SaveLoad
    public void LoadData(ItemCategory category, int level)
    {
        statData = ItemDataBase.GetItemStatData(category, level);
        remainSpawnCount = statData.maxSpawnCount;
        triggerTimestamp = 0;
    }
    public void LoadData(ItemCategory category, int level, long triggerTimestamp, int remainSpawnCount)
    {
        statData = ItemDataBase.GetItemStatData(category, level);
        this.triggerTimestamp = triggerTimestamp;
        this.remainSpawnCount = remainSpawnCount;
    }
    public void LoadData(ItemType type, ItemCategory category, int level, long triggerTimestamp, int remainSpawnCount)
    {
        switch (type)
        {
            case ItemType.Bubble:
            case ItemType.Ice:
                statData = SpecialItemDataBase.GetItemStatData(type, category, level);
                break;
            default:
                statData = ItemDataBase.GetItemStatData(category, level);
                break;
        }
        this.triggerTimestamp = triggerTimestamp;
        this.remainSpawnCount = remainSpawnCount;
    }
    #endregion
}
