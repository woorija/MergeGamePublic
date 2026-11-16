using TMPro;
using UnityEngine;

public class WeeklyMissionScoreRewardSlot : MonoBehaviour
{
    [SerializeField] MissionRewardSlot rewardSlot;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject lockedOverlay;
    [SerializeField] GameObject claimedOverlay;
    public void Init(WeeklyMissionScoreRewardData rewardData)
    {
        rewardSlot.SetRewardSlot(rewardData.reward);
        scoreText.text = rewardData.requiredScore.ToString();
        lockedOverlay.SetActive(true);
        claimedOverlay.SetActive(false);
    }
    public void SetClaimedOverlay(bool active)
    {
        claimedOverlay.SetActive(active);
    }
    public void SetLockedOverlay(bool active)
    {
        lockedOverlay.SetActive(active);
    }
}
