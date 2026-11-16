using TMPro;
using UnityEngine;

public class OrderRewardTooltip : MonoBehaviour
{
    [SerializeField] TMP_Text rewardAmount;

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
    public void SetText(int amount)
    {
        gameObject.SetActive(true);
        rewardAmount.text = $"+ {amount}";
    }

}
