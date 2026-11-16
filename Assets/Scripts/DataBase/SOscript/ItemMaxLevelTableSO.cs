using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemMaxLevelTableSO", menuName = "Scriptable Objects/ItemMaxLevelTableSO")]
public class ItemMaxLevelTableSO : ScriptableObject
{
    public List<ItemMaxLevelData> list;

#if UNITY_EDITOR
    public int GetMaxLevel(ItemCategory category)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].category == category)
            {
                return list[i].maxLevel;
            }
        }

        return 0;
    }
#endif
}
