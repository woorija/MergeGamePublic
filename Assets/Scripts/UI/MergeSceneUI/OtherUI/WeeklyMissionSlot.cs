using UnityEngine;

public class WeeklyMissionSlot : BaseMissionSlot
{
    public void RefreshAll(WeeklyMissionData data, int currentProgress, bool isComplete)
    {
        SetInfoText(data.missionType, data.targetCount);
        SetRewardSlot(data.reward);
        RefreshProgress(data, currentProgress, isComplete);
    }
    public void RefreshProgress(WeeklyMissionData data, int currentProgress, bool isComplete)
    {
        SetProgressText(currentProgress, data.targetCount);
        SetSlider(currentProgress, data.targetCount);
        if (isComplete)
        {
            SetCompleteButton(false);
            SetCompleteOverlay(true);
        }
        else
        {
            SetCompleteButton(currentProgress, data.targetCount);
            SetCompleteOverlay(false);
        }
    }
}
