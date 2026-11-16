using TMPro;
using UnityEngine;

public class ItemInformationTooltipLayout : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] RectTransform rt;

    [Header("BG")]
    [SerializeField] RectTransform nameBG;
    [SerializeField] RectTransform descBG;
    [SerializeField] RectTransform timerBG;

    [Header("Text")]
    //[SerializeField] RectTransform nameTextRT;
    [SerializeField] RectTransform descTextRT;
    //[SerializeField] RectTransform timerTextRT;

    [Header("Buttons")]
    [SerializeField] RectTransform sellButton;
    [SerializeField] RectTransform buyButton;
    [SerializeField] RectTransform deleteButton;

    float padding = 10f;
    float spacing = 5f;

    int currentButtonCount;
    float buttonHeight = 50;

    // BottomUILayout이 호출하는 진입점
    public void ApplyLayout(Vector2 _pos, Vector2 _size)
    {
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        rt.sizeDelta = _size;
        rt.anchoredPosition = _pos;

        RefreshChildLayout();
    }

    public void RefreshChildLayout()
    {
        float width = rt.rect.width;
        float height = rt.rect.height;

        // =========================
        // Height
        // =========================

        float nameHeight = height * 0.3f;
        float descHeight = height * 0.55f;
        float timerHeight = height * 0.15f;
        buttonHeight = descHeight * 0.6f;

        // =========================
        // NameBG
        // =========================

        nameBG.anchorMin = new Vector2(0f, 1f);
        nameBG.anchorMax = new Vector2(1f, 1f);
        nameBG.pivot = new Vector2(0.5f, 1f);

        nameBG.sizeDelta = new Vector2(
            0f,
            nameHeight);

        nameBG.anchoredPosition = Vector2.zero;

        // =========================
        // DescBG
        // =========================

        descBG.anchorMin = new Vector2(0f, 0f);
        descBG.anchorMax = new Vector2(1f, 0f);
        descBG.pivot = new Vector2(0.5f, 0f);

        descBG.sizeDelta = new Vector2(
            0f,
            descHeight);

        descBG.anchoredPosition = new Vector2(
            0f,
            timerHeight);

        // =========================
        // TimerBG
        // =========================

        timerBG.anchorMin = new Vector2(0f, 0f);
        timerBG.anchorMax = new Vector2(1f, 0f);
        timerBG.pivot = new Vector2(0.5f, 0f);

        timerBG.sizeDelta = new Vector2(
            0f,
            timerHeight);

        timerBG.anchoredPosition = Vector2.zero;

        // =========================
        // Buttons
        // =========================

        sellButton.sizeDelta = new Vector2(buttonHeight, buttonHeight);
        deleteButton.sizeDelta = new Vector2(buttonHeight, buttonHeight);
        buyButton.sizeDelta = new Vector2(buttonHeight, buttonHeight);

        sellButton.anchoredPosition = new Vector2(-padding - buttonHeight * 0.5f, 0f);
        deleteButton.anchoredPosition = new Vector2(-padding - buttonHeight * 0.5f, 0f);
        buyButton.anchoredPosition = new Vector2(-padding - buttonHeight * 1.5f - spacing, 0f);

        RefreshDescTextLayout();
    }

    public void RefreshDescTextLayout()
    {
        float width = rt.rect.width;
        float height = rt.rect.height * 0.7f;

        float rightOffset = 0;
        switch (currentButtonCount)
        {
            case 0:
                rightOffset = 0;
                break;
            case 1:
                rightOffset = padding + spacing + buttonHeight;
                break;
            case 2:
                rightOffset = padding * 2 + buttonHeight * 2;
                break;
        }
        descTextRT.offsetMax = new Vector2(-rightOffset, descTextRT.offsetMax.y);
    }
    public void SetButtonCount(int buttonCount)
    {
        currentButtonCount = buttonCount;

        RefreshDescTextLayout();
    }
}