using System.Collections.Generic;
using UnityEngine;

public class OrderRewardDatabase : MonoBehaviour
{
    public static Dictionary<(ItemCategory, int), RewardWeight> orderRewardTable { get; private set; } = new Dictionary<(ItemCategory, int), RewardWeight>();

    public void Init(OrderRewardTableSO _tableSO)
    {
        orderRewardTable.Clear();

        int count = _tableSO.list.Count;

        for (int i = 0; i < count; i++)
        {
            OrderRewardInfo sourceData = _tableSO.list[i];

            if (sourceData == null)
            {
                continue;
            }

            (ItemCategory, int) key = (sourceData.category, sourceData.level);

            if (orderRewardTable.ContainsKey(key))
            {
                Debug.LogWarning(
                    $"Duplicate OrderRewardData detected : {sourceData.category} / Lv.{sourceData.level}"
                );

                continue;
            }

            orderRewardTable.Add(key, sourceData.rewardWeight);
        }
    }
}
