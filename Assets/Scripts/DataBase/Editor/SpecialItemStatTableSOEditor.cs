#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpecialItemStatTableSO))]
public class SpecialItemStatTableSOEditor : TableEditorBase
{
    private ItemCategory[] categories;

    private enum SpecialItemTypeFilter
    {
        None,
        Bubble,
        Ice
    }

    private SpecialItemTypeFilter itemTypeFilter = SpecialItemTypeFilter.None;

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
        if (GUILayout.Button("Sync Table") == false)
        {
            return;
        }

        SpecialItemStatTableSO table = (SpecialItemStatTableSO)target;

        ItemMaxLevelTableSO maxLevelTable = TableEditorUtility.LoadMaxLevelTable();
        ItemStatTableSO itemStatTable = TableEditorUtility.LoadItemStatTable();

        if (maxLevelTable == null)
        {
            Debug.LogError("ItemMaxLevelTableSO를 찾을 수 없습니다.");
            return;
        }

        bool dirty = TableEditorUtility.SyncSpecialItemStat(table.list, itemStatTable, maxLevelTable);

        if (dirty)
        {
            EditorUtility.SetDirty(table);
            AssetDatabase.SaveAssets();
        }
    }

    protected override void DrawFilter()
    {
        itemTypeFilter = (SpecialItemTypeFilter)EditorGUILayout.EnumPopup(
            "Type",
            itemTypeFilter);

        categoryFilter = (ItemCategory)EditorGUILayout.EnumPopup(
            "Category",
            categoryFilter);
    }

    protected override void DrawTableHeader()
    {
        Rect rect = EditorGUILayout.GetControlRect();

        float itemWidth = rect.width * 0.80f;
        float timeWidth = rect.width * 0.20f;

        EditorGUI.LabelField(
            new Rect(rect.x, rect.y, itemWidth, rect.height),
            "Item");

        EditorGUI.LabelField(
            new Rect(rect.x + itemWidth, rect.y, timeWidth, rect.height),
            "Time");
    }

    protected override void DrawTableBody()
    {
        SerializedProperty listProperty =
            serializedObject.FindProperty("list");

        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);

            SerializedProperty itemType = element.FindPropertyRelative("itemType");

            SerializedProperty category = element.FindPropertyRelative("category");

            ItemType type = (ItemType)itemType.enumValueIndex;
            ItemCategory itemCategory = (ItemCategory)category.intValue;

            if (IsVisible(type, itemCategory) == false)
            {
                continue;
            }

            SerializedProperty level = element.FindPropertyRelative("level");

            SerializedProperty triggerTime = element.FindPropertyRelative("triggerTime");

            string label =
                $"[{itemType.enumDisplayNames[itemType.enumValueIndex]}] " +
                $"[{category.enumDisplayNames[category.enumValueIndex]} Lv.{level.intValue}]";

            Rect rect = EditorGUILayout.GetControlRect();

            float itemWidth = rect.width * 0.80f;
            float timeWidth = rect.width * 0.20f;

            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, itemWidth, rect.height),
                label);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth, rect.y, timeWidth, rect.height),
                triggerTime,
                GUIContent.none);
        }
    }
    private bool IsVisible(ItemType itemType, ItemCategory category)
    {
        if (itemTypeFilter != SpecialItemTypeFilter.None)
        {
            ItemType filterType = itemTypeFilter switch
            {
                SpecialItemTypeFilter.Bubble => ItemType.Bubble,
                SpecialItemTypeFilter.Ice => ItemType.Ice,
                _ => ItemType.None
            };

            if (itemType != filterType)
            {
                return false;
            }
        }

        if (categoryFilter != ItemCategory.None &&
            category != categoryFilter)
        {
            return false;
        }

        return true;
    }
}

#endif