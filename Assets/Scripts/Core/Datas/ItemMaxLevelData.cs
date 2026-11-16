using Unity.Collections;

[System.Serializable]
public class ItemMaxLevelData
{
    [ReadOnly] public ItemCategory category;
    public int maxLevel;
}
