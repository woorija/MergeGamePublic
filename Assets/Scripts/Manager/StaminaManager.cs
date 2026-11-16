using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    [SerializeField] PlayerDataManager playerDataManager;

    const int recoveryLimitStamina = 100;
    const int recoveryInterval = 60;

    const int staminaPurchaseAmount = 100;
    const int InitialPurchaseCost = 10;

    int purchaseCount = 0;
    private StaminaSaveData staminaSaveData;

    public Action OnDailyReset;
    public int PurchaseCount => purchaseCount;

    private void Awake()
    {
        staminaSaveData = new StaminaSaveData($"{Application.persistentDataPath}/{SavePath.StaminaSavePath}", 128, this);
    }
    public async UniTask InitializeAsync()
    {
        bool loaded = await staminaSaveData.LoadFileAsync();

        if (!loaded)
        {
            InitDefaultData();
        }
        SaveManager.Instance.Register(staminaSaveData);
        MarkDirty();
    }
    private void InitDefaultData()
    {
        purchaseCount = 0;
    }

    private void OnDestroy()
    {
        SaveManager.Instance.Unregister(staminaSaveData);
    }

    public bool HasStamina(int value)
    {
        return playerDataManager.Stamina >= value;
    }
    public void UseStamina(int value)
    {
        playerDataManager.UseStamina(value);
    }
    public void RecoverStamina(long time)
    {
        if (playerDataManager.Stamina >= recoveryLimitStamina)
        {
            playerDataManager.NextRecoveryTimestamp = 0;
            return;
        }
        if (playerDataManager.NextRecoveryTimestamp == 0)
        {
            return;
        }
        if (time < playerDataManager.NextRecoveryTimestamp)
        {
            return;
        }
        long elapsedTime = time - playerDataManager.NextRecoveryTimestamp;

        int recoverCount = (int)(elapsedTime / recoveryInterval) + 1;

        int beforeStamina = playerDataManager.Stamina;

        int afterStamina = Mathf.Min(playerDataManager.Stamina + recoverCount, recoveryLimitStamina);
        playerDataManager.SetStamina(afterStamina);

        int actualRecoverCount = playerDataManager.Stamina - beforeStamina;

        if (playerDataManager.Stamina >= recoveryLimitStamina)
        {
            playerDataManager.NextRecoveryTimestamp = 0;
        }
        else
        {
            playerDataManager.NextRecoveryTimestamp += actualRecoverCount * recoveryInterval;
        }
    }
    public void UpdateRecoveryTimer(int currentStamina)
    {
        if(currentStamina >= recoveryLimitStamina)
        {
            playerDataManager.NextRecoveryTimestamp = 0;
        }
        else if(playerDataManager.NextRecoveryTimestamp == 0)
        {
            playerDataManager.NextRecoveryTimestamp = TimeManager.Instance.currentTime + recoveryInterval;
        }
    }

    public int GetPurchaseCost()
    {
        return InitialPurchaseCost * (1 << purchaseCount);
    }
    public bool TryPurchaseStamina()
    {
        int cost = GetPurchaseCost();

        if(playerDataManager.Gem < cost)
        {
            return false;
        }

        playerDataManager.AddStamina(staminaPurchaseAmount);
        playerDataManager.UseGem(cost);
        purchaseCount++;
        MarkDirty();
        return true;
    }
    public void ResetPurchaseCount()
    {
        purchaseCount = 0;
        MarkDirty();
        OnDailyReset?.Invoke();
    }
    #region SaveLoad
    public void SetPurchaseCount(int purchaseCount)
    {
        this.purchaseCount = purchaseCount;
    }
    private void MarkDirty()
    {
        staminaSaveData.MarkDirty();
    }
    #endregion
}
