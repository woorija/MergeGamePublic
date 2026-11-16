using UnityEngine;
using UnityEngine.EventSystems;

public class StorageButton : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        DragItem.Instance.DropStorage();
        BoardSlot.ResetTarget();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PopupManager.Instance.storage.Open();
    }
}
