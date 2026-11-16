using UnityEngine;
using UnityEngine.UI;

public class OrderAreaLayout : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] RectTransform orderArea;

    [Header("Normal Buttons")]
    [SerializeField] LayoutElement storageUI;
    [SerializeField] LayoutElement mainQuestUI;
    [SerializeField] LayoutElement dailyMissionUI;

    [Header("Orders")]
    [SerializeField] OrderUILayout[] orderUIs;

    const float storageRatio = 0.4f;
    const float buttonWidthRatio = 0.5f;
    const float NormalButtonHeightRatio = 0.6f;
    const float OrderWidthRatio = 0.7f;

    public void Init()
    {
        ApplyLayout();
    }

    public void ApplyLayout()
    {
        float areaHeight = orderArea.rect.height;

        if (areaHeight <= 0f)
        {
            return;
        }

        float storageSize = areaHeight * storageRatio;
        float buttonSizeX = areaHeight * buttonWidthRatio;
        float buttonSizeY = areaHeight * NormalButtonHeightRatio;

        SetSize(storageUI, storageSize, storageSize);
        SetSize(mainQuestUI, buttonSizeX, buttonSizeY);
        SetSize(dailyMissionUI, buttonSizeX, buttonSizeY);

        float orderWidth = areaHeight * OrderWidthRatio;

        for (int i = 0; i < orderUIs.Length; i++)
        {
            orderUIs[i].layoutElement.preferredWidth = orderWidth;
            orderUIs[i].layoutElement.preferredHeight = areaHeight;
            orderUIs[i].ApplyLayout(orderWidth, areaHeight);
        }
    }

    private void SetSize(LayoutElement element, float width, float height)
    {
        if (element == null)
        {
            return;
        }

        element.preferredWidth = width;
        element.preferredHeight = height;
    }
}