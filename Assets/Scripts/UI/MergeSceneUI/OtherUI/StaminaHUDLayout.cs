using TMPro;
using UnityEngine;

public class StaminaHUDLayout : MonoBehaviour
{
    [SerializeField] RectTransform iconRT;
    [SerializeField] RectTransform textRT;
    [SerializeField] RectTransform shopRT;
    [SerializeField] TMP_Text valueText;

    RectTransform rt;
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    public void ApplyLayout(Vector2 _pos, Vector2 _size)
    {
        if (rt == null)
        {
            rt = GetComponent<RectTransform>();
        }
        rt.anchoredPosition = _pos;
        rt.sizeDelta = _size;

        RefreshChildLayout();
    }

    void RefreshChildLayout()
    {
        float width = rt.rect.width;
        float height = rt.rect.height;

        float iconSize = height * 0.6f;

        iconRT.anchorMin = new Vector2(0, 0.5f);
        iconRT.anchorMax = new Vector2(0, 0.5f);
        iconRT.pivot = new Vector2(0.5f, 0.5f);

        iconRT.sizeDelta = new Vector2(
            iconSize,
            iconSize);

        iconRT.anchoredPosition = new Vector2(
            width * 0.15f,
            0f);

        float textHeight = height * 0.8f;

        textRT.anchorMin = new Vector2(0.5f, 0.5f);
        textRT.anchorMax = new Vector2(0.5f, 0.5f);
        textRT.pivot = new Vector2(0.5f, 0.5f);

        textRT.sizeDelta = new Vector2(
            width,
            textHeight);

        textRT.anchoredPosition = new Vector2(
            0f,
            height * 0.1f);

        float shopSize = height * 0.3f;

        shopRT.anchorMin = new Vector2(1f, 0.5f);
        shopRT.anchorMax = new Vector2(1f, 0.5f);
        shopRT.pivot = new Vector2(0.5f, 0.5f);

        shopRT.sizeDelta = new Vector2(
            shopSize,
            shopSize);

        shopRT.anchoredPosition = new Vector2(
            -(width * 0.13f),
            0f);

        valueText.fontSize = textHeight * 0.6f;
    }
}