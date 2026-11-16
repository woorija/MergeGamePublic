using System.Collections.Generic;
using UnityEngine;

public class LevelUpRewardDataBase : SingletonBehaviour<LevelUpRewardDataBase>
{
    LevelUpRewardTableSO tableSO;
    public IReadOnlyList<ItemRewardData> GetRewards(int level)
    {
        int index = Mathf.Clamp(level - 2, 0, tableSO.tables.Length - 1);
        return tableSO.tables[index].list;
    }
    public void Init(LevelUpRewardTableSO tableSO)
    {
        this.tableSO = tableSO;
    }
}
