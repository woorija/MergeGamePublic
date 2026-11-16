public class TimeSaveData : BaseSaveData
{
    TimeManager manager;
    public TimeSaveData(string _path, int _bufferSize, TimeManager manager) : base(_path, _bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }

    protected override void Serialize()
    {
        Save(dataVersion);
        Save(manager.currentTime);
    }
    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            long lastSavetime = LoadLong();
            manager.LoadData(lastSavetime);
        }
    }


}
