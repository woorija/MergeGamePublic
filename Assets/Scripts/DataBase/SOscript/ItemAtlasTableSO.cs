using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemAtlasTableSO", menuName = "Scriptable Objects/ItemAtlasTableSO")]
public class ItemAtlasTableSO : ScriptableObject
{
    public List<ItemAtlasEntry> entries;
}
