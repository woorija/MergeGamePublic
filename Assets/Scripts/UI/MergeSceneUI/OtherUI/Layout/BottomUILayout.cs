using UnityEngine;

public class BottomUILayout : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] RectTransform storageUI;
    [SerializeField] ItemInformationTooltipLayout informationUI;
    [SerializeField] RectTransform exitUI;

    float horizontalPadding = 20f;
    float verticalPadding = 10f;
    float spacing = 30f;

    RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    public void Init()
    {
        RefreshLayout();
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying == false)
        {
            rt = GetComponent<RectTransform>();

            RefreshLayout();
        }
    }
#endif

    protected virtual void OnRectTransformDimensionsChange()
    {
        RefreshLayout();
    }

    void RefreshLayout()
    {
        if (rt == null)
        {
            return;
        }

        if (storageUI == null ||
            informationUI == null ||
            exitUI == null)
        {
            return;
        }

        float panelWidth = rt.rect.width;
        float panelHeight = rt.rect.height;

        // =========================
        // Square Size
        // =========================

        float squareSize
            = panelHeight - (verticalPadding * 2f);

        squareSize = Mathf.Max(0f, squareSize);

        // =========================
        // storageUI
        // =========================

        storageUI.anchorMin = new Vector2(0f, 0.5f);
        storageUI.anchorMax = new Vector2(0f, 0.5f);
        storageUI.pivot = new Vector2(0.5f, 0.5f);

        storageUI.sizeDelta = new Vector2(
            squareSize,
            squareSize);

        storageUI.anchoredPosition = new Vector2(
            horizontalPadding + (squareSize * 0.5f),
            0f);

        // =========================
        // exitUI
        // =========================

        exitUI.anchorMin = new Vector2(1f, 0.5f);
        exitUI.anchorMax = new Vector2(1f, 0.5f);
        exitUI.pivot = new Vector2(0.5f, 0.5f);

        exitUI.sizeDelta = new Vector2(
            squareSize,
            squareSize);

        exitUI.anchoredPosition = new Vector2(
            -(horizontalPadding + (squareSize * 0.5f)),
            0f);

        // =========================
        // information UI
        // =========================

        float informationWidth =
            panelWidth
            - (horizontalPadding * 2f)
            - (spacing * 2f)
            - (squareSize * 2f);

        informationWidth = Mathf.Max(0f, informationWidth);

        float informationHeight = squareSize;

        informationUI.ApplyLayout(Vector2.zero, new Vector2(informationWidth, informationHeight));
    }
}
