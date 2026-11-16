#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShopTableSO))]
public class ShopTableSOEditor : Editor
{
    private SerializedProperty lowItems;
    private SerializedProperty midItems;
    private SerializedProperty highItems;

    private void OnEnable()
    {
        lowItems = serializedObject.FindProperty("lowItemDatas");
        midItems = serializedObject.FindProperty("middleItemDatas");
        highItems = serializedObject.FindProperty("highItemDatas");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawItemList("Low Items", lowItems);
        DrawItemList("Mid Items", midItems);
        DrawItemList("High Items", highItems);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawItemList(string title, SerializedProperty list)
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        DrawDataHeader();

        for (int i = 0; i < list.arraySize; i++)
        {
            SerializedProperty item = list.GetArrayElementAtIndex(i);

            SerializedProperty category = item.FindPropertyRelative("category");
            SerializedProperty level = item.FindPropertyRelative("level");
            SerializedProperty priceType = item.FindPropertyRelative("priceType");
            SerializedProperty price = item.FindPropertyRelative("price");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(
                $"#{i}",
                GUILayout.Width(30));

            EditorGUILayout.PropertyField(
                category,
                GUIContent.none,
                GUILayout.MinWidth(100));

            EditorGUILayout.PropertyField(
                level,
                GUIContent.none,
                GUILayout.Width(50));

            EditorGUILayout.PropertyField(
                priceType,
                GUIContent.none,
                GUILayout.Width(80));

            EditorGUILayout.PropertyField(
                price,
                GUIContent.none,
                GUILayout.Width(80));

            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                list.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add"))
        {
            list.InsertArrayElementAtIndex(list.arraySize);
        }
    }

    private void DrawDataHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        EditorGUILayout.LabelField(
            "Index",
            GUILayout.Width(30));

        EditorGUILayout.LabelField(
            "Category",
            GUILayout.MinWidth(100));

        EditorGUILayout.LabelField(
            "Level",
            GUILayout.Width(50));

        EditorGUILayout.LabelField(
            "Price Type",
            GUILayout.Width(80));

        EditorGUILayout.LabelField(
            "Price",
            GUILayout.Width(80));

        GUILayout.Space(25);

        EditorGUILayout.EndHorizontal();
    }
}

#endif