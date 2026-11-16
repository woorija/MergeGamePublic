#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RewardTableSO))]

public class RewardTableSOEditor : TableEditorBase
{
    private static readonly ItemCategory[] RewardCategories =
    {
        ItemCategory.Gold,
        ItemCategory.Gem,
        ItemCategory.Stamina
    };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawSyncButton();
        EditorGUILayout.Space(5);

        DrawTableHeader();
        DrawTableBody();

        serializedObject.ApplyModifiedProperties();
    }

    protected override void DrawSyncButton()
    {
        if (GUILayout.Button("Sync Table"))
        {
            RewardTableSO table = (RewardTableSO)target;

            ItemMaxLevelTableSO maxLevelTable = TableEditorUtility.LoadMaxLevelTable();

            if (maxLevelTable == null)
            {
                Debug.LogError("ItemMaxLevelTableSO를 찾을 수 없습니다.");
                return;
            }

            bool dirty = TableEditorUtility.Sync(table.list, maxLevelTable, RewardCategories, false);

            if (dirty)
            {
                EditorUtility.SetDirty(table);
                AssetDatabase.SaveAssets();
            }
        }
    }

    protected override void DrawTableHeader()
    {
        Rect rect = EditorGUILayout.GetControlRect();

        float itemWidth = rect.width * 0.7f;
        float valueWidth = rect.width * 0.3f;


        EditorGUI.LabelField(
            new Rect(rect.x, rect.y, itemWidth, rect.height),
            "Item");

        EditorGUI.LabelField(
            new Rect(rect.x + itemWidth, rect.y, valueWidth, rect.height),
            "Reward Value");
    }

    protected override void DrawTableBody()
    {
        SerializedProperty listProperty = serializedObject.FindProperty("list");

        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
            SerializedProperty category = element.FindPropertyRelative("category");
            SerializedProperty level = element.FindPropertyRelative("level");
            SerializedProperty value = element.FindPropertyRelative("value");

            string label = $"[{category.enumDisplayNames[category.enumValueIndex]}  Lv.{level.intValue}]";

            Rect rect = EditorGUILayout.GetControlRect();

            float itemWidth = rect.width * 0.7f;
            float valueWidth = rect.width * 0.3f;

            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, itemWidth, rect.height),
                label);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth, rect.y, valueWidth, rect.height),
                value,
                GUIContent.none);
        }
    }
}
#endif