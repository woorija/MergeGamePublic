#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OrderRuleLevelTableSO))]
public class OrderRuleLevelTableSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Sort"))
        {
            OrderRuleLevelTableSO table =
                (OrderRuleLevelTableSO)target;

            table.levels.Sort();

            EditorUtility.SetDirty(table);
        }
    }
}
#endif