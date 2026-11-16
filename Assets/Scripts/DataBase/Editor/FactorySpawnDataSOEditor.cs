#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FactorySpawnDataSO))]
public class FactorySpawnDataSOEditor : Editor
{
    private ItemCategory prevCategory;
    private bool[] foldouts;
    private Vector2 scrollPosition;

    private void OnEnable()
    {
        FactorySpawnDataSO so = (FactorySpawnDataSO)target;

        prevCategory = so.factoryCategory;

        RefreshFoldouts();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        FactorySpawnDataSO so = (FactorySpawnDataSO)target;

        DrawFactoryCategory(so);

        DrawUseStaminaToggle(so);
        DrawAutoRecoverSpawnCountToggle(so);

        EditorGUILayout.Space();

        DrawUtilityButtons(so);

        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawLevels(so);

        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFactoryCategory(FactorySpawnDataSO so)
    {
        EditorGUI.BeginChangeCheck();

        ItemCategory newCategory =
            (ItemCategory)EditorGUILayout.EnumPopup(
                "Factory Category",
                so.factoryCategory);

        if (!EditorGUI.EndChangeCheck())
        {
            return;
        }

        if (newCategory == prevCategory)
        {
            return;
        }

        bool replace =
            EditorUtility.DisplayDialog(
                "Factory Category Changed",
                "기존 Spawn 데이터를 삭제하고 레벨을 다시 생성하시겠습니까?",
                "Yes",
                "No");

        if (!replace)
        {
            return;
        }

        Undo.RecordObject(
            so,
            "Change Factory Category");

        so.factoryCategory = newCategory;

        RegenerateLevels(so);

        prevCategory = newCategory;

        EditorUtility.SetDirty(so);
    }
    private void DrawUseStaminaToggle(FactorySpawnDataSO so)
    {
        EditorGUI.BeginChangeCheck();

        bool useStamina =
            EditorGUILayout.Toggle(
                "Use Stamina",
                so.useStamina);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(
                so,
                "Change Use Stamina");

            so.useStamina = useStamina;

            EditorUtility.SetDirty(so);
        }
    }
    private void DrawAutoRecoverSpawnCountToggle(FactorySpawnDataSO so)
    {
        EditorGUI.BeginChangeCheck();

        bool autoRecoverSpawnCount =
            EditorGUILayout.Toggle(
                "Auto Recover Spawn Count",
                so.autoRecoverSpawnCount);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(
                so,
                "Change Auto Recover Spawn Count");

            so.autoRecoverSpawnCount = autoRecoverSpawnCount;

            EditorUtility.SetDirty(so);
        }
    }

    private void DrawUtilityButtons(FactorySpawnDataSO so)
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Missing Levels"))
        {
            AddMissingLevels(so);
        }

        if (GUILayout.Button("Sort All"))
        {
            SortAllLevels(so);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawLevels(FactorySpawnDataSO so)
    {
        if (so.factoryLevelDatas == null)
        {
            return;
        }

        RefreshFoldouts();

        for (int i = 0; i < so.factoryLevelDatas.Count; i++)
        {
            FactoryLevelData levelData =
                so.factoryLevelDatas[i];

            int totalWeight = 0;

            if (levelData.spawnDatas != null)
            {
                for (int j = 0; j < levelData.spawnDatas.Count; j++)
                {
                    totalWeight += levelData.spawnDatas[j].weight;
                }
            }

            string title =
                $"Lv.{i + 1} ({levelData.spawnDatas?.Count ?? 0} Entries, Total Weight : {totalWeight})";

            foldouts[i] =
                EditorGUILayout.Foldout(
                    foldouts[i],
                    title,
                    true);

            if (!foldouts[i])
            {
                continue;
            }

            EditorGUI.indentLevel++;

            DrawSpawnList(levelData);

            EditorGUILayout.Space();

            if (GUILayout.Button("+ Add Spawn"))
            {
                Undo.RecordObject(
                    so,
                    "Add Spawn Info");

                levelData.spawnDatas ??=
                    new List<SpawnInfo>();

                levelData.spawnDatas.Add(
                    new SpawnInfo()
                    {
                        category = ItemCategory.None,
                        level = 1,
                        weight = 1
                    });

                EditorUtility.SetDirty(so);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
        }
    }

    private void DrawSpawnList(FactoryLevelData levelData)
    {
        if (levelData.spawnDatas == null)
        {
            return;
        }

        Rect headerRect =
            EditorGUILayout.GetControlRect();

        float categoryWidth = headerRect.width * 0.45f;
        float levelWidth = headerRect.width * 0.15f;
        float weightWidth = headerRect.width * 0.20f;
        float chanceWidth = headerRect.width * 0.15f;
        float removeWidth = headerRect.width * 0.05f;

        EditorGUI.LabelField(
            new Rect(
                headerRect.x,
                headerRect.y,
                categoryWidth,
                headerRect.height),
            "Category");

        EditorGUI.LabelField(
            new Rect(
                headerRect.x + categoryWidth,
                headerRect.y,
                levelWidth,
                headerRect.height),
            "Lv");

        EditorGUI.LabelField(
            new Rect(
                headerRect.x + categoryWidth + levelWidth,
                headerRect.y,
                weightWidth,
                headerRect.height),
            "Weight");

        EditorGUI.LabelField(
            new Rect(
                headerRect.x + categoryWidth + levelWidth + weightWidth,
                headerRect.y,
                chanceWidth,
                headerRect.height),
            "Chance");

        int totalWeight = 0;

        for (int i = 0; i < levelData.spawnDatas.Count; i++)
        {
            totalWeight += levelData.spawnDatas[i].weight;
        }

        for (int i = 0; i < levelData.spawnDatas.Count; i++)
        {
            SpawnInfo spawn =
                levelData.spawnDatas[i];

            Rect rect =
                EditorGUILayout.GetControlRect();

            spawn.category =
                (ItemCategory)EditorGUI.EnumPopup(
                    new Rect(
                        rect.x,
                        rect.y,
                        categoryWidth,
                        rect.height),
                    spawn.category);

            spawn.level =
                EditorGUI.IntField(
                    new Rect(
                        rect.x + categoryWidth,
                        rect.y,
                        levelWidth,
                        rect.height),
                    spawn.level);

            spawn.weight =
                EditorGUI.IntField(
                    new Rect(
                        rect.x + categoryWidth + levelWidth,
                        rect.y,
                        weightWidth,
                        rect.height),
                    spawn.weight);

            float chance =
                totalWeight <= 0
                    ? 0f
                    : (float)spawn.weight / totalWeight * 100f;

            EditorGUI.LabelField(
                new Rect(
                    rect.x + categoryWidth + levelWidth + weightWidth,
                    rect.y,
                    chanceWidth,
                    rect.height),
                $"{chance:F1}%");

            Rect removeRect =
                new Rect(
                    rect.x + categoryWidth + levelWidth + weightWidth + chanceWidth,
                    rect.y,
                    removeWidth,
                    rect.height);

            if (GUI.Button(removeRect, "X"))
            {
                Undo.RecordObject(
                    (FactorySpawnDataSO)target,
                    "Remove Spawn Info");

                levelData.spawnDatas.RemoveAt(i);

                EditorUtility.SetDirty(
                    (FactorySpawnDataSO)target);

                break;
            }
        }
    }

    private void RegenerateLevels(FactorySpawnDataSO so)
    {
        Undo.RecordObject(
            so,
            "Regenerate Factory Levels");

        so.factoryLevelDatas =
            new List<FactoryLevelData>();

        ItemMaxLevelTableSO maxLevelTable =
            TableEditorUtility.LoadMaxLevelTable();

        if (maxLevelTable == null)
        {
            return;
        }

        int maxLevel =
            GetMaxLevel(
                maxLevelTable,
                so.factoryCategory);

        for (int i = 0; i < maxLevel; i++)
        {
            so.factoryLevelDatas.Add(
                new FactoryLevelData());
        }

        RefreshFoldouts();

        EditorUtility.SetDirty(so);
    }

    private void AddMissingLevels(FactorySpawnDataSO so)
    {
        Undo.RecordObject(
            so,
            "Generate Missing Levels");

        ItemMaxLevelTableSO maxLevelTable =
            TableEditorUtility.LoadMaxLevelTable();

        if (maxLevelTable == null)
        {
            return;
        }

        int maxLevel =
            GetMaxLevel(
                maxLevelTable,
                so.factoryCategory);

        so.factoryLevelDatas ??=
            new List<FactoryLevelData>();

        while (so.factoryLevelDatas.Count < maxLevel)
        {
            so.factoryLevelDatas.Add(
                new FactoryLevelData());
        }

        RefreshFoldouts();

        EditorUtility.SetDirty(so);
    }

    private void SortAllLevels(FactorySpawnDataSO so)
    {
        Undo.RecordObject(
            so,
            "Sort Factory Spawn Data");

        foreach (FactoryLevelData levelData in so.factoryLevelDatas)
        {
            if (levelData.spawnDatas == null)
            {
                continue;
            }

            levelData.spawnDatas.Sort(
                (a, b) =>
                {
                    int categoryCompare =
                        a.category.CompareTo(b.category);

                    if (categoryCompare != 0)
                    {
                        return categoryCompare;
                    }

                    return a.level.CompareTo(b.level);
                });
        }

        EditorUtility.SetDirty(so);
    }

    private int GetMaxLevel(ItemMaxLevelTableSO table, ItemCategory category)
    {
        foreach (ItemMaxLevelData data in table.list)
        {
            if (data.category == category)
            {
                return data.maxLevel;
            }
        }

        return 1;
    }

    private void RefreshFoldouts()
    {
        FactorySpawnDataSO so =
            (FactorySpawnDataSO)target;

        if (so.factoryLevelDatas == null)
        {
            return;
        }

        if (foldouts == null ||
            foldouts.Length != so.factoryLevelDatas.Count)
        {
            foldouts =
                new bool[so.factoryLevelDatas.Count];
        }
    }
}
#endif