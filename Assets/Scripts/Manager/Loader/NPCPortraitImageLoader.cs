using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
public class NPCPortraitImageLoader : MonoBehaviour
{
    SpriteAtlas atlas;
    public async UniTask InitializeAsync()
    {
        atlas = await AddressableManager.Instance.LoadAssetAsync<SpriteAtlas>("NPCPortraitAtlas",AddressableAssetScope.Local);
    }
    public Sprite GetSprite(int npcId)
    {
        return atlas.GetSprite($"NPC-{npcId}");
    }
}
