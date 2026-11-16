public class StaminaSaveData : BaseSaveData
{
    private StaminaManager manager;

    public StaminaSaveData(string path, int bufferSize, StaminaManager manager) : base(path, bufferSize)
    {
        this.manager = manager;
        dataVersion = 1;
    }

    protected override void Serialize()
    {
        Save(dataVersion);
        Save(manager.PurchaseCount);
    }

    protected override void Deserialize()
    {
        dataVersion = LoadInt();

        if (dataVersion == 1)
        {
            int purchaseCount = LoadInt();
            manager.SetPurchaseCount(purchaseCount);
        }
    }
}
