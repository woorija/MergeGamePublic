using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemMaxLevelTableSO))]
public class ItemMaxLevelTableSOEditor : Editor
{
    private void OnEnable()
    {
        ItemMaxLevelTableSO table = (ItemMaxLevelTableSO)target;

        bool dirty = false;

        dirty |= table.list.RemoveAll(x => x.category == ItemCategory.None) > 0;

        ItemCategory[] categories = (ItemCategory[])Enum.GetValues(typeof(ItemCategory));

        foreach (ItemCategory category in categories)
        {
            if (category == ItemCategory.None)
            {
                continue;
            }

            bool exists = table.list.Exists(x => x.category == category);

            if (!exists)
            {
                table.list.Add(
                    new ItemMaxLevelData
                    {
                        category = category,
                        maxLevel = 1
                    });

                dirty = true;
            }
        }

        bool needsSort = false;

        for (int i = 1; i < table.list.Count; i++)
        {
            if (table.list[i - 1].category > table.list[i].category)
            {
                needsSort = true;
                break;
            }
        }

        if (needsSort)
        {
            table.list.Sort((a, b) => a.category.CompareTo(b.category));
            dirty = true;
        }

        if (dirty)
        {
            EditorUtility.SetDirty(table);
        }
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTableHeader();
        DrawTableBody();

        serializedObject.ApplyModifiedProperties();
    }
    private void DrawTableHeader()
    {
        Rect rect = EditorGUILayout.GetControlRect();

        float itemWidth = rect.width * 0.55f;
        float levelWidth = rect.width * 0.45f;

        EditorGUI.LabelField(
            new Rect(rect.x, rect.y, itemWidth, rect.height),
            "Category");

        EditorGUI.LabelField(
            new Rect(rect.x + itemWidth, rect.y, levelWidth, rect.height),
            "Max Level");
    }
    private void DrawTableBody()
    {
        SerializedProperty entries = serializedObject.FindProperty("list");

        for (int i = 0; i < entries.arraySize; i++)
        {
            SerializedProperty element = entries.GetArrayElementAtIndex(i);
            SerializedProperty category = element.FindPropertyRelative("category");
            SerializedProperty maxLevel = element.FindPropertyRelative("maxLevel");

            Rect rect = EditorGUILayout.GetControlRect();

            float itemWidth = rect.width * 0.55f;
            float levelWidth = rect.width * 0.45f;

            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, itemWidth, rect.height),
                category.enumDisplayNames[category.enumValueIndex]);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth, rect.y, levelWidth, rect.height),
                maxLevel,
                GUIContent.none);
        }
    }



}
