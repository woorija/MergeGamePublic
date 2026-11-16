using UnityEngine;

public class PlayerSaveData : BaseSaveData
{
    PlayerDataManager manager;
    public PlayerSaveData(string _path, int _bufferSize, PlayerDataManager manager) : base(_path, _bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }

    protected override void Serialize()
    {
        Save(dataVersion);
        Save(manager.Gold);
        Save(manager.Xp);
        Save(manager.Level);
        Save(manager.Gem);
        Save(manager.Stamina);
        Save(manager.NextRecoveryTimestamp);
        Save(manager.StoryProgress);
    }
    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            int gold = LoadInt();
            int xp = LoadInt();
            int level = LoadInt();
            int gem = LoadInt();
            int stamina = LoadInt();
            long nextRecoveryTimestamp = LoadLong();
            int storyProgress = LoadInt();

            manager.LoadData(gold, xp, level, gem, stamina, nextRecoveryTimestamp, storyProgress);
        }
    }
}
