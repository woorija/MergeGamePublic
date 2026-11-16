using UnityEngine;

public class PlayerDataEventHub : MonoBehaviour
{
    [SerializeField] CurrencyTextBinder goldText, gemText, staminaText, levelText;
    [SerializeField] XpImage XPFilledImage;

    public void Init()
    {
        PlayerDataManager.Instance.OnGoldChanged += goldText.SetText;
        PlayerDataManager.Instance.OnGemChanged += gemText.SetText;
        PlayerDataManager.Instance.OnStaminaChanged += staminaText.SetText;
        PlayerDataManager.Instance.OnLevelChanged += levelText.SetText;
        PlayerDataManager.Instance.OnXpProgressChanged += XPFilledImage.SetFilled;

        PlayerDataManager.Instance.InvokeAllEvent();


    }
    private void OnDestroy()
    {
        PlayerDataManager.Instance.OnGoldChanged -= goldText.SetText;
        PlayerDataManager.Instance.OnGemChanged -= gemText.SetText;
        PlayerDataManager.Instance.OnStaminaChanged -= staminaText.SetText;
        PlayerDataManager.Instance.OnLevelChanged -= levelText.SetText;
        PlayerDataManager.Instance.OnXpProgressChanged -= XPFilledImage.SetFilled;
    }
}
