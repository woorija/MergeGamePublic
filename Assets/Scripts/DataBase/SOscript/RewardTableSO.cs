using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardTableSO", menuName = "Scriptable Objects/RewardTableSO")]
public class RewardTableSO : ScriptableObject
{
    public List<RewardInfo> list;
}
