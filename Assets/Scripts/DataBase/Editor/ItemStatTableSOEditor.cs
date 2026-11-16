#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemStatTableSO))]
public class ItemStatTableSOEditor : TableEditorBase
{
    private ItemCategory[] categories;

    private void OnEnable()
    {
        categories = Array.FindAll(
            (ItemCategory[])Enum.GetValues(typeof(ItemCategory)),
            x => x != ItemCategory.None);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawSyncButton();
        EditorGUILayout.Space();
        DrawFilter();
        EditorGUILayout.Space();
        DrawTableHeader();
        DrawTableBody();
        serializedObject.ApplyModifiedProperties();
    }

    protected override void DrawSyncButton()
    {
        if (GUILayout.Button("Sync Table"))
        {
            ItemStatTableSO table = (ItemStatTableSO)target;

            ItemMaxLevelTableSO maxLevelTable = TableEditorUtility.LoadMaxLevelTable();

            if (maxLevelTable == null)
            {
                Debug.LogError("ItemMaxLevelTableSO를 찾을 수 없습니다.");
                return;
            }

            bool dirty = TableEditorUtility.Sync(table.list, maxLevelTable, categories, true);

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

        float itemWidth = rect.width * 0.45f;
        float typeWidth = rect.width * 0.30f;
        float timeWidth = rect.width * 0.15f;
        float countWidth = rect.width * 0.10f;

        EditorGUI.LabelField(
            new Rect(rect.x, rect.y, itemWidth, rect.height),
            "Item");

        EditorGUI.LabelField(
            new Rect(rect.x + itemWidth, rect.y, typeWidth, rect.height),
            "Type");

        EditorGUI.LabelField(
            new Rect(rect.x + itemWidth + typeWidth, rect.y, timeWidth, rect.height),
            "Time");

        EditorGUI.LabelField(
            new Rect(rect.x + itemWidth + typeWidth + timeWidth, rect.y, countWidth, rect.height),
            "Count");
    }

    protected override void DrawTableBody()
    {
        SerializedProperty listProperty =
            serializedObject.FindProperty("list");

        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
            SerializedProperty category = element.FindPropertyRelative("category");

            ItemCategory itemCategory = (ItemCategory)category.intValue;
            if (IsVisible(itemCategory) == false)
            {
                continue;
            }

            SerializedProperty level = element.FindPropertyRelative("level");
            SerializedProperty itemType = element.FindPropertyRelative("itemType");
            SerializedProperty triggerTime = element.FindPropertyRelative("triggerTime");
            SerializedProperty maxSpawnCount = element.FindPropertyRelative("maxSpawnCount");

            string label = $"[{category.enumDisplayNames[category.enumValueIndex]} Lv.{level.intValue}]";

            Rect rect = EditorGUILayout.GetControlRect();

            float itemWidth = rect.width * 0.45f;
            float typeWidth = rect.width * 0.30f;
            float timeWidth = rect.width * 0.15f;
            float countWidth = rect.width * 0.10f;

            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, itemWidth, rect.height),
                label);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth, rect.y, typeWidth, rect.height),
                itemType,
                GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth + typeWidth, rect.y, timeWidth, rect.height),
                triggerTime,
                GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth + typeWidth + timeWidth, rect.y, countWidth, rect.height),
                maxSpawnCount,
                GUIContent.none);
        }
    }
}
#endif