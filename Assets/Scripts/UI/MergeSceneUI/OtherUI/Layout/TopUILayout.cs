using UnityEngine;

public class TopUILayout : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] PlayerInfoHUDLayout playerInfoHUD;
    [SerializeField] StaminaHUDLayout staminaHUD;
    [SerializeField] GoldHUDLayout goldHUD;
    [SerializeField] GemHUDLayout gemHUD;
    [SerializeField] RectTransform shopIcon;

    [Header("Padding")]
    [SerializeField] float horizontalPadding = 30f;
    [SerializeField] float verticalPadding = 10f;
    [SerializeField] float spacing = 20f;

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

        float panelWidth = rt.rect.width;
        float panelHeight = rt.rect.height;

        float playerInfoSize = panelHeight * 0.9f;
        float staminaHeight = panelHeight * 0.6f;
        float goldHeight = panelHeight * 0.6f;
        float gemHeight = panelHeight * 0.6f;
        float shopSize = panelHeight * 0.8f;

        playerInfoSize -= verticalPadding * 2f;
        staminaHeight -= verticalPadding * 2f;
        goldHeight -= verticalPadding * 2f;
        gemHeight -= verticalPadding * 2f;
        shopSize -= verticalPadding * 2f;

        playerInfoSize = Mathf.Max(0f, playerInfoSize);
        staminaHeight = Mathf.Max(0f, staminaHeight);
        goldHeight = Mathf.Max(0f, goldHeight);
        gemHeight = Mathf.Max(0f, gemHeight);
        shopSize = Mathf.Max(0f, shopSize);

        // =========================
        // Left Player Info
        // =========================

        Vector2 playerInfoPos = new Vector2(
            horizontalPadding + (playerInfoSize * 0.5f),
            -(verticalPadding + (playerInfoSize * 0.5f)));

        Vector2 playerInfoSizeData = new Vector2(
            playerInfoSize,
            playerInfoSize);

        playerInfoHUD.ApplyLayout(
            playerInfoPos,
            playerInfoSizeData);

        // =========================
        // Shop Icon
        // =========================

        SetShopLayout(shopSize);

        // =========================
        // Middle Area
        // =========================

        float remainWidth =
            panelWidth
            - (horizontalPadding * 2f)
            - playerInfoSize
            - shopSize
            - (spacing * 4f);

        remainWidth = Mathf.Max(0f, remainWidth);

        float hudWidth = remainWidth / 3f;

        float startX =
            horizontalPadding
            + playerInfoSize
            + spacing
            + (hudWidth * 0.5f);

        // =========================
        // Stamina
        // =========================

        staminaHUD.ApplyLayout(
            new Vector2(
                startX,
                -(staminaHeight * 0.5f) - verticalPadding),
            new Vector2(
                hudWidth,
                staminaHeight));

        // =========================
        // Gold
        // =========================

        goldHUD.ApplyLayout(
            new Vector2(
                startX + hudWidth + spacing,
                -(goldHeight * 0.5f) - verticalPadding),
            new Vector2(
                hudWidth,
                goldHeight));

        // =========================
        // Gem
        // =========================

        gemHUD.ApplyLayout(
            new Vector2(
                startX + ((hudWidth + spacing) * 2f),
                -(gemHeight * 0.5f) - verticalPadding),
            new Vector2(
                hudWidth,
                gemHeight));
    }

    void SetShopLayout(float _shopSize)
    {
        if (shopIcon == null)
        {
            return;
        }

        shopIcon.anchorMin = new Vector2(1f, 1f);
        shopIcon.anchorMax = new Vector2(1f, 1f);
        shopIcon.pivot = new Vector2(0.5f, 0.5f);

        shopIcon.sizeDelta = new Vector2(
            _shopSize,
            _shopSize);

        shopIcon.anchoredPosition = new Vector2(
            -(horizontalPadding + (_shopSize * 0.5f)),
            -(verticalPadding + (_shopSize * 0.5f)));
    }
}