using System.Collections.Generic;
using UnityEngine;

public class ItemMaxLevelDataBase : MonoBehaviour
{
    public static Dictionary<ItemCategory, int> ItemMaxLevelTable {  get; private set; } = new Dictionary<ItemCategory, int>();

    public void Init(ItemMaxLevelTableSO _tableSO)
    {
        ItemMaxLevelTable.Clear();

        int count = _tableSO.list.Count;

        for (int i = 0; i < count; i++)
        {
            ItemMaxLevelData sourceData = _tableSO.list[i];

            if (sourceData == null)
            {
                continue;
            }
            ItemCategory key = sourceData.category;
            if (ItemMaxLevelTable.ContainsKey(key))
            {
                Debug.LogWarning(
                    $"Duplicate ItemMaxLevelData detected : {sourceData.category}"
                );

                continue;
            }

            ItemMaxLevelTable.Add(key, sourceData.maxLevel);
        }
    }
    public static int GetMaxLevel(ItemCategory category)
    {
        if (!ItemMaxLevelTable.TryGetValue(category, out int maxLevel))
        {
            return 1;
        }
        return maxLevel;
    }
}
