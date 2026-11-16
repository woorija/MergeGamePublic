using UnityEngine;
using UnityEngine.EventSystems;

public class BoardSlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerUpHandler, IPointerDownHandler
{
    static int targetIndex;

    int index;
    public RectTransform rt {  get; private set; }
    [SerializeField] RectTransform itemImageRT;
    [SerializeField] RectTransform timerImageRT;
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData()
    {
        targetIndex = -1;
    }

    public void SetItemImageRTSize(float size)
    {
        itemImageRT.sizeDelta = new Vector2(size, size);
    }
    public void SetTimerImageRTSize(float size)
    {
        timerImageRT.sizeDelta = new Vector2(size, size);
    }
    public void Init(int _index)
    {
        index = _index;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragItem.Instance.DragStart(index, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragItem.Instance.Drag(eventData.position);
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragItem.Instance.DropBoard(index);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragItem.Instance.DragEnd();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //DevelopUtility.Log($"{index}번 슬롯 PointerUp");
        if (!DragItem.Instance.isDragging && eventData.pointerCurrentRaycast.gameObject == gameObject && BoardManager.Instance.CanDrag(index))
        {
            if (targetIndex == index)
            {
                //DevelopUtility.Log($"타겟인덱스같음:{targetIndex}");
                ItemInteractionManager.Instance.OnClick(targetIndex);
            }
            else
            {
                targetIndex = index;
                BoardUIManager.Instance.SetTargetSlot(targetIndex);
                //DevelopUtility.Log($"타겟인덱스설정:{targetIndex}");
            }
        }
    }
    public static void ResetTarget()
    {
        targetIndex = -1;
        BoardUIManager.Instance.SetTargetSlot(targetIndex);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // OnPointerUp을 사용하기 위해 구현
    }
}
