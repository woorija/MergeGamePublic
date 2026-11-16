using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] TMP_Text percentageText;
    [SerializeField] Slider progressSlider;

    long totalSize;
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void SetSlider(float _value)
    {
        progressSlider.value = _value;
    }
    public void SetPercentageText(float _percentage)
    {
        percentageText.text = $"{_percentage:##.#}%";
    }
    public void SetTotalSize(long _size)
    {
        totalSize = _size;
    }
    public void SetInfoText(float _size)
    {
        infoText.text = $"{DownloadManager.SetFileSizeText((long)_size)} / {DownloadManager.SetFileSizeText(totalSize)}";
    }
    public void SetInfoText(string _text)
    {
        infoText.text = _text;
    }
}
