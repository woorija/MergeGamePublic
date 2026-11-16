using UnityEngine;

public class OrderRewardsUILayout : MonoBehaviour
{
    RectTransform rt;
    [SerializeField] OrderRewardTooltipLayout[] orderRewardTooltips;
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    public void ApplyLayout(float width, float height)
    {
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);

        rt.sizeDelta = new Vector2(width * 0.4f, 0f);
        rt.anchoredPosition = Vector2.zero;

        foreach (var orderRewardTooltip in orderRewardTooltips)
        {
            orderRewardTooltip.ApplyLayout(width * 0.4f, height * 0.125f);
        }
    }
}
