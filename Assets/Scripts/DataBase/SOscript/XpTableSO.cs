using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "XpTableSO", menuName = "Scriptable Objects/XpTableSO")]
public class XpTableSO : ScriptableObject
{
    public List<int> xpTable;
    public int GetRequiredXp(int level)
    {
        if (level <= 0)
        {
            return 1;
        }

        if (level > xpTable.Count)
        {
            return xpTable[^1];
        }

        return xpTable[level - 1];
    }
}
