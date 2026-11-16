using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderRuleSO", menuName = "Scriptable Objects/OrderRuleSO")]
public class OrderRuleSO : ScriptableObject
{
    public int maxOrderCount;
    public int maxRequirementCount;
    public int itemLevelLimit;
    public List<OrderItemData> orderItems;
}
