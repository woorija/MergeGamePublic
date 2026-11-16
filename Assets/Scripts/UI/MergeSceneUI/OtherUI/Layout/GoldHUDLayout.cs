using TMPro;
using UnityEngine;

public class GoldHUDLayout : MonoBehaviour
{
    [SerializeField] RectTransform iconRT;
    [SerializeField] RectTransform textRT;
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

        valueText.fontSize = textHeight * 0.6f;
    }
}