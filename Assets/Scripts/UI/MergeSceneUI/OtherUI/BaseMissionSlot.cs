using Cysharp.Threading.Tasks;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseMissionSlot : MonoBehaviour
{
    [SerializeField] protected TMP_Text infoText;
    [SerializeField] protected TMP_Text progressText;
    [SerializeField] protected Slider progressSlider;
    [SerializeField] protected MissionRewardSlot rewardSlot;
    [SerializeField] protected GameObject completeButton;
    [SerializeField] protected GameObject completeOverlay;
    protected StringBuilder sb = new StringBuilder(32);

    protected void SetRewardSlot(MissionRewardData rewardData)
    {
        rewardSlot.SetRewardSlot(rewardData);
    }
    protected void SetInfoText(MissionType missionType, int count)
    {
        sb.Clear();
        switch (missionType)
        {
            case MissionType.Merge:
                sb.Append("합성 ");
                sb.Append(count);
                sb.Append("회 진행");
                infoText.text = sb.ToString();
                return;
            case MissionType.OrderComplete:
                sb.Append("주문 ");
                sb.Append(count);
                sb.Append("회 완료");
                infoText.text = sb.ToString();
                return;
            case MissionType.ShopPurchase:
                sb.Append("상점");
                sb.Append(count);
                sb.Append("회 이용");
                infoText.text = sb.ToString();
                return;
            case MissionType.GemSpend:
                sb.Append("젬 ");
                sb.Append(count);
                sb.Append("개 사용");
                infoText.text = sb.ToString();
                return;
            case MissionType.GemEarn:
                sb.Append("젬 ");
                sb.Append(count);
                sb.Append("개 획득");
                infoText.text = sb.ToString();
                return;
            case MissionType.GoldSpend:
                sb.Append(count);
                sb.Append(" 골드 사용");
                infoText.text = sb.ToString();
                return;
            case MissionType.GoldEarn:
                sb.Append(count);
                sb.Append(" 골드 획득");
                infoText.text = sb.ToString();
                return;
            case MissionType.XpEarn:
                sb.Append("Xp ");
                sb.Append(count);
                sb.Append("획득");
                infoText.text = sb.ToString();
                return;
        }
    }
    protected void SetProgressText(int progress, int count)
    {
        progressText.text = $"{progress}/{count}";
    }
    protected void SetSlider(int progress, int count)
    {
        progressSlider.maxValue = count;
        progressSlider.value = progress;
    }
    public void SetCompleteButton(int progress, int count)
    {
        completeButton.SetActive(progress >= count);
    }
    public void SetCompleteButton(bool complete)
    {
        completeButton.SetActive(complete);
    }
    public void SetCompleteOverlay(bool complete)
    {
        completeOverlay.SetActive(complete);
    }
}
