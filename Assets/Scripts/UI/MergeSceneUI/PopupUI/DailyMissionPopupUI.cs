using UnityEngine;

public class DailyMissionPopupUI : BasePopupUI
{
    [SerializeField] DailyMissionManager dailyMissionManager;
    [SerializeField] DailyMissionSlot[] dailyMissionSlots;

    public override void Open()
    {
        base.Open();
        if (dailyMissionManager.missionResetFlag)
        {
            RefreshAll();
            dailyMissionManager.ClearResetFlag();
            dailyMissionManager.ClearProgressFlag();
        }
        else if (dailyMissionManager.missionProgressFlag)
        {
            RefreshProgress();
            dailyMissionManager.ClearProgressFlag();
        }
    }
    private void RefreshAll()
    {
        for(int i = 0; i < dailyMissionSlots.Length; i++)
        {
            var data = dailyMissionManager.GetMissionData(i);
            int progress = dailyMissionManager.GetCount(data.missionType);
            dailyMissionSlots[i].SetMission(data, progress);
        }
    }
    private void RefreshProgress()
    {
        for (int i = 0; i < dailyMissionSlots.Length; i++)
        {
            var data = dailyMissionManager.GetMissionData(i);
            int progress = dailyMissionManager.GetCount(data.missionType);
            dailyMissionSlots[i].UpdateProgress(data, progress);
        }
    }
    public void OnClickCompleteButton(int index)
    {
        dailyMissionManager.GetMissionReward(index);
        dailyMissionSlots[index].SetCompleteOverlay(true);
        dailyMissionSlots[index].SetCompleteButton(false);
    }
    public void OnReset()
    {
        if (gameObject.activeInHierarchy)
        {
            RefreshAll();
        }
    }
}
