using System.Collections.Generic;
using UnityEngine;

public class RewardDataBase : MonoBehaviour
{
    public static Dictionary<(ItemCategory, int), int> rewardTable { get; private set; } = new Dictionary<(ItemCategory, int), int>();

    public void Init(RewardTableSO _tableSO)
    {
        rewardTable.Clear();

        int count = _tableSO.list.Count;

        for (int i = 0; i < count; i++)
        {
            RewardInfo sourceData = _tableSO.list[i];

            if (sourceData == null)
            {
                continue;
            }

            (ItemCategory, int) key = (sourceData.category, sourceData.level);

            if (rewardTable.ContainsKey(key))
            {
                Debug.LogWarning(
                    $"Duplicate RewardData detected : {sourceData.category} / Lv.{sourceData.level}"
                );

                continue;
            }

            rewardTable.Add(key, sourceData.value);
        }
    }
}
