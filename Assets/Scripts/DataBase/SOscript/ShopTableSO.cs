using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopTableSO", menuName = "Scriptable Objects/ShopTableSO")]
public class ShopTableSO : ScriptableObject
{
    public List<ShopItemData> lowItemDatas;
    public List<ShopItemData> middleItemDatas;
    public List<ShopItemData> highItemDatas;
}
