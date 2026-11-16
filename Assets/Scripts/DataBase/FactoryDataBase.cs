using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FactoryDataBase : DontDestroySingletonBehaviour<FactoryDataBase>
{
    public Dictionary<ItemCategory, FactoryRuntimeData> factorySpawnTable {  get; private set; }

    public async UniTask LoadTableAsync()
    {
        var handle = Addressables.LoadAssetsAsync<FactorySpawnDataSO>("FactoryData", null);
        IList<FactorySpawnDataSO> soList = await handle.ToUniTask();

        factorySpawnTable = BakeTable(soList);

        Addressables.Release(handle);
    }

    private Dictionary<ItemCategory, FactoryRuntimeData> BakeTable(IList<FactorySpawnDataSO> _soList)
    {
        var table = new Dictionary<ItemCategory, FactoryRuntimeData>();

        foreach (var so in _soList)
        {
            var levelTable = new Dictionary<int, FactoryLevelRuntimeData>();
            bool useStamina = so.useStamina;
            bool autoRecoverSpawnCount = so.autoRecoverSpawnCount;
            int levelCount = so.factoryLevelDatas.Count;

            for (int i = 0; i < levelCount; i++)
            {
                var levelData = so.factoryLevelDatas[i];

                if (levelData.spawnDatas == null || levelData.spawnDatas.Count == 0)
                {
                    continue;
                }

                int totalWeight = 0;
                var copiedSpawnDatas = new List<SpawnInfo>(levelData.spawnDatas.Count);

                foreach (var spawnData in levelData.spawnDatas)
                {
                    totalWeight += spawnData.weight;
                    copiedSpawnDatas.Add(new SpawnInfo(spawnData));
                }

                levelTable[i + 1] = new FactoryLevelRuntimeData
                {
                    spawnDatas = copiedSpawnDatas,
                    totalWeight = totalWeight
                };
            }

            table[so.factoryCategory] = new FactoryRuntimeData
            {
                useStamina = useStamina,
                autoRecoverSpawnCount = autoRecoverSpawnCount,
                factoryLevelTable = levelTable
            };
        }

        return table;
    }
    public FactoryRuntimeData GetFactoryData(ItemCategory category)
    {
        if (!factorySpawnTable.TryGetValue(category, out FactoryRuntimeData data))
        {
            Debug.LogError($"Factory not found : {category}");
            return null;
        }

        return data;
    }
}
