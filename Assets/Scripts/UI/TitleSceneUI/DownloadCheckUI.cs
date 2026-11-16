using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownloadCheckUI : MonoBehaviour
{
    [SerializeField] TMP_Text sizeInfoText;
    [field:SerializeField] public Button downloadButton {  get; private set; }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void SetSizeText(long _size)
    {
        sizeInfoText.text = $"{DownloadManager.SetFileSizeText(_size)}";
    }
}
