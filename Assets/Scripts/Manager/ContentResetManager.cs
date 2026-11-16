using UnityEngine;

public class ContentResetManager : MonoBehaviour
{
    [SerializeField] StaminaManager staminaManager;
    [SerializeField] MissionManager missionManager;
    [SerializeField] ShopManager shopManager;
    
    public void ResetDailyContents()
    {
        staminaManager.ResetPurchaseCount();
        missionManager.ResetDailyMission();
        shopManager.DailyReset();
    }
    public void ResetWeeklyContents()
    {
        missionManager.ResetWeeklyMission();
    }
}
