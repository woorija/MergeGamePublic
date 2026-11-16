using UnityEngine;

public class UIAreaPanel : MonoBehaviour
{
    [SerializeField] float anchorMax;
    [SerializeField] float anchorMin;
    RectTransform rt;
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    public void Init()
    {
        SetAnchor();
    }
    void SetAnchor()
    {
        rt.anchorMin = new Vector2(0f, anchorMin);
        rt.anchorMax = new Vector2(1f, anchorMax);
    }
}
