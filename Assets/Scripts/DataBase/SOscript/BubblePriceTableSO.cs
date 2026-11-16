using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BubblePriceTableSO", menuName = "Scriptable Objects/BubblePriceTableSO")]
public class BubblePriceTableSO : ScriptableObject
{
    public List<BubblePriceData> list;
}
