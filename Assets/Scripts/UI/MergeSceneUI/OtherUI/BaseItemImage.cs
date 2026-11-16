using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BaseItemImage : MonoBehaviour
{
    Image baseImage;
    private void Awake()
    {
        baseImage = GetComponent<Image>();
    }
    public void SetVisible(bool visible)
    {
        if (baseImage.enabled == visible)
        {
            return;
        }
        baseImage.enabled = visible;
    }
    public async UniTask SetBaseImage(ItemData _data)
    {
        SetVisible(true);
        baseImage.sprite = await ItemImageLoader.Instance.Load(_data.category, _data.level);
    }
    public async UniTask SetBaseImage(ItemCategory category, int level)
    {
        SetVisible(true);
        baseImage.sprite = await ItemImageLoader.Instance.Load(category, level);
    }
    public void SetClearImage()
    {
        SetVisible(false);
        //baseImage.sprite = await ItemImageLoader.Instance.Load(ItemCategory.None, 0);
    }
}
