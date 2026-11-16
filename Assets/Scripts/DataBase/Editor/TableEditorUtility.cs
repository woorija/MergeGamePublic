using Codice.Client.BaseCommands.Merge.Xml;
using System.Collections.Generic;
using UnityEditor;

public static class TableEditorUtility
{
    public static bool Sync<T>(List<T> list, ItemMaxLevelTableSO maxLevelTable, IReadOnlyList<ItemCategory> targetCategories, bool includeNone) where T : CategoryLevelData, new()
    {
        bool dirty = false;

        HashSet<(ItemCategory category, int level)> existingKeys = new();
        HashSet<(ItemCategory category, int level)> validKeys = new();

        foreach (T entry in list)
        {
            existingKeys.Add((entry.category, entry.level));
        }

        if (includeNone)
        {
            validKeys.Add((ItemCategory.None, 0));

            if (existingKeys.Add((ItemCategory.None, 0)))
            {
                list.Add(new T
                {
                    category = ItemCategory.None,
                    level = 0
                });

                dirty = true;
            }
        }

        foreach (ItemCategory category in targetCategories)
        {
            int maxLevel = maxLevelTable.GetMaxLevel(category);

            for (int level = 1; level <= maxLevel; level++)
            {
                validKeys.Add((category, level));

                if (existingKeys.Add((category, level)))
                {
                    list.Add(new T
                    {
                        category = category,
                        level = level
                    });

                    dirty = true;
                }
            }
        }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            T entry = list[i];

            if (!validKeys.Contains((entry.category, entry.level)))
            {
                list.RemoveAt(i);
                dirty = true;
            }
        }

        list.Sort(
            (a, b) =>
            {
                bool aIsNone = a.category == ItemCategory.None && a.level == 0;
                bool bIsNone = b.category == ItemCategory.None && b.level == 0;

                if (aIsNone && !bIsNone)
                {
                    return -1;
                }

                if (!aIsNone && bIsNone)
                {
                    return 1;
                }

                int categoryCompare = a.category.CompareTo(b.category);

                if (categoryCompare != 0)
                {
                    return categoryCompare;
                }

                return a.level.CompareTo(b.level);
            });

