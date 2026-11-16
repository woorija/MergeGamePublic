using Cysharp.Threading.Tasks;
using UnityEngine;

public class DragItem : SingletonBehaviour<DragItem>
{
    BoardItemImage dragItemImage;

    public bool isDragging {  get; private set; }
    public bool isDropped { get; private set; }
    int index;
    protected override void Awake()
    {
        base.Awake();
        dragItemImage = GetComponent<BoardItemImage>();
    }
    public void SetImageRTSize(float _size)
    {
        dragItemImage.rt.sizeDelta = new Vector2(_size, _size);
    }

    public void DragStart(int index, Vector2 pos)
    {
        if (BoardManager.Instance.CanDrag(index) == false)
        {
            isDragging = false;
            return;
        }

        isDragging = true;
        isDropped = false;

        this.index = index;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)dragItemImage.rt.parent, pos, null, out Vector2 localPos);
        dragItemImage.rt.localPosition = localPos;
        dragItemImage.SetImage(BoardManager.Instance.GetItemData(index)).Forget();
        BoardUIManager.Instance.ClearSlotImage(index);
    }
    public void DragEnd()
    {
        if (isDragging == false) return;

        if (!isDropped)
        {
            BoardUIManager.Instance.RefreshUI(index, BoardManager.Instance.GetItemData(index)).Forget();
        }
        isDragging = false;
        index = -1;
        dragItemImage.rt.localPosition = new Vector2(-2000, 2000);
    }
    public void Drag(Vector2 pos)
    {
        if (isDragging == false) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)dragItemImage.rt.parent, pos, null, out Vector2 localPos);
        dragItemImage.rt.localPosition = localPos;
    }
    public void DropBoard(int dropIndex)
    {
        if (isDragging == false) return;
        isDropped = true;
        BoardManager.Instance.ResolveDrag(index, dropIndex);
    }
    public void DropStorage()
    {
        if (isDragging == false) return;
        isDropped = true;
        ItemData itemData = BoardManager.Instance.GetItemData(index);
        StorageAddItemResult result = StorageManager.Instance.AddItem(itemData);
        if (result == StorageAddItemResult.Success)
        {
            BoardManager.Instance.ClearIndex(index);
        }
        else
        {
            BoardUIManager.Instance.RefreshUI(index, itemData).Forget();
            if (result == StorageAddItemResult.SpecialItem)
            {
                PopupManager.Instance.popup.SetPopupMessage("버블, 아이스 아이템은 창고에 넣을 수 없습니다.");
            }
            else if (result == StorageAddItemResult.NoEmptySlot)
            {
                PopupManager.Instance.popup.SetPopupMessage("창고가 가득 찼습니다.");
            }
        }
    }
}
