using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStatTableSO", menuName = "Scriptable Objects/ItemStatTableSO")]
public class ItemStatTableSO : ScriptableObject
{
    public List<ItemStatData> list;
}
