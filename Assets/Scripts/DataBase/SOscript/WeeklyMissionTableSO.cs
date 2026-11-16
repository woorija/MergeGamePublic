using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeeklyMissionTableSO", menuName = "Scriptable Objects/WeeklyMissionTableSO")]
public class WeeklyMissionTableSO : ScriptableObject
{
    public List<WeeklyMissionScoreRewardData> scoreRewardTable;
    public List<WeeklyMissionData> table;
}
