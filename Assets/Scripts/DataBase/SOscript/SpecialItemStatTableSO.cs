using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialItemStatTableSO", menuName = "Scriptable Objects/SpecialItemStatTableSO")]
public class SpecialItemStatTableSO : ScriptableObject
{
    public List<ItemStatData> list = new();
}
