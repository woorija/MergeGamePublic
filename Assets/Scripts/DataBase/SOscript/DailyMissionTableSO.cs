using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyMissionTableSO", menuName = "Scriptable Objects/DailyMissionTableSO")]
public class DailyMissionTableSO : ScriptableObject
{
    public List<DailyMissionTemplate> templates;
}
