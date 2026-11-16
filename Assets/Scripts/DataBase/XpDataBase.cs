public class XpDataBase : SingletonBehaviour<XpDataBase>
{
    XpTableSO tableSO;

    public int GetRequiredXp(int level)
    {
        return tableSO.GetRequiredXp(level);
    }

    public void Init(XpTableSO tableSO)
    {
        this.tableSO = tableSO;
    }
}
