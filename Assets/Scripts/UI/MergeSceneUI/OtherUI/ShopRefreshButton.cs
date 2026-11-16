using TMPro;
using UnityEngine;

public class ShopRefreshButton : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public void SetRefreshPriceText(int price)
    {
        text.text = price.ToString();
    }
}
