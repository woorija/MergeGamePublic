using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemNameDataBase : MonoBehaviour
{
    public static Dictionary<(ItemCategory, int), string> ItemNameTable { get; private set; } = new Dictionary<(ItemCategory, int), string>();

    public void Init(ItemNameTableSO _tableSO)
    {
        ItemNameTable.Clear();

        int count = _tableSO.list.Count;

        for (int i = 0; i < count; i++)
        {
            ItemNameInfo sourceData = _tableSO.list[i];

            if (sourceData == null)
            {
                continue;
            }

            (ItemCategory, int) key = (sourceData.category, sourceData.level);

            if (ItemNameTable.ContainsKey(key))
            {
                Debug.LogWarning(
                    $"Duplicate ItemNameInfo detected : {sourceData.category} / Lv.{sourceData.level}"
                );

                continue;
            }

            ItemNameTable.Add(key, sourceData.itemName);
        }
    }
}
