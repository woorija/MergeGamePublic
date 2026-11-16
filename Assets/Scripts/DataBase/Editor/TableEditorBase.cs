using UnityEditor;
public abstract class TableEditorBase: Editor
{
    protected ItemCategory categoryFilter = ItemCategory.None;

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

    protected virtual void DrawSyncButton()
    {

    }

    protected virtual void DrawFilter()
    {
        categoryFilter = (ItemCategory)EditorGUILayout.EnumPopup(
            "Filter",
            categoryFilter);
    }

    protected abstract void DrawTableHeader();
    protected abstract void DrawTableBody();

    protected bool IsVisible(ItemCategory category)
    {
        if (categoryFilter == ItemCategory.None)
        {
            return true;
        }
        return category == categoryFilter;
    }
}
