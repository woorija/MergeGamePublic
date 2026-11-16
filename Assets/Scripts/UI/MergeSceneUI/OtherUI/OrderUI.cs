using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    [SerializeField] Image npcImage;
    [SerializeField] OrderItemImage[] orderItemImages;
    [SerializeField] OrderRewardsUI orderRewardsUI;
    [SerializeField] Button completeButton;

    NPCPortraitImageLoader npcPortraitImageLoader;
    public void Init(NPCPortraitImageLoader loader)
    {
        npcPortraitImageLoader = loader;
    }
    public void SetOrder(OrderData order)
    {
        SetNpcImage(order.npcId);
        for(int i = 0; i < 3; i++)
        {
            if(i < order.requirementCount)
            {
                SetOrderItemImage(i, order.requirements[i]);
            }
            else
            {
                orderItemImages[i].SetActiveFalse();
            }
        }
        SetRewardTooltip(order.totalRewardWeight);
        SetOrderState(order);
    }
    public void SetOrderState(OrderData order)
    {
        for (int i = 0; i < order.requirementCount; i++)
        {
            orderItemImages[i].SetCheck(order.requirementSatisfied[i]);
        }
        SetCompleteButton(order.canComplete);
    }
    void SetNpcImage(int npcId)
    {
        npcImage.sprite = npcPortraitImageLoader.GetSprite(npcId);
    }
    void SetOrderItemImage(int index, OrderRequirement requirement)
    {
        orderItemImages[index].SetImage(requirement).Forget();
    }
    void SetRewardTooltip(RewardWeight rewardWeight)
    {
        orderRewardsUI.SetTooltip(rewardWeight);
    }
    void SetCompleteButton(bool isComplete)
    {
        completeButton.gameObject.SetActive(isComplete);
    }
}
