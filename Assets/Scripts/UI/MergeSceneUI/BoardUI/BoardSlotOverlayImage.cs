using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BoardSlotOverlayImage : MonoBehaviour
{
    Image overlayImage;
    private void Awake()
    {
        overlayImage = GetComponent<Image>();
    }
    public async UniTask SetOverlayImage(BoardState _state)
    {
        overlayImage.sprite = await ItemImageLoader.Instance.LoadBoardOverlay(_state);
    }
}
