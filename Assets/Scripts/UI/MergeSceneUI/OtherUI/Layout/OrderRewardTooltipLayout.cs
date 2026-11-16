using UnityEngine;
using UnityEngine.UI;

public class OrderRewardTooltipLayout : MonoBehaviour
{
    LayoutElement layoutElement;
    [SerializeField] RectTransform iconRT;
    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }
    public void ApplyLayout(float width, float height)
    {
        layoutElement.preferredWidth = width;
        layoutElement.preferredHeight = height;

        iconRT.anchorMin = new Vector2(0f, 0.5f);
        iconRT.anchorMax = new Vector2(0f, 0.5f);
        iconRT.pivot = new Vector2(0f, 0.5f);

        iconRT.anchoredPosition = Vector2.zero;
        iconRT.sizeDelta = new Vector2(height, height);
    }
}
