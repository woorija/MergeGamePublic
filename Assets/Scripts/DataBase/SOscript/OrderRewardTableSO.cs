using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderRewardTableSO", menuName = "Scriptable Objects/OrderRewardTableSO")]
public class OrderRewardTableSO : ScriptableObject
{
    public List<OrderRewardInfo> list;
}
