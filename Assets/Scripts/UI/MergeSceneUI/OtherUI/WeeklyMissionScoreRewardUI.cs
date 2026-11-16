using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeeklyMissionScoreRewardUI : MonoBehaviour
{
    [SerializeField] WeeklyMissionScoreRewardSlot[] slots;
    [SerializeField] Slider scoreSlider;
    [SerializeField] TMP_Text scoreText;

    IReadOnlyList<WeeklyMissionScoreRewardData> scoreRewardDatas;
    public void Init(IReadOnlyList<WeeklyMissionScoreRewardData> scoreRewardDatas)
    {
        this.scoreRewardDatas = scoreRewardDatas;
        for(int i = 0; i<slots.Length; i++)
        {
            slots[i].Init(scoreRewardDatas[i]);
        }
    }
    public void Refresh(int score)
    {
        RefreshScoreText(score);
        RefreshSlider(score);
        RefreshSlots(score);
    }
    public void OnRewardClaimed(int index)
    {
        slots[index].SetClaimedOverlay(true);
    }
    public void SetClaimedOverlay(int index, bool active)
    {
        slots[index].SetClaimedOverlay(active);
    }
    private void RefreshScoreText(int score)
    {
        scoreText.text = score.ToString();
    }
    private void RefreshSlider(int score)
    {
        scoreSlider.value = GetNormalizedSliderValue(score);
    }
    private void RefreshSlots(int score)
    {
        int count = Mathf.Min(slots.Length, scoreRewardDatas.Count);

        for (int i = 0; i < count; i++)
        {
            bool isUnlocked = score >= scoreRewardDatas[i].requiredScore;

            slots[i].SetLockedOverlay(!isUnlocked);
        }
    }
    private float GetNormalizedSliderValue(int score)
    {
        if (scoreRewardDatas == null || scoreRewardDatas.Count == 0)
        {
            return 0f;
        }

        int rewardCount = scoreRewardDatas.Count;

        for (int i = 0; i < rewardCount; i++)
        {
            int previousScore = i == 0 ? 0 : scoreRewardDatas[i - 1].requiredScore;

            int currentRequiredScore =  scoreRewardDatas[i].requiredScore;

            if (score <= currentRequiredScore)
            {
                float scoreProgress = Mathf.InverseLerp(previousScore, currentRequiredScore, score);

                float previousSliderValue = (float)i / rewardCount;

                float currentSliderValue = (float)(i + 1) / rewardCount;

                return Mathf.Lerp(previousSliderValue, currentSliderValue, scoreProgress);
            }
        }

        return 1f;
    }
}
