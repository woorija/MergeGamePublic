using System;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerDataManager : SingletonBehaviour<PlayerDataManager>
{
    public int Gold {  get; private set; }

    public int Xp { get; private set; }
    public int MaxXp { get; private set; }

    private int level;
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
            OnLevelChanged?.Invoke(level);
        }
    }

    public int Gem {  get; private set; }

    public int Stamina { get; private set; }

    private long nextRecoveryTimestamp;
    public long NextRecoveryTimestamp
    {
        get => nextRecoveryTimestamp;
        set
        {
            nextRecoveryTimestamp = value;
            MarkDirty();
        }
    }

    private PlayerSaveData playerSaveData;

    private int storyProgress;
    public int StoryProgress
    {
        get
        {
            return storyProgress;
        }
        private set
        {
            storyProgress = value;
        }
    }

    public Action<int> OnLevelChanged;
    public Action<int> OnGoldChanged;
    public Action<int> OnGemChanged;
    public Action<int> OnStaminaChanged;
    public event Action<float> OnXpProgressChanged;

    public Action<int> OnGoldUsed;
    public Action<int> OnGemUsed;
    public Action<int> OnStaminaUsed;

    public Action<int> OnXpAdded;
    public Action<int> OnGoldAdded;
    public Action<int> OnGemAdded;

    protected override void Awake()
    {
        base.Awake();
        playerSaveData = new PlayerSaveData($"{Application.persistentDataPath}/{SavePath.PlayerSavePath}", 128, this);
    }
    public async UniTask InitializeAsync()
    {
        bool loaded = await playerSaveData.LoadFileAsync();

        if (!loaded)
        {
            InitDefaultData();   
        }
        SaveManager.Instance.Register(playerSaveData);
    }

    private void OnDestroy()
    {
        SaveManager.Instance.Unregister(playerSaveData);
    }

    public void Test()
    {
        Xp += 1;
        MarkDirty();
    }
    public void InvokeAllEvent()
    {
        OnLevelChanged?.Invoke(Level);
        OnGoldChanged?.Invoke(Gold);
        OnGemChanged?.Invoke(Gem);
        OnStaminaChanged?.Invoke(Stamina);
        OnXpProgressChanged?.Invoke((float)Xp / MaxXp);
    }
    private void InitDefaultData()
    {
        Level = 1;
        MaxXp = XpDataBase.Instance.GetRequiredXp(level);
        Xp = 0;
        Stamina = 100;
        Gem = 0;
    }

    public void LoadData(int gold, int xp, int level, int gem, int stamina, long nextRecoveryTimestamp, int storyProgress)
    {
        Level = level;
        MaxXp = XpDataBase.Instance.GetRequiredXp(level);
        Gold = gold;
        Xp = xp;
        Gem = gem;
        Stamina = stamina;
        this.nextRecoveryTimestamp = nextRecoveryTimestamp;
        StoryProgress = storyProgress;
        InvokeAllEvent();
    }

    private void MarkDirty()
    {
        playerSaveData.MarkDirty();
    }
    // =========================
    // Gold
    // =========================
    public void SetGold(int amount)
    {
        Gold = amount;
        MarkDirty();
        OnGoldChanged?.Invoke(Gold);
    }
    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Gold += amount;
        MarkDirty();
        OnGoldChanged?.Invoke(Gold);
        OnGoldAdded?.Invoke(amount);
    }

    public void UseGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Gold -= amount;
        MarkDirty();
        OnGoldChanged?.Invoke(Gold);
        OnGoldUsed?.Invoke(amount);
    }

    public bool TryUseGold(int amount)
    {
        if (amount <= 0 || Gold < amount)
        {
            return false;
        }

        UseGold(amount);

        return true;
    }


    // =========================
    // XP
    // =========================
    public void SetXp(int amount)
    {
        Xp = amount;
        MarkDirty();
        OnXpProgressChanged?.Invoke((float)Xp / MaxXp);
    }
    public void AddXp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Xp += amount;

        while (Xp >= MaxXp)
        {
            Xp -= MaxXp;
            Level += 1;
            MaxXp = XpDataBase.Instance.GetRequiredXp(level);
        }
        MarkDirty();
        OnXpProgressChanged?.Invoke((float)Xp / MaxXp);
        OnXpAdded?.Invoke(amount);
    }


    // =========================
    // Gem
    // =========================

    public void SetGem(int amount)
    {
        Gem = amount;
        MarkDirty();
        OnGemChanged?.Invoke(Gem);
    }
    public void AddGem(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Gem += amount;
        MarkDirty();
        OnGemChanged?.Invoke(Gem);
        OnGemAdded?.Invoke(amount);

    }

    public void UseGem(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Gem -= amount;
        MarkDirty();
        OnGemUsed?.Invoke(amount);
        OnGemChanged?.Invoke(Gem);
    }

    public bool TryUseGem(int amount)
    {
        if (amount <= 0 || Gem < amount)
        {
            return false;
        }

        UseGem(amount);

        return true;
    }


    // =========================
    // Stamina
    // =========================
    public void SetStamina(int amount)
    {
        Stamina = amount;
        MarkDirty();
        OnStaminaChanged?.Invoke(Stamina);
    }
    public void AddStamina(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Stamina += amount;
        MarkDirty();
        OnStaminaChanged?.Invoke(Stamina);
    }

    public void UseStamina(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Stamina -= amount;
        MarkDirty();
        OnStaminaUsed?.Invoke(amount);
        OnStaminaChanged?.Invoke(Stamina);
    }

    public bool TryUseStamina(int amount)
    {
        if (amount <= 0 || Stamina < amount)
        {
            return false;
        }

        UseStamina(amount);

        return true;
    }
}
