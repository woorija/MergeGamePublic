using UnityEngine;
using UnityEngine.UI;

public class MainBoard : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 10;

    [Header("References")]
    [SerializeField] private RectTransform boardArea;

    [SerializeField] BoardSlot[] boardSlots;
    [SerializeField] RectTransform[] dynamicItemImagesRT;

    [SerializeField] RectTransform targetSlotRT;
    private RectTransform rectTransform;
    private Image image;

    public Vector2 CurrentTileSize { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        if (boardArea == null)
        {
            boardArea = GetComponentInParent<RectTransform>();
            if (boardArea == rectTransform && transform.parent != null)
            {
                boardArea = transform.parent.GetComponent<RectTransform>();
            }
        }
    }
    public void Init()
    {
        AlignSlots();
    }

    public void UpdateBoardSize()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (image == null) image = GetComponent<Image>();
        
        if (boardArea == null)
        {
            boardArea = transform.parent as RectTransform;
        }

        if (boardArea == null || image == null || image.sprite == null)
        {
            return;
        }

        image.type = Image.Type.Sliced;

        Vector4 border = image.sprite.border;
        float borderTotalWidth = border.x + border.z;
        float borderTotalHeight = border.y + border.w;

        float areaWidth = boardArea.rect.width;
        float areaHeight = boardArea.rect.height;

        if (areaWidth <= 0 || areaHeight <= 0)
        {
            Canvas.ForceUpdateCanvases();
            areaWidth = boardArea.rect.width;
            areaHeight = boardArea.rect.height;
            if (areaWidth <= 0 || areaHeight <= 0) return;
        }

        float availableWidth = areaWidth - borderTotalWidth;
        float availableHeight = areaHeight - borderTotalHeight;

        // 슬롯의 비율 설정 (가로 8 : 세로 9)
        float ratioW = 8f;
        float ratioH = 9f;

        // 가로 기준 가능한 최대 타일 너비 (8w <= availableWidth)
        float maxW_by_Width = availableWidth / gridWidth;
        // 세로 기준 가능한 최대 타일 너비 (10 * (w * 9/8) <= availableHeight => w <= availableHeight / (10 * 1.125))
        float maxW_by_Height = (availableHeight / gridHeight) * (ratioW / ratioH);

        // Aspect-Fit: 더 제한적인 쪽을 선택
        float tileW = Mathf.Min(maxW_by_Width, maxW_by_Height);
        float tileH = tileW * (ratioH / ratioW);

        CurrentTileSize = new Vector2(tileW, tileH);

        float finalWidth = (gridWidth * tileW) + borderTotalWidth - 0.2f;
        float finalHeight = (gridHeight * tileH) + borderTotalHeight - 0.2f;

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(finalWidth, finalHeight);
    }

    public Vector2 GetTileLocalPosition(int x, int y)
    {
        if (image == null || image.sprite == null) return Vector2.zero;

        Vector4 border = image.sprite.border;
        
        float startX = -rectTransform.sizeDelta.x * 0.5f + border.x;
        float startY = rectTransform.sizeDelta.y * 0.5f - border.w;

        float posX = startX + (x + 0.5f) * CurrentTileSize.x;
        float posY = startY - (y + 0.5f) * CurrentTileSize.y;

        return new Vector2(posX, posY);
    }

    public void AlignSlots()
    {
        UpdateBoardSize();

        if (boardSlots.Length == 0) return;

        for (int i = 0; i < boardSlots.Length; i++)
        {
            int x = i % gridWidth;
            int y = i / gridWidth;

            if (y >= gridHeight) break;

            RectTransform slotRt = boardSlots[i].rt;
            if (slotRt != null)
            {
                slotRt.anchorMin = new Vector2(0.5f, 0.5f);
                slotRt.anchorMax = new Vector2(0.5f, 0.5f);
                slotRt.pivot = new Vector2(0.5f, 0.5f);

                slotRt.anchoredPosition = GetTileLocalPosition(x, y);
                slotRt.sizeDelta = CurrentTileSize;

                boardSlots[i].SetItemImageRTSize(CurrentTileSize.x);
                boardSlots[i].SetTimerImageRTSize(CurrentTileSize.x * 0.3f);
            }

            boardSlots[i].Init(i);
        }

        for (int i = 0; i < dynamicItemImagesRT.Length; i++)
        {
            dynamicItemImagesRT[i].sizeDelta = new Vector2(CurrentTileSize.x, CurrentTileSize.x);
        }
        DragItem.Instance.SetImageRTSize(CurrentTileSize.x);

        targetSlotRT.sizeDelta = CurrentTileSize;
        targetSlotRT.position = new Vector3(4000, 4000, 4000);
    }
}