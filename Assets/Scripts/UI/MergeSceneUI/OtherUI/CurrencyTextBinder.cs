using UnityEngine;
using TMPro;

public class CurrencyTextBinder : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public void SetText(int _value)
    {
        text.text = _value.ToString();
    }
}
