using UnityEngine;
using UnityEngine.UI;

public class WeeklyMissionPopupUI : BasePopupUI
{
    [SerializeField] WeeklyMissionManager weeklyMissionManager;
    [SerializeField] WeeklyMissionSlot[] weeklyMissionSlots;
    [SerializeField] WeeklyMissionScoreRewardUI weeklyMissionScoreRewardUI;
    [SerializeField] Button[] daySelectButtons;
    [SerializeField] GameObject[] daySelectedOverlays;
    [SerializeField] GameObject lockedOverlay;
    int selectedDay = -1;
    bool isFirstOpen = true;
    public void Init()
    {
        weeklyMissionManager.OnScoreChanged += RefreshScore;
        weeklyMissionManager.OnScoreRewardClaimed += weeklyMissionScoreRewardUI.OnRewardClaimed;

        weeklyMissionScoreRewardUI.Init(weeklyMissionManager.GetScoreRewardTable());
    }
    private void OnDestroy()
    {
        if(weeklyMissionManager == null)
        {
            return;
        }
        weeklyMissionManager.OnScoreChanged -= RefreshScore;
        weeklyMissionManager.OnScoreRewardClaimed -= weeklyMissionScoreRewardUI.OnRewardClaimed;
    }
    public override void Open()
    {
        base.Open();
        if (isFirstOpen)
        {
            RefreshScoreReward();
            isFirstOpen = false;
        }
        if (selectedDay == -1)
        {
            selectedDay = TimeManager.Instance.dayOfWeek;
            OnClickDay(selectedDay);
        }
        if (selectedDay <= TimeManager.Instance.dayOfWeek)
        {
            SetLockedOverlay(false);
            if (weeklyMissionManager.missionResetFlag)
            {
                RefreshMission();
                weeklyMissionManager.ClearResetFlag();
                weeklyMissionManager.ClearProgressFlag();
            }
            else if (weeklyMissionManager.missionProgressFlag)
            {
                RefreshProgress();
                weeklyMissionManager.ClearProgressFlag();
            }
        }
        else
        {
            SetLockedOverlay(true);
        }
    }
    private void RefreshMission()
    {
        int startIndex = selectedDay * 5;
        for(int i = 0; i < weeklyMissionSlots.Length; i++)
        {
            var data = weeklyMissionManager.GetMissionData(startIndex + i);
            int progress = weeklyMissionManager.GetCount(data.missionType);

            weeklyMissionSlots[i].RefreshAll(data, progress, weeklyMissionManager.isCompletes[startIndex + i]);
        }
    }
    private void RefreshProgress()
    {
        int startIndex = selectedDay * 5;
        for (int i = 0; i < weeklyMissionSlots.Length; i++)
        {
            var data = weeklyMissionManager.GetMissionData(startIndex + i);
            int progress = weeklyMissionManager.GetCount(data.missionType);

            weeklyMissionSlots[i].RefreshProgress(data, progress, weeklyMissionManager.isCompletes[startIndex + i]);
        }
    }
    private void RefreshScoreReward()
    {
        RefreshScore();
        for(int i = 0; i < weeklyMissionManager.claimedScoreRewards.Length; i++)
        {
            weeklyMissionScoreRewardUI.SetClaimedOverlay(i,weeklyMissionManager.claimedScoreRewards[i]);
        }
    }
    private void RefreshScore()
    {
        weeklyMissionScoreRewardUI.Refresh(weeklyMissionManager.totalScore);
    }
    private void SetLockedOverlay(bool active)
    {
        lockedOverlay.SetActive(active);
    }
    public void OnClickCompleteButton(int index) // 각 미션완료 버튼에 적용
    {
        weeklyMissionManager.GetMissionReward(selectedDay * 5 + index);
        weeklyMissionSlots[index].SetCompleteOverlay(true);
        weeklyMissionSlots[index].SetCompleteButton(false);
    }
    public void OnClickDay(int day) // n일차 버튼에 적용
    {
        daySelectedOverlays[selectedDay].SetActive(false);
        selectedDay = day;
        daySelectedOverlays[selectedDay].SetActive(true);
        if(selectedDay <= TimeManager.Instance.dayOfWeek)
        {
            SetLockedOverlay(false);
            RefreshMission();
        }
        else
        {
            SetLockedOverlay(true);
        }
    }
    public void OnClickScoreReward(int index) // 미션 점수보상에 적용
    {
        weeklyMissionManager.GetScoreReward(index);
    }
    public void OnReset()
    {
        if (gameObject.activeInHierarchy)
        {
            RefreshScoreReward();
            RefreshMission();
        }
    }
}
