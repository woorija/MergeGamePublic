#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OrderRewardTableSO))]
public class OrderRewardTableSOEditor : TableEditorBase
{
    private ItemCategory[] categories;
    private string[] categoryNames;

    private void OnEnable()
    {
        categories = Array.FindAll(
            (ItemCategory[])Enum.GetValues(typeof(ItemCategory)),
            TableEditorUtility.IsNormalCategory);

        categoryNames = new string[categories.Length + 1];
        categoryNames[0] = "All";

        for (int i = 0; i < categories.Length; i++)
        {
            categoryNames[i + 1] = categories[i].ToString();
        }
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
    protected override void DrawFilter()
    {
        int currentIndex = 0;

        if (categoryFilter != ItemCategory.None)
        {
            int categoryIndex = Array.IndexOf(categories, categoryFilter);

            currentIndex = categoryIndex + 1;
        }

        int nextIndex = EditorGUILayout.Popup(
            "Filter",
            currentIndex,
            categoryNames);

        categoryFilter = nextIndex == 0 ? ItemCategory.None : categories[nextIndex - 1];
    }
    protected override void DrawSyncButton()
    {
        if (GUILayout.Button("Sync Table"))
        {
            OrderRewardTableSO table = (OrderRewardTableSO)target;
            ItemMaxLevelTableSO maxLevelTable = TableEditorUtility.LoadMaxLevelTable();

            if (maxLevelTable == null)
            {
                Debug.LogError("ItemMaxLevelTableSO를 찾을 수 없습니다.");
                return;
            }

            bool dirty = TableEditorUtility.SyncOrderReward(table.list, maxLevelTable, categories);

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

        float itemWidth = rect.width * 0.6f;
        float goldWidth = rect.width * 0.2f;
        float xpWidth = rect.width * 0.2f;

        EditorGUI.LabelField(
            new Rect(rect.x, rect.y, itemWidth, rect.height),
            "Item");

        EditorGUI.LabelField(
            new Rect(rect.x + itemWidth, rect.y, goldWidth, rect.height),
            "Gold");

        EditorGUI.LabelField(
            new Rect(rect.x + itemWidth + goldWidth, rect.y, xpWidth, rect.height),
            "XP");
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
            SerializedProperty rewardWeight = element.FindPropertyRelative("rewardWeight");
            SerializedProperty goldRewardWeight = rewardWeight.FindPropertyRelative("gold");
            SerializedProperty xpRewardWeight = rewardWeight.FindPropertyRelative("xp");

            string label = $"[{category.enumDisplayNames[category.enumValueIndex]}  Lv.{level.intValue}]";

            Rect rect = EditorGUILayout.GetControlRect();

            float itemWidth = rect.width * 0.6f;
            float goldWidth = rect.width * 0.2f;
            float xpWidth = rect.width * 0.2f;

            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, itemWidth, rect.height),
                label);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth, rect.y, goldWidth, rect.height),
                goldRewardWeight,
                GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth + goldWidth, rect.y, xpWidth, rect.height),
                xpRewardWeight,
                GUIContent.none);
        }
    }
}

#endif