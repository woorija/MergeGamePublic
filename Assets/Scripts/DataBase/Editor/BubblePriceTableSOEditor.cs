#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BubblePriceTableSO))]
public class BubblePriceTableSOEditor : TableEditorBase
{
    protected override void DrawSyncButton()
    {
        if (GUILayout.Button("Sync Table"))
        {
            BubblePriceTableSO table = (BubblePriceTableSO)target;

            SpecialItemStatTableSO specialTable = TableEditorUtility.LoadSpecialItemStatTable();

            if (specialTable == null)
            {
                Debug.LogError("SpecialItemStatTableSO를 찾을 수 없습니다.");
                return;
            }

            bool dirty = TableEditorUtility.SyncBubblePrice(table.list, specialTable);

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

        float itemWidth = rect.width * 0.65f;
        float priceWidth = rect.width * 0.35f;

        EditorGUI.LabelField(
            new Rect(rect.x, rect.y, itemWidth, rect.height),
            "Item");

        EditorGUI.LabelField(
            new Rect(
                rect.x + itemWidth,
                rect.y,
                priceWidth,
                rect.height),
            "Price");
    }

    protected override void DrawTableBody()
    {
        SerializedProperty listProperty = serializedObject.FindProperty("list");

        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);

            SerializedProperty category = element.FindPropertyRelative("category");

            ItemCategory itemCategory = (ItemCategory)category.intValue;

            if (!IsVisible(itemCategory))
            {
                continue;
            }

            SerializedProperty level = element.FindPropertyRelative("level");

            SerializedProperty price = element.FindPropertyRelative("price");

            string label = $"[{category.enumDisplayNames[category.enumValueIndex]} Lv.{level.intValue}]";

            Rect rect = EditorGUILayout.GetControlRect();

            float itemWidth = rect.width * 0.65f;
            float priceWidth = rect.width * 0.35f;

            EditorGUI.LabelField(
                new Rect(
                    rect.x,
                    rect.y,
                    itemWidth,
                    rect.height),
                label);

            EditorGUI.PropertyField(
                new Rect(
                    rect.x + itemWidth,
                    rect.y,
                    priceWidth,
                    rect.height),
                price,
                GUIContent.none);
        }
    }
}

#endif