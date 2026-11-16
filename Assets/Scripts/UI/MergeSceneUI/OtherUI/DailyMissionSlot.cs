public class DailyMissionSlot : BaseMissionSlot
{
    public void SetMission(DailyMissionRuntimeData data, int currentProgress)
    {
        SetInfoText(data.missionType, data.count);
        SetRewardSlot(data.reward);
        SetCompleteOverlay(false);
        UpdateProgress(data, currentProgress);
    }
    public void UpdateProgress(DailyMissionRuntimeData data, int currentProgress)
    {
        SetProgressText(currentProgress, data.count);
        SetSlider(currentProgress, data.count);

        if (data.isComplete)
        {
            completeButton.SetActive(false);
            completeOverlay.SetActive(true);
        }
        else
        {
            SetCompleteButton(currentProgress, data.count);
        }
    }
    
}
