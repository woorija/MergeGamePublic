using System.Collections.Generic;
using UnityEngine;

public class BubblePriceDataBase : MonoBehaviour
{
    public static Dictionary<(ItemCategory, int), int> BubblePriceTable { get; private set; } = new Dictionary<(ItemCategory, int), int>();

    public void Init(BubblePriceTableSO _tableSO)
    {
        BubblePriceTable.Clear();

        int count = _tableSO.list.Count;

        for (int i = 0; i < count; i++)
        {
            BubblePriceData sourceData = _tableSO.list[i];

            if(sourceData == null)
            {
                continue;
            }

            (ItemCategory, int) key = (sourceData.category, sourceData.level);

            if (BubblePriceTable.ContainsKey(key))
            {
                Debug.LogWarning(
                    $"Duplicate BubblePriceData detected : {sourceData.category} / Lv.{sourceData.level}"
                );

                continue;
            }

            int price = sourceData.price;

            BubblePriceTable.Add(key, price);
        }
    }
}
