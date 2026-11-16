using System.Collections.Generic;
using UnityEngine;

public class SpecialItemDataBase : MonoBehaviour
{
    private static Dictionary<(ItemType, ItemCategory, int), ItemStatData> SpecialItemTable;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData()
    {
        SpecialItemTable = new Dictionary<(ItemType, ItemCategory, int), ItemStatData>();
    }
    public void Init(SpecialItemStatTableSO _tableSO)
    {
        SpecialItemTable.Clear();

        int count = _tableSO.list.Count;

        for (int i = 0; i < count; i++)
        {
            ItemStatData sourceData = _tableSO.list[i];

            if (sourceData == null)
            {
                continue;
            }

            (ItemType, ItemCategory, int) key = (sourceData.itemType, sourceData.category, sourceData.level);

            if (SpecialItemTable.ContainsKey(key))
            {
                Debug.LogWarning(
                    $"Duplicate ItemStatData detected : {sourceData.category} / Lv.{sourceData.level}"
                );

                continue;
            }

            // 깊은 복사
            ItemStatData copiedData = new ItemStatData(sourceData);

            SpecialItemTable.Add(key, copiedData);
        }
    }

    public static ItemStatData GetItemStatData(ItemType type, ItemCategory category, int level)
    {
        return SpecialItemTable[(type, category, level)];
    }
}
