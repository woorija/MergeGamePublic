using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemNameTableSO))]
public class ItemNameTableSOEditor : TableEditorBase
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
            ItemNameTableSO table = (ItemNameTableSO)target;
            ItemMaxLevelTableSO maxLevelTable = TableEditorUtility.LoadMaxLevelTable();

            if (maxLevelTable == null)
            {
                Debug.LogError("ItemMaxLevelTableSO를 찾을 수 없습니다.");
                return;
            }

            bool dirty = TableEditorUtility.Sync(table.list, maxLevelTable, categories, false);

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

        float itemWidth = rect.width * 0.55f;
        float nameWidth = rect.width * 0.45f;

        EditorGUI.LabelField(
            new Rect(rect.x, rect.y, itemWidth, rect.height),
            "Item");

        EditorGUI.LabelField(
            new Rect(rect.x + itemWidth, rect.y, nameWidth, rect.height),
            "Name");
    }

    protected override void DrawTableBody()
    {
        SerializedProperty listProperty = serializedObject.FindProperty("list");

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
            SerializedProperty itemName = element.FindPropertyRelative("itemName");

            string label = $"[{category.enumDisplayNames[category.enumValueIndex]}  Lv.{level.intValue}]";

            Rect rect = EditorGUILayout.GetControlRect();

            float itemWidth = rect.width * 0.55f;
            float nameWidth = rect.width * 0.45f;

            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, itemWidth, rect.height),
                label);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth, rect.y, nameWidth, rect.height),
                itemName,
                GUIContent.none);
        }
    }
}
