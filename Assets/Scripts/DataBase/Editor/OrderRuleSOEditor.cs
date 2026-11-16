#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
[CustomEditor(typeof(OrderRuleSO))]
public class OrderRuleSOEditor : Editor
{
    SerializedProperty maxOrderCountProperty;
    SerializedProperty maxRequirementCountProperty;
    SerializedProperty itemLevelLimitProperty;
    SerializedProperty orderItemsProperty;

    ReorderableList orderItemList;

    private void OnEnable()
    {
        maxOrderCountProperty = serializedObject.FindProperty("maxOrderCount");
        maxRequirementCountProperty = serializedObject.FindProperty("maxRequirementCount");
        itemLevelLimitProperty = serializedObject.FindProperty("itemLevelLimit");
        orderItemsProperty = serializedObject.FindProperty("orderItems");

        orderItemList = new ReorderableList(
            serializedObject,
            orderItemsProperty,
            true,
            true,
            true,
            true);

        orderItemList.drawHeaderCallback = DrawHeader;
        orderItemList.drawElementCallback = DrawElement;
        orderItemList.elementHeight = EditorGUIUtility.singleLineHeight + 4f;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.IntSlider(maxOrderCountProperty, 1, 7);
        EditorGUILayout.IntSlider(maxRequirementCountProperty, 1, 3);
        EditorGUILayout.IntSlider(itemLevelLimitProperty, 7, 14);
        EditorGUILayout.Space(8);

        orderItemList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(Rect rect)
    {
        float categoryWidth = rect.width * 0.7f;
        float levelWidth = rect.width * 0.3f;

        Rect categoryRect = new Rect(
            rect.x,
            rect.y,
            categoryWidth,
            rect.height);

        Rect levelRect = new Rect(
            rect.x + categoryWidth,
            rect.y,
            levelWidth,
            rect.height);

        EditorGUI.LabelField(categoryRect, "Category");
        EditorGUI.LabelField(levelRect, "Max Level");
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = orderItemsProperty.GetArrayElementAtIndex(index);

        SerializedProperty categoryProperty = element.FindPropertyRelative("category");
        SerializedProperty maxLevelProperty = element.FindPropertyRelative("maxLevel");

        rect.y += 2f;

        float categoryWidth = rect.width * 0.7f;
        float levelWidth = rect.width * 0.3f;

        Rect categoryRect = new Rect(
            rect.x,
            rect.y,
            categoryWidth - 4f,
            EditorGUIUtility.singleLineHeight);

        Rect levelRect = new Rect(
            rect.x + categoryWidth,
            rect.y,
            levelWidth,
            EditorGUIUtility.singleLineHeight);

        EditorGUI.BeginChangeCheck();

        EditorGUI.PropertyField(
            categoryRect,
            categoryProperty,
            GUIContent.none);

        EditorGUI.PropertyField(
            levelRect,
            maxLevelProperty,
            GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            ItemCategory category = (ItemCategory)categoryProperty.intValue;

            ItemMaxLevelTableSO maxLevelTable = TableEditorUtility.LoadMaxLevelTable();
            int maxAvailableLevel = maxLevelTable.GetMaxLevel(category);

            maxLevelProperty.intValue = Mathf.Clamp(maxLevelProperty.intValue, 3, maxAvailableLevel);
        }
    }
}
#endif