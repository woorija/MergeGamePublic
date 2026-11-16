#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 프로젝트 내 ItemSpriteAtlas_* 에셋을 스캔해서
/// 스프라이트 이름 -> (카테고리, 레벨) -> AtlasName 매핑을 만들고
/// ItemAtlasEntry 리스트에 자동 할당한다.
/// </summary>
public static class ItemAtlasAutoAssigner
{
    private const string AtlasPrefix = "ItemSpriteAtlas_";

    /// <returns>하나라도 값이 바뀌었으면 true</returns>
    public static bool Assign(List<ItemAtlasEntry> entries)
    {
        Dictionary<(ItemCategory, int), AtlasName> map = BuildSpriteMap();

        if (map.Count == 0)
        {
            Debug.LogWarning($"{AtlasPrefix}* 아틀라스에서 매핑 가능한 스프라이트를 찾지 못했습니다.");
            return false;
        }

        bool dirty = false;

        foreach (ItemAtlasEntry entry in entries)
        {
            if (map.TryGetValue((entry.category, entry.level), out AtlasName atlasName))
            {
                if (entry.atlasName != atlasName)
                {
                    entry.atlasName = atlasName;
                    dirty = true;
                }
            }
            else
            {
                // 못 찾은 항목은 기존 값을 유지(None으로 덮어쓰지 않음)
                Debug.LogWarning($"[{entry.category} Lv.{entry.level}] 해당 스프라이트가 어떤 아틀라스에도 없습니다.");
            }
        }

        return dirty;
    }

    private static Dictionary<(ItemCategory, int), AtlasName> BuildSpriteMap()
    {
        var map = new Dictionary<(ItemCategory, int), AtlasName>();

        foreach (string guid in AssetDatabase.FindAssets("t:SpriteAtlas"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);

            if (fileName.StartsWith(AtlasPrefix) == false)
            {
                continue;
            }

            // ItemSpriteAtlas_Common_1 -> Common_1 -> AtlasName.Common_1
            string atlasKey = fileName.Substring(AtlasPrefix.Length);
            if (Enum.TryParse(atlasKey, false, out AtlasName atlasName) == false
                || atlasName == AtlasName.None)
            {
                Debug.LogWarning($"AtlasName으로 변환할 수 없는 아틀라스입니다: {fileName}");
                continue;
            }

            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            if (atlas == null)
            {
                continue;
            }

            foreach (Sprite sprite in GetSprites(atlas))
            {
                if (TryParseSpriteName(sprite.name, out ItemCategory category, out int level) == false)
                {
                    Debug.LogWarning($"스프라이트 이름을 (카테고리, 레벨)로 변환할 수 없습니다: {sprite.name} ({fileName})");
                    continue;
                }

                var key = (category, level);
                if (map.TryGetValue(key, out AtlasName existing) && existing != atlasName)
                {
                    Debug.LogWarning($"[{category} Lv.{level}] 여러 아틀라스에 중복 존재: {existing}, {atlasName}");
                }

                map[key] = atlasName;
            }
        }

        return map;
    }

    /// <summary>
    /// 스프라이트 이름 규칙: "{카테고리명}_{레벨}" (예: Fruit_3)
    /// 규칙이 다르면 이 함수만 수정하면 된다.
    /// </summary>
    private static bool TryParseSpriteName(string spriteName, out ItemCategory category, out int level)
    {
        category = ItemCategory.None;
        level = 0;

        int index = spriteName.LastIndexOf('-');
        if (index <= 0)
        {
            return false;
        }

        string categoryStr = spriteName.Substring(0, index);
        string levelStr = spriteName.Substring(index + 1);

        if (Enum.TryParse(categoryStr, true, out category) == false)
        {
            return false;
        }

        return int.TryParse(levelStr, out level);
    }

    /// <summary>
    /// 아틀라스의 Objects for Packing(스프라이트 / 텍스처 / 폴더)에서 스프라이트를 모은다.
    /// 아틀라스를 Pack하지 않아도 동작한다.
    /// </summary>
    private static IEnumerable<Sprite> GetSprites(SpriteAtlas atlas)
    {
        foreach (UnityEngine.Object packable in atlas.GetPackables())
        {
            switch (packable)
            {
                case Sprite sprite:
                    yield return sprite;
                    break;

                case Texture2D texture:
                    foreach (Sprite s in LoadSpritesAtPath(AssetDatabase.GetAssetPath(texture)))
                    {
                        yield return s;
                    }
                    break;

                case DefaultAsset folder:
                    string folderPath = AssetDatabase.GetAssetPath(folder);
                    foreach (string guid in AssetDatabase.FindAssets("t:Sprite", new[] { folderPath }))
                    {
                        foreach (Sprite s in LoadSpritesAtPath(AssetDatabase.GUIDToAssetPath(guid)))
                        {
                            yield return s;
                        }
                    }
                    break;
            }
        }
    }

    private static IEnumerable<Sprite> LoadSpritesAtPath(string assetPath)
    {
        foreach (UnityEngine.Object obj in AssetDatabase.LoadAllAssetsAtPath(assetPath))
        {
            if (obj is Sprite sprite)
            {
                yield return sprite;
            }
        }
    }
}

#endif