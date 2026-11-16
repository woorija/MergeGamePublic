using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactorySpawnDataSO", menuName = "ScriptableObjects/FactorySpawnDataSO")]
public class FactorySpawnDataSO : ScriptableObject
{
    public ItemCategory factoryCategory;
    public bool useStamina;
    public bool autoRecoverSpawnCount;
    public List<FactoryLevelData> factoryLevelDatas;
}
