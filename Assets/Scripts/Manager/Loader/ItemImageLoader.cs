using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ItemImageLoader : SingletonBehaviour<ItemImageLoader>
{
    private Dictionary<string, SpriteAtlas> atlasCache = new Dictionary<string, SpriteAtlas>();
    private Dictionary<(ItemCategory, int), string> atlasNameCache = new Dictionary<(ItemCategory, int), string>();
    private Dictionary<(ItemCategory, int), string> itemNameCache = new Dictionary<(ItemCategory, int), string>();
    const string overlayAtlasName = "ItemOverlaySpriteAtlas";
    public void Init(ItemAtlasTableSO _tableSO)
    {
        ItemAtlasTableSO atlasTable = _tableSO;

        atlasCache.Clear();
        atlasNameCache.Clear();
        itemNameCache.Clear();

        List<ItemAtlasEntry> entries = _tableSO.entries;

        int count = entries.Count;

        atlasNameCache.EnsureCapacity(count);
        itemNameCache.EnsureCapacity(count);

        for(int i = 0; i < count; i++)
        {
            ItemAtlasEntry entry = entries[i];

            (ItemCategory, int) key = (entry.category, entry.level);

            atlasNameCache[key] = $"ItemSpriteAtlas_{entry.atlasName.ToStr()}";
            itemNameCache[key] = $"{entry.category}-{entry.level}";
        }
    }
    public async UniTask<Sprite> LoadBoardOverlay(BoardState _state)
    {
        const string atlasName = "BoardSlotAtlas";

        var atlas = await LoadAtlas(atlasName);

        if (atlas == null)
        {
            return null;
        }

        string name;

        switch (_state)
        {
            case BoardState.Hidden:
                {
                    name = "hiddenOverlay";
                    break;
                }
            case BoardState.Blocked:
                {
                    name = "blockedOverlay";
                    break;
                }
            default:
                {
                    name = "clearImage";
                    break;
                }
        }

        Sprite sprite = atlas.GetSprite(name);

        if (sprite == null)
        {
            DevelopUtility.Log($"스프라이트를 찾을 수 없습니다 : {name}");
        }

        return sprite;
    }

    public async UniTask<Sprite> Load(ItemCategory _category, int _level)
    {
        string atlasName = atlasNameCache[(_category, _level)];

        if (atlasName == null)
        {
            DevelopUtility.Log($"AtlasTable에 ({_category}, {_level}) 항목이 없습니다.");
            return null;
        }

        if (!atlasCache.TryGetValue(atlasName, out var atlas))
        {
            atlas = await LoadAtlas(atlasName);

            if (atlas == null)
            {
                DevelopUtility.Log($"아틀라스 로드 실패 : {atlasName}");
                return null;
            }

            atlasCache[atlasName] = atlas;
        }

        var key = (_category, _level);
        if (!itemNameCache.TryGetValue(key, out var name))
        {
            name = $"{_category}-{_level}";
            itemNameCache[key] = name;
        }

        Sprite sprite = atlas.GetSprite(name);

        if (sprite == null)
        {
            DevelopUtility.Log($"스프라이트를 찾을 수 없습니다 : {name}");
        }

        return sprite;
    }
    public async UniTask<Sprite> LoadXp()
    {
        const string atlasName = "ItemSpriteAtlas_Reward";
        const string spriteName = "Xp";

        if (!atlasCache.TryGetValue(atlasName, out var atlas))
        {
            atlas = await LoadAtlas(atlasName);

            if (atlas == null)
            {
                DevelopUtility.Log($"아틀라스 로드 실패 : {atlasName}");
                return null;
            }

            atlasCache[atlasName] = atlas;
        }

        Sprite sprite = atlas.GetSprite(spriteName);

        if (sprite == null)
        {
            DevelopUtility.Log($"스프라이트를 찾을 수 없습니다 : {spriteName}");
        }

        return sprite;
    }

    public async UniTask<Sprite> LoadOverlay(ItemOverlayType _overlayType)
    {
        string atlasName = overlayAtlasName;

        if (!atlasCache.TryGetValue(atlasName, out var atlas))
        {
            atlas = await LoadAtlas(atlasName);

            if (atlas == null)
            {
                DevelopUtility.Log($"아틀라스 로드 실패 : {atlasName}");
                return null;
            }

            atlasCache[atlasName] = atlas;
        }

        string name;
        switch (_overlayType)
        {
            case ItemOverlayType.Ice:
                name = "IceOverlay";
                break;
            case ItemOverlayType.Bubble:
                name = "BubbleOverlay";
                break;
            default:
                name = "clearImage";
                break;
        }

        Sprite sprite = atlas.GetSprite(name);

        if (sprite == null)
        {
            DevelopUtility.Log($"스프라이트를 찾을 수 없습니다 : {name}");
        }

        return sprite;
    }

    private async UniTask<SpriteAtlas> LoadAtlas(string atlasName)
    {
        if (!atlasCache.TryGetValue(atlasName, out var atlas))
        {
            atlas = await AddressableManager.Instance.LoadAssetAsync<SpriteAtlas>(atlasName, AddressableAssetScope.Global);

            if (atlas == null)
            {
                DevelopUtility.Log($"아틀라스 로드 실패 : {atlasName}");
                return null;
            }

            atlasCache[atlasName] = atlas;
        }

        return atlas;
    }
}
