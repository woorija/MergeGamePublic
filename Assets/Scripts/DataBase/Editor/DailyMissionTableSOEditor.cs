using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// DailyMissionTableSO를 위한 커스텀 에디터.
/// 템플릿별로 [미션타입 / 카운트] + [Xp / 골드 / 젬 / 스태미너] 값을 한눈에 보고 편집할 수 있게 표시한다.
/// </summary>
[CustomEditor(typeof(DailyMissionTableSO))]
public class DailyMissionTableSOEditor : Editor
{
    private DailyMissionTableSO table;

    private void OnEnable()
    {
        table = (DailyMissionTableSO)target;
        if (table.templates == null)
            table.templates = new List<DailyMissionTemplate>();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Daily Mission Templates", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"총 {table.templates.Count}개", EditorStyles.miniLabel);
        EditorGUILayout.Space(6);

        int removeIndex = -1;

        for (int i = 0; i < table.templates.Count; i++)
        {
            if (DrawTemplate(table.templates[i], i))
                removeIndex = i;

            EditorGUILayout.Space(4);
        }

        if (removeIndex >= 0)
        {
            Undo.RecordObject(table, "Remove Daily Mission Template");
            table.templates.RemoveAt(removeIndex);
            EditorUtility.SetDirty(table);
        }

        EditorGUILayout.Space(8);
        if (GUILayout.Button("+ 템플릿 추가"))
        {
            Undo.RecordObject(table, "Add Daily Mission Template");
            table.templates.Add(new DailyMissionTemplate
            {
                rewardPool = new List<MissionRewardData>()
            });
            EditorUtility.SetDirty(table);
        }
    }

    /// <returns>true면 이 템플릿 삭제 요청됨</returns>
    private bool DrawTemplate(DailyMissionTemplate t, int index)
    {
        bool deleteRequested = false;

        EditorGUILayout.BeginVertical("box");

        // ── 헤더: 인덱스 / 미션타입 / 난이도 / 삭제버튼 ──
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"#{index}", GUILayout.Width(28));

        EditorGUI.BeginChangeCheck();
        var newType = (MissionType)EditorGUILayout.EnumPopup(t.missionType, GUILayout.Width(130));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(table, "Edit Mission Type/Difficulty");
            t.missionType = newType;
            EditorUtility.SetDirty(table);
        }

        GUILayout.FlexibleSpace();

        var prevColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(1f, 0.55f, 0.55f);
        if (GUILayout.Button("삭제", GUILayout.Width(50)))
            deleteRequested = true;
        GUI.backgroundColor = prevColor;

        EditorGUILayout.EndHorizontal();

        // ── 카운트 ──
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("카운트", GUILayout.Width(70));
        EditorGUI.BeginChangeCheck();
        int newCount = EditorGUILayout.IntField(t.targetCount, GUILayout.Width(80));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(table, "Edit Target Count");
            t.targetCount = Mathf.Max(0, newCount);
            EditorUtility.SetDirty(table);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(2);

        // ── 보상풀 (Xp / 골드 / 젬 / 스태미너) ──
        if (t.rewardPool == null)
            t.rewardPool = new List<MissionRewardData>();

        DrawRewardField(t.rewardPool, MissionRewardType.Xp, "Xp");
        DrawRewardField(t.rewardPool, MissionRewardType.Gold, "골드");
        DrawRewardField(t.rewardPool, MissionRewardType.Gem, "젬");
        DrawRewardField(t.rewardPool, MissionRewardType.Stamina, "스태미너");

        EditorGUILayout.EndVertical();

        return deleteRequested;
    }

    private void DrawRewardField(List<MissionRewardData> pool, MissionRewardType type, string label)
    {
        int idx = pool.FindIndex(r => r.rewardType == type);
        int amount = idx >= 0 ? pool[idx].amount : 0;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(70));

        EditorGUI.BeginChangeCheck();
        int newAmount = Mathf.Max(0, EditorGUILayout.IntField(amount, GUILayout.Width(80)));
        if (EditorGUI.EndChangeCheck() && newAmount != amount)
        {
            Undo.RecordObject(table, $"Edit {label} Reward");

            if (idx >= 0)
            {
                if (newAmount == 0)
                {
                    // 0이면 보상풀에서 제거 (해당 타입은 보상 후보에서 빠짐)
                    pool.RemoveAt(idx);
                }
                else
                {
                    var data = pool[idx];
                    data.amount = newAmount;
                    pool[idx] = data;
                }
            }
            else if (newAmount > 0)
            {
                pool.Add(new MissionRewardData { rewardType = type, amount = newAmount });
            }

            EditorUtility.SetDirty(table);
        }

        EditorGUILayout.EndHorizontal();
    }
}