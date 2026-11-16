using System.Collections.Generic;
using UnityEngine;

public class FactoryRuntimeData
{
    public bool useStamina;
    public bool autoRecoverSpawnCount;
    public Dictionary<int, FactoryLevelRuntimeData> factoryLevelTable;

    public bool IsSpawnable(ItemData itemData)
    {
        if (itemData == null)
        {
            return false;
        }
        return factoryLevelTable.ContainsKey(itemData.level);
    }
    public ItemStatData GetSpawnItem(ItemData itemData)
    {
        FactoryLevelRuntimeData levelData = factoryLevelTable[itemData.level];

        int randomValue = Random.Range(0, levelData.totalWeight);
        int cumulativeWeight = 0;
        List<SpawnInfo> spawnDatas = levelData.spawnDatas;

        for (int i = 0; i < spawnDatas.Count; i++)
        {
            SpawnInfo spawnInfo = spawnDatas[i];

            cumulativeWeight += spawnInfo.weight;

            if (randomValue < cumulativeWeight)
            {
                return ItemDataBase.GetItemStatData(spawnInfo.category, spawnInfo.level);
            }
        }

        Debug.LogError($"Factory spawn failed : {itemData.category} Lv.{itemData.level}");

        return null;
    }
}
