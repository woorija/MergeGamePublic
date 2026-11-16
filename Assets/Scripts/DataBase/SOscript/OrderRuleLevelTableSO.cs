using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderRuleLevelTableSO", menuName = "Scriptable Objects/OrderRuleLevelTableSO")]
public class OrderRuleLevelTableSO : ScriptableObject
{
    public List<int> levels;
}
