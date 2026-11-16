using UnityEngine;

public class OrderUIManager : MonoBehaviour
{
    [SerializeField] OrderUI[] orderUIs;
    [SerializeField] NPCPortraitImageLoader npcPortraitImageLoader;
    public void RefreshOrder(int index, OrderData order)
    {
        orderUIs[index].gameObject.SetActive(true);
        orderUIs[index].SetOrder(order);
    }
    public void Init()
    {
        for(int i = 0; i < orderUIs.Length; i++)
        {
            orderUIs[i].Init(npcPortraitImageLoader);
            orderUIs[i].gameObject.SetActive(false);
        }
    }

    public void RefreshOrderStates(int count, OrderData[] orderDatas)
    {
        for(int i = 0; i < count; i++)
        {
            orderUIs[i].SetOrderState(orderDatas[i]);
        }
    }
    public void RefreshOrderState(int index, OrderData orderData)
    {
        orderUIs[index].SetOrderState(orderData);
    }
}
