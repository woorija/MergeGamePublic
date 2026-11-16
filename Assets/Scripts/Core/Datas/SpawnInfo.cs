[System.Serializable]
public class SpawnInfo
{
    public ItemCategory category;
    public int level;
    public int weight;
    public SpawnInfo()
    {

    }
    public SpawnInfo(SpawnInfo _other)
    {
        category = _other.category;
        level = _other.level;
        weight = _other.weight;
    }
}
