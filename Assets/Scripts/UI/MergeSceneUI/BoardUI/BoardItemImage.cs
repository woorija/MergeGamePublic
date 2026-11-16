using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BoardItemImage : MonoBehaviour
{
    [SerializeField] BaseItemImage baseImage; // 아이템 이미지
    [SerializeField] Image overlayImage; // Ice, Bubble 오버레이
    public RectTransform rt {  get; private set; }

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    public void SetVisibleAll(bool visible)
    {
        SetOverlayVisible(visible);
        baseImage.SetVisible(visible);
    }
    public void SetOverlayVisible(bool visible)
    {
        if (overlayImage.enabled == visible)
        {
            return;
        }
        overlayImage.enabled = visible;
    }
    public async UniTask SetImage(ItemData _data)
    {
        await SetOverlayImage(_data.type);
        await baseImage.SetBaseImage(_data);
    }

    async UniTask SetOverlayImage(ItemType _type)
    {
        if(_type == ItemType.Bubble)
        {
            SetOverlayVisible(true);
            overlayImage.sprite = await ItemImageLoader.Instance.LoadOverlay(ItemOverlayType.Bubble);
        }
        else if(_type == ItemType.Ice)
        {
            SetOverlayVisible(true);
            overlayImage.sprite = await ItemImageLoader.Instance.LoadOverlay(ItemOverlayType.Ice);
        }
        else
        {
            SetOverlayVisible(false);
        }
    }
    public void SetClearImage()
    {
        SetOverlayVisible(false);
        baseImage.SetClearImage();
    }
}
