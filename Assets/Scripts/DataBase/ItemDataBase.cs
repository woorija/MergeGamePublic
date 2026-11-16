using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    private static Dictionary<(ItemCategory, int), ItemStatData> ItemTable;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData()
    {
        ItemTable = new Dictionary<(ItemCategory, int), ItemStatData>();
    }
    public void Init(ItemStatTableSO _tableSO)
    {
        ItemTable.Clear();

        int count = _tableSO.list.Count;

        for (int i = 0; i < count; i++)
        {
            ItemStatData sourceData = _tableSO.list[i];

            if (sourceData == null)
            {
                continue;
            }

            (ItemCategory, int) key = (sourceData.category, sourceData.level);

            if (ItemTable.ContainsKey(key))
            {
                Debug.LogWarning(
                    $"Duplicate ItemStatData detected : {sourceData.category} / Lv.{sourceData.level}"
                );

                continue;
            }

            // 깊은 복사
            ItemStatData copiedData = new ItemStatData(sourceData);

            ItemTable.Add(key, copiedData);
        }
    }
    public static ItemStatData GetItemStatData(ItemCategory category, int level)
    {
        return ItemTable[(category, level)];
    }
}