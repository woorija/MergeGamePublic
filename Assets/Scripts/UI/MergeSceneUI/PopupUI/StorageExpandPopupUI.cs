using TMPro;
using UnityEngine;

public class StorageExpandPopupUI : BasePopupUI
{
    [SerializeField] TMP_Text priceText;

    int price;
    public override void Open()
    {
        base.Open();
        price = StorageManager.Instance.GetExpandCost();
        priceText.text = $"창고 슬롯 확장 시 {price}젬이 필요합니다.";
    }
}
