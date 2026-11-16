using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoardSlotElement : MonoBehaviour
{
    [field: SerializeField] public GameObject boardHighlight { get; private set; }
    [field: SerializeField] public GameObject slotTimerImage { get; private set; }
    [field: SerializeField] public BoardItemImage itemImage { get; private set; }
    [field: SerializeField] public BoardSlotOverlayImage boardSlotOverlayImage { get; private set; }

    public bool IsHighlightActive => boardHighlight.activeSelf;
    public Vector3 ItemPosition => itemImage.rt.position;
    public Vector3 OverlayPosition => boardSlotOverlayImage.transform.position;

    public void SetHighlight(bool active) => boardHighlight.SetActive(active);
    public void SetTimer(bool active) => slotTimerImage.SetActive(active);

    public UniTask SetItemImage(ItemData data) => itemImage.SetImage(data);
    public UniTask SetOverlayImage(BoardState state) => boardSlotOverlayImage.SetOverlayImage(state);

    public void ClearItemImage() => itemImage.SetClearImage();

    public void ClearSlot()
    {
        itemImage.SetClearImage();
        SetTimer(false);
    }
}
