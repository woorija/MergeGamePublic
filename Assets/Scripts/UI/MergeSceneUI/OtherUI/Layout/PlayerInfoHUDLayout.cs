using UnityEngine;

public class PlayerInfoHUDLayout : MonoBehaviour
{
    [SerializeField] RectTransform borderRT;
    [SerializeField] RectTransform iconRT;
    [SerializeField] RectTransform levelXpRT;

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

        // Border 100%
        borderRT.anchorMin = Vector2.zero;
        borderRT.anchorMax = Vector2.one;
        borderRT.offsetMin = Vector2.zero;
        borderRT.offsetMax = Vector2.zero;

        // Icon 90%
        float iconSize = Mathf.Min(width, height) * 0.9f;

        iconRT.anchorMin = new Vector2(0.5f, 0.5f);
        iconRT.anchorMax = new Vector2(0.5f, 0.5f);
        iconRT.pivot = new Vector2(0.5f, 0.5f);

        iconRT.sizeDelta = new Vector2(
            iconSize,
            iconSize);

        iconRT.anchoredPosition = Vector2.zero;

        // Level + XP Area 50%
        float levelSize = Mathf.Min(width, height) * 0.5f;

        levelXpRT.anchorMin = new Vector2(0.5f, 0f);
        levelXpRT.anchorMax = new Vector2(0.5f, 0f);
        levelXpRT.pivot = new Vector2(0.5f, 0f);

        levelXpRT.sizeDelta = new Vector2(
            levelSize,
            levelSize);

        levelXpRT.anchoredPosition = new Vector2(0f, -levelSize * 0.5f);
    }
}