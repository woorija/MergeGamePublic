using TMPro;
using UnityEngine;

public class PopupUI : BasePopupUI
{
    [SerializeField] TMP_Text text;
    public void SetPopupMessage(string message)
    {
        Open();
        text.text = message;
    }
}
