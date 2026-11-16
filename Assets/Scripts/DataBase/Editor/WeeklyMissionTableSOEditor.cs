using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeeklyMissionTableSO))]
public class WeeklyMissionTableSOEditor : Editor
{
    private const int MissionCount = 35;
    private const int MissionsPerDay = 5;
    private const int ScoreRewardCount = 5;

    private static readonly string[] DayNames =
    {
        "월",
        "화",
        "수",
        "목",
        "금",
        "토",
        "일"
    };

    private bool scoreRewardFoldout;
    private readonly bool[] dayFoldouts = new bool[7];

    private SerializedProperty tableProperty;
    private SerializedProperty scoreRewardTableProperty;

    private void OnEnable()
    {
        tableProperty = serializedObject.FindProperty("table");

        scoreRewardTableProperty =
            serializedObject.FindProperty("scoreRewardTable");

        EnsureMissionCount();
        EnsureScoreRewardCount();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnsureMissionCount();
        EnsureScoreRewardCount();

        EditorGUILayout.Space();

        DrawTableHeader();

        EditorGUILayout.Space();

        DrawScoreRewards();

        EditorGUILayout.Space(8);

        for (int day = 0; day < 7; day++)
        {
            DrawDay(day);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTableHeader()
    {
        EditorGUILayout.LabelField(
            "Weekly Mission Table",
            EditorStyles.boldLabel);

        EditorGUILayout.LabelField(
            $"총 미션 수 : {MissionCount}");

        EditorGUILayout.LabelField(
            $"총 점수 : {GetTotalScore()}");
    }

    private void DrawScoreRewards()
    {
        scoreRewardFoldout = EditorGUILayout.Foldout(
            scoreRewardFoldout,
            "점수별 보상",
            true);

        if (!scoreRewardFoldout)
        {
            return;
        }

        EditorGUI.indentLevel++;

        for (int i = 0; i < ScoreRewardCount; i++)
        {
            DrawScoreReward(i);
        }

        EditorGUI.indentLevel--;
    }

    private void DrawScoreReward(int index)
    {
        SerializedProperty scoreRewardProperty =
            scoreRewardTableProperty.GetArrayElementAtIndex(index);

        SerializedProperty requiredScoreProperty =
            scoreRewardProperty.FindPropertyRelative("requiredScore");

        SerializedProperty rewardProperty =
            scoreRewardProperty.FindPropertyRelative("reward");

        SerializedProperty rewardTypeProperty =
            rewardProperty.FindPropertyRelative("rewardType");

        SerializedProperty amountProperty =
            rewardProperty.FindPropertyRelative("amount");

        SerializedProperty itemProperty =
            rewardProperty.FindPropertyRelative("item");

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField(
            $"보상 #{index + 1}",
            EditorStyles.boldLabel);

        // Required Score
        EditorGUILayout.PropertyField(
            requiredScoreProperty,
            new GUIContent("요구 점수"));

        // Reward Type
        EditorGUILayout.PropertyField(
            rewardTypeProperty,
            new GUIContent("보상 종류"));

        // Reward Value
        DrawRewardValue(
            rewardTypeProperty,
            amountProperty,
            itemProperty);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(4);
    }

    private void DrawDay(int day)
    {
        int startIndex = day * MissionsPerDay;

        dayFoldouts[day] = EditorGUILayout.Foldout(
            dayFoldouts[day],
            DayNames[day],
            true);

        if (!dayFoldouts[day])
        {
            return;
        }

        EditorGUI.indentLevel++;

        for (int i = 0; i < MissionsPerDay; i++)
        {
            int missionIndex = startIndex + i;

            DrawMission(missionIndex);
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.Space(8);
    }

    private void DrawMission(int index)
    {
        SerializedProperty missionProperty =
            tableProperty.GetArrayElementAtIndex(index);

        SerializedProperty missionTypeProperty =
            missionProperty.FindPropertyRelative("missionType");

        SerializedProperty targetCountProperty =
            missionProperty.FindPropertyRelative("targetCount");

        SerializedProperty scoreProperty =
            missionProperty.FindPropertyRelative("score");

        SerializedProperty rewardProperty =
            missionProperty.FindPropertyRelative("reward");

        SerializedProperty rewardTypeProperty =
            rewardProperty.FindPropertyRelative("rewardType");

        SerializedProperty amountProperty =
            rewardProperty.FindPropertyRelative("amount");

        SerializedProperty itemProperty =
            rewardProperty.FindPropertyRelative("item");

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // #Index + MissionType
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            $"#{index}",
            GUILayout.Width(35));

        EditorGUILayout.PropertyField(
            missionTypeProperty,
            GUIContent.none);

        EditorGUILayout.EndHorizontal();

        // Count
        EditorGUILayout.PropertyField(
            targetCountProperty,
            new GUIContent("카운트"));

        // Score
        EditorGUILayout.PropertyField(
            scoreProperty,
            new GUIContent("스코어"));

        // Reward Type
        EditorGUILayout.PropertyField(
            rewardTypeProperty,
            new GUIContent("리워드"));

        // Reward Value
        DrawRewardValue(
            rewardTypeProperty,
            amountProperty,
            itemProperty);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(4);
    }

    private void DrawRewardValue(
        SerializedProperty rewardTypeProperty,
        SerializedProperty amountProperty,
        SerializedProperty itemProperty)
    {
        MissionRewardType rewardType =
            (MissionRewardType)rewardTypeProperty.enumValueIndex;

        if (rewardType == MissionRewardType.BoardItem)
        {
            DrawItemReward(itemProperty);
        }
        else
        {
            EditorGUILayout.PropertyField(
                amountProperty,
                new GUIContent("보상값"));
        }
    }

    private void DrawItemReward(SerializedProperty itemProperty)
    {
        SerializedProperty categoryProperty =
            itemProperty.FindPropertyRelative("category");

        SerializedProperty levelProperty =
            itemProperty.FindPropertyRelative("level");

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "아이템",
            GUILayout.Width(EditorGUIUtility.labelWidth));

        EditorGUILayout.PropertyField(
            categoryProperty,
            GUIContent.none);

        EditorGUILayout.PropertyField(
            levelProperty,
            GUIContent.none);

        EditorGUILayout.EndHorizontal();
    }

    private int GetTotalScore()
    {
        if (tableProperty == null)
        {
            return 0;
        }

        int totalScore = 0;

        for (int i = 0; i < tableProperty.arraySize; i++)
        {
            SerializedProperty missionProperty =
                tableProperty.GetArrayElementAtIndex(i);

            SerializedProperty scoreProperty =
                missionProperty.FindPropertyRelative("score");

            totalScore += scoreProperty.intValue;
        }

        return totalScore;
    }

    private void EnsureMissionCount()
    {
        if (tableProperty == null)
        {
            return;
        }

        if (tableProperty.arraySize == MissionCount)
        {
            return;
        }

        tableProperty.arraySize = MissionCount;

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }

    private void EnsureScoreRewardCount()
    {
        if (scoreRewardTableProperty == null)
        {
            return;
        }

        if (scoreRewardTableProperty.arraySize == ScoreRewardCount)
        {
            return;
        }

        scoreRewardTableProperty.arraySize = ScoreRewardCount;

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}