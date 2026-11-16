using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemNameTableSO", menuName = "Scriptable Objects/ItemNameTableSO")]
public class ItemNameTableSO : ScriptableObject
{
    public List<ItemNameInfo> list;
}
