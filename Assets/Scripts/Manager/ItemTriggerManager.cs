using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ItemTriggerManager : MonoBehaviour
{
    List<ItemData> triggerItems = new List<ItemData>();

    public void Register(ItemData itemData, long triggerTimestamp)
    {
        if (triggerItems.Contains(itemData))
        {
            return;
        }
        itemData.SetTriggerTimestamp(triggerTimestamp);
        int insertIndex = FindInsertIndex(triggerTimestamp);

        triggerItems.Insert(insertIndex, itemData);
    }
    public void Unregister(ItemData itemData)
    {
        triggerItems.Remove(itemData);
    }
    public void OnSecondTick(long currentTime)
    {
        while (triggerItems.Count > 0)
        {
            ItemData itemData = triggerItems[0];

            if (itemData.triggerTimestamp > currentTime)
            {
                break;
            }

            triggerItems.RemoveAt(0); // Unregister와 같은 역할

            switch (itemData.type)
            {
                case ItemType.Factory:
                    RecoverFactory(itemData);
                    BoardManager.Instance.RefreshTimerUI(itemData, false);
                    break;
                case ItemType.Bubble:
                case ItemType.Ice:
                    BoardManager.Instance.ConvertToGold(itemData);
                    break;
            }
        }
        //DebugTriggerItems();
    }
    private int FindInsertIndex(long triggerTimestamp)
    {
        for (int i = 0; i < triggerItems.Count; i++)
        {
            if (triggerItems[i].triggerTimestamp > triggerTimestamp)
            {
                return i;
            }
        }
        return triggerItems.Count;
    }

    private void RecoverFactory(ItemData itemData)
    {
        itemData.RecoverSpawnCount();
        itemData.ClearTriggerTimestamp();
    }
    private void DebugTriggerItems()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"현재시간 : {TimeManager.Instance.currentTime}");
        sb.AppendLine("아이템목록");

        foreach (ItemData itemData in triggerItems)
        {
            sb.Append(itemData.type);
            sb.Append(", ");
            sb.Append(itemData.category);
            sb.Append(", Lv.");
            sb.Append(itemData.level);
            sb.Append(" : ");
            sb.Append(itemData.triggerTimestamp);

            if (itemData.type == ItemType.Factory)
            {
                sb.Append(" (");
                sb.Append(itemData.remainSpawnCount);
                sb.Append(" / ");
                sb.Append(itemData.statData.maxSpawnCount);
                sb.Append(")");
            }

            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }
}
