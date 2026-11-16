using UnityEngine;

public class ShopDataBase : SingletonBehaviour<ShopDataBase>
{
    public ShopTableSO tableSO {  get; private set; }
    public void Init(ShopTableSO tableSO)
    {
        this.tableSO = tableSO;
    }
    public ShopItemData GetLowItem(int index)
    {
        return tableSO.lowItemDatas[index];
    }
    public ShopItemData GetMiddleItem(int index)
    {
        return tableSO.middleItemDatas[index];
    }
    public ShopItemData GetHighItem(int index)
    {
        return tableSO.highItemDatas[index];
    }
    public int GetLowItemCount()
    {
        return tableSO.lowItemDatas.Count;
    }
    public int GetMiddleItemCount()
    {
        return tableSO.middleItemDatas.Count;
    }
    public int GetHighItemCount()
    {
        return tableSO.highItemDatas.Count;
    }
}
