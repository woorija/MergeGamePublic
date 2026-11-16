using UnityEngine;
using UnityEngine.UI;

public class OrderUILayout : MonoBehaviour
{
    [SerializeField] RectTransform NpcImageRT;
    [SerializeField] RectTransform requirementAreaRT;
    [SerializeField] RectTransform requirementBackgroundRT;
    [SerializeField] LayoutElement[] requirementElements;
    [SerializeField] RectTransform completeButtonRT;
    [SerializeField] OrderRewardsUILayout orderRewardsUILayout;


    RectTransform rt;
    public LayoutElement layoutElement {  get; private set; }
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        layoutElement = GetComponent<LayoutElement>();
    }

    public void ApplyLayout(float width, float height)
    {
        // ==========================
        // NPC
        // ==========================
        NpcImageRT.anchorMin = Vector2.zero;
        NpcImageRT.anchorMax = Vector2.one;
        NpcImageRT.offsetMin = Vector2.zero;
        NpcImageRT.offsetMax = Vector2.zero;

        // ==========================
        // Requirement Area
        // ==========================
        float requirementAreaHeight = height * 0.3f;

        requirementAreaRT.anchorMin = new Vector2(0f, 0f);
        requirementAreaRT.anchorMax = new Vector2(1f, 0f);
        requirementAreaRT.pivot = new Vector2(0.5f, 0f);

        requirementAreaRT.anchoredPosition = Vector2.zero;
        requirementAreaRT.sizeDelta = new Vector2(0f, requirementAreaHeight);

        // ==========================
        // Requirement BG
        // ========================== 
        float requirementBGHeight = height * 0.15f;

        requirementBackgroundRT.anchorMin = new Vector2(0f, 0f);
        requirementBackgroundRT.anchorMax = new Vector2(1f, 0f);
        requirementBackgroundRT.pivot = new Vector2(0.5f, 0f);

        requirementBackgroundRT.anchoredPosition = Vector2.zero;
        requirementBackgroundRT.sizeDelta = new Vector2(0f, requirementBGHeight);

        // ==========================
        // Requirement Items
        // ==========================
        float requirementSize = width * 0.3f;

        for (int i = 0; i < requirementElements.Length; i++)
        {
            requirementElements[i].preferredWidth = requirementSize;
            requirementElements[i].preferredHeight = requirementSize;
        }

        // ==========================
        // Complete Button
        // ==========================
        completeButtonRT.anchorMin = new Vector2(0.5f, 0.5f);
        completeButtonRT.anchorMax = new Vector2(0.5f, 0.5f);
        completeButtonRT.pivot = new Vector2(0.5f, 0.5f);

        completeButtonRT.sizeDelta = new Vector2(width * 0.8f, height * 0.2f);
        completeButtonRT.anchoredPosition = Vector2.zero;

        // ==========================
        // Reward
        // ==========================
        orderRewardsUILayout.ApplyLayout(width, height);
    }
}
