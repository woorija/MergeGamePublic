using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelUpRewardTableSO))]
public class LevelUpRewardTableSOEditor : Editor
{
    private SerializedProperty tablesProperty;

    private void OnEnable()
    {
        tablesProperty = serializedObject.FindProperty("tables");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < tablesProperty.arraySize; i++)
        {
            DrawLevelReward(i);
        }

        EditorGUILayout.Space(10f);

        if (GUILayout.Button("Add Level"))
        {
            tablesProperty.arraySize++;
        }

        GUI.enabled = tablesProperty.arraySize > 1;

        if (GUILayout.Button("Remove Level"))
        {
            int lastIndex = tablesProperty.arraySize - 1;
            tablesProperty.DeleteArrayElementAtIndex(lastIndex);
        }

        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLevelReward(int index)
    {
        SerializedProperty tableProperty = tablesProperty.GetArrayElementAtIndex(index);
        SerializedProperty listProperty = tableProperty.FindPropertyRelative("list");

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField($"Level {index + 2}", EditorStyles.boldLabel);

        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty rewardProperty = listProperty.GetArrayElementAtIndex(i);

            SerializedProperty categoryProperty = rewardProperty.FindPropertyRelative("category");
            SerializedProperty levelProperty = rewardProperty.FindPropertyRelative("level");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(categoryProperty, GUIContent.none);
            EditorGUILayout.PropertyField(levelProperty, GUIContent.none, GUILayout.Width(50f));

            if (GUILayout.Button("X", GUILayout.Width(20f)))
            {
                listProperty.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Reward"))
        {
            listProperty.arraySize++;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(4f);
    }
}