        return dirty;
    }
    public static bool SyncOrderReward<T>( List<T> list, ItemMaxLevelTableSO maxLevelTable, IReadOnlyList<ItemCategory> targetCategories) where T : CategoryLevelData, new()
    {
        bool dirty = false;

        HashSet<(ItemCategory category, int level)> existingKeys = new();
        HashSet<(ItemCategory category, int level)> validKeys = new();

        foreach (T entry in list)
        {
            existingKeys.Add((entry.category, entry.level));
        }

        foreach (ItemCategory category in targetCategories)
        {
            int maxLevel = maxLevelTable.GetMaxLevel(category);

            for (int level = 3; level <= maxLevel; level++)
            {
                validKeys.Add((category, level));

                if (existingKeys.Add((category, level)))
                {
                    list.Add(new T
                    {
                        category = category,
                        level = level
                    });

                    dirty = true;
                }
            }
        }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            T entry = list[i];

            if (!validKeys.Contains((entry.category, entry.level)))
            {
                list.RemoveAt(i);
                dirty = true;
            }
        }

        list.Sort(
            (a, b) =>
            {
                int categoryCompare = a.category.CompareTo(b.category);

                if (categoryCompare != 0)
                {
                    return categoryCompare;
                }

                return a.level.CompareTo(b.level);
            });

        return dirty;
    }
    public static bool SyncSpecialItemStat<T>(
    List<T> list,
    ItemStatTableSO itemStatTable,
    ItemMaxLevelTableSO itemMaxLevelTable)
    where T : ItemStatData, new()
    {
        bool dirty = false;

        HashSet<(ItemType sourceType, ItemCategory category)> processed = new();

        HashSet<(ItemType itemType, ItemCategory category, int level)> existingKeys = new();
        HashSet<(ItemType itemType, ItemCategory category, int level)> validKeys = new();
        ItemType[] specialTypes = new[]
        {
            ItemType.Bubble,
            ItemType.Ice
        };
        foreach (T entry in list)
        {
            existingKeys.Add((entry.itemType, entry.category, entry.level));
        }

        foreach (ItemStatData item in itemStatTable.list)
        {
            if (!processed.Add((item.itemType, item.category)))
            {
                continue;
            }

            int maxLevel = itemMaxLevelTable.GetMaxLevel(item.category);

            foreach (ItemType specialType in specialTypes)
            {
                if (!TryGetSpecialLevelRange(
                    item.category,
                    specialType,
                    maxLevel,
                    out int startLevel,
                    out int endLevel))
                {
                    continue;
                }

                for (int level = startLevel; level <= endLevel; level++)
                {
                    validKeys.Add((specialType, item.category, level));

                    if (!existingKeys.Add((specialType, item.category, level)))
                    {
                        continue;
                    }

                    list.Add(new T
                    {
                        itemType = specialType,
                        category = item.category,
                        level = level
                    });

                    dirty = true;
                }
            }
        }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            T entry = list[i];

            if (!validKeys.Contains((entry.itemType, entry.category, entry.level)))
            {
                list.RemoveAt(i);
                dirty = true;
            }
        }

        list.Sort((a, b) =>
        {
            int typeCompare = a.itemType.CompareTo(b.itemType);

            if (typeCompare != 0)
            {
                return typeCompare;
            }

            int categoryCompare = a.category.CompareTo(b.category);

            if (categoryCompare != 0)
            {
                return categoryCompare;
            }

            return a.level.CompareTo(b.level);
        });

        return dirty;
    }

    private static bool TryGetSpecialLevelRange(ItemCategory sourceCategory, ItemType specialType, int maxLevel, out int startLevel, out int endLevel)
    {
        startLevel = 0;
        endLevel = 0;

        int category = (int)sourceCategory;
        switch (category)
        {
            case >= 1 and <= 1000:
                {
                    switch (specialType)
                    {
                        case ItemType.Bubble:
                            startLevel = 2;
                            endLevel = maxLevel;
                            return startLevel <= endLevel;

                        case ItemType.Ice:
                            startLevel = 4;
                            endLevel = maxLevel - 1;
                            return startLevel <= endLevel;
                    }

                    break;
                }

            case >= 1001 and <= 5000:
                {
                    if (specialType == ItemType.Bubble)
                    {
                        startLevel = 2;
                        endLevel = maxLevel;
                        return startLevel <= endLevel;
                    }

                    break;
                }

            case >= 5001:
            case <= 0:
                break;
        }

        return false;
    }
    public static bool SyncBubblePrice<T>(List<T> list, SpecialItemStatTableSO specialTable) where T : CategoryLevelData, new()
    {
        bool dirty = false;

        HashSet<(ItemCategory category, int level)> existingKeys = new();
        HashSet<(ItemCategory category, int level)> validKeys = new();

        foreach (T entry in list)
        {
            existingKeys.Add((entry.category, entry.level));
        }

        foreach (ItemStatData item in specialTable.list)
        {
            if (item.itemType != ItemType.Bubble)
            {
                continue;
            }

            validKeys.Add((item.category, item.level));

            if (existingKeys.Add((item.category, item.level)))
            {
                list.Add(new T
                {
                    category = item.category,
                    level = item.level
                });

                dirty = true;
            }
        }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            T entry = list[i];

            if (!validKeys.Contains((entry.category, entry.level)))
            {
                list.RemoveAt(i);
                dirty = true;
            }
        }

        list.Sort((a, b) =>
        {
            int categoryCompare =
                a.category.CompareTo(b.category);

            if (categoryCompare != 0)
            {
                return categoryCompare;
            }

            return a.level.CompareTo(b.level);
        });

        return dirty;
    }
    public static ItemMaxLevelTableSO LoadMaxLevelTable()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemMaxLevelTableSO");

        if (guids.Length == 0)
        {
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);

        return AssetDatabase.LoadAssetAtPath<ItemMaxLevelTableSO>(path);
    }
    public static ItemStatTableSO LoadItemStatTable()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemStatTableSO");

        if (guids.Length == 0)
        {
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);

        return AssetDatabase.LoadAssetAtPath<ItemStatTableSO>(path);
    }
    public static SpecialItemStatTableSO LoadSpecialItemStatTable()
    {
        string[] guids = AssetDatabase.FindAssets("t:SpecialItemStatTableSO");

        if (guids.Length == 0)
        {
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);

        return AssetDatabase.LoadAssetAtPath<SpecialItemStatTableSO>(path);
    }
    public static bool IsNormalCategory(ItemCategory category)
    {
        int value = (int)category;

        return value >=1 && value <= 1000;
    }
}