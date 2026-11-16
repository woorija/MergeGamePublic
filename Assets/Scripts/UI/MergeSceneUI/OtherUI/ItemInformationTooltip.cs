using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInformationTooltip : MonoBehaviour
{
    [SerializeField] ItemInformationTooltipLayout layout;

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text timerText;

    [SerializeField] Button sellButton;
    [SerializeField] Button buyButton;
    [SerializeField] Button deleteButton;

    ItemData itemData;
    int index = -1;
    StringBuilder sb = new StringBuilder(256);
    private void Awake()
    {
        SetInformation(-1);
    }
    public void SetInformation(int index)
    {
        if(index == -1)
        {
            ClearInformation();
        }
        else
        {
            itemData = BoardManager.Instance.GetItemData(index);
            if(itemData.category == ItemCategory.None)
            {
                ClearInformation();
            }
            else
            {
                SetInformation();
            }
        }
        this.index = index;
    }
    void SetInformation()
    {
        SetNameText();
        SetButtons();
        SetDescText();
        ClearTimerText();
    }
    void ClearInformation()
    {
        itemData = null;
        nameText.text = string.Empty;
        descText.text = string.Empty;
        timerText.text = string.Empty;
        sellButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
        layout.SetButtonCount(0);
        index = -1;
    }
    void SetButtons()
    {
        switch (itemData.type)
        {
            case ItemType.Bubble:
            case ItemType.Ice:
                sellButton.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(true);
                layout.SetButtonCount(2);
                break;
            case ItemType.Reward:
                sellButton.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(false);
                layout.SetButtonCount(0);
                break;
            default:
                buyButton.gameObject.SetActive(false);
                if(itemData.level >= 3)
                {
                    sellButton.gameObject.SetActive(true);
                    deleteButton.gameObject.SetActive(false);
                }
                else
                {
                    sellButton.gameObject.SetActive(false);
                    deleteButton.gameObject.SetActive(true);
                }
                layout.SetButtonCount(1);
                break;
        }
    }
    void SetDescText()
    {
        sb.Clear();
        switch (itemData.type)
        {
            case ItemType.Bubble:
                descText.text = "거품 안의 아이템을 구매하거나 제거하세요. 거품이 제거되면 금화를 획득합니다.";
                break;
            case ItemType.Ice:
                descText.text = "같은 아이템을 합성해 레벨업할 수 있습니다.시간이 지나면 얼음이 녹아 금화가 됩니다.";
                break;
            case ItemType.None:
                descText.text = string.Empty;
                break;
            default:
                bool isMaxLevel = itemData.level == ItemMaxLevelDataBase.ItemMaxLevelTable[itemData.category];
                
                if (isMaxLevel)
                {
                    sb.Append("이 아이템은 이미 최고 레벨입니다.");
                }
                else
                {
                    sb.Append("같은 아이템을 합성해 레벨업할 수 있습니다.");
                }

                if(itemData.type == ItemType.Factory && FactoryDataBase.Instance.GetFactoryData(itemData.category).IsSpawnable(itemData))
                {
                    sb.Append(" 누르면 아이템을 생성합니다.");
                }
                else if(itemData.type == ItemType.Reward)
                {
                    sb.Append(" 누르면 ");
                    sb.Append(RewardDataBase.rewardTable[(itemData.category,itemData.level)]);
                    sb.Append("의");
                    switch (itemData.category)
                    {
                        case ItemCategory.Gold:
                            sb.Append("골드를 획득합니다.");
                            break;
                        case ItemCategory.Gem:
                            sb.Append("젬을 획득합니다.");
                            break;
                        case ItemCategory.Stamina:
                            sb.Append("스태미너를 획득합니다.");
                            break;
                    }
                }

                descText.text = sb.ToString();
                break;
        }
    }
    void SetNameText()
    {
        sb.Clear();

        sb.Append(ItemNameDataBase.ItemNameTable[(itemData.category, itemData.level)]);
        sb.Append(" (");
        sb.Append(itemData.level);
        sb.Append("레벨)");

        nameText.text = sb.ToString();
    }
    void SetTimerText(long currentTime)
    {
        sb.Clear();

        long remainSeconds = itemData.triggerTimestamp - currentTime;

        if (remainSeconds < 0)
        {
            remainSeconds = 0;
        }

        TimeSpan time = TimeSpan.FromSeconds(remainSeconds);

        long hours = (long)time.TotalHours;

        if (hours > 0)
        {
            sb.Append(hours);
            sb.Append("시간 ");
        }

        if (time.Minutes > 0)
        {
            sb.Append(time.Minutes);
            sb.Append("분 ");
        }

        if (time.Seconds > 0 || remainSeconds == 0)
        {
            sb.Append(time.Seconds);
            sb.Append("초");
        }

        switch (itemData.type)
        {
            case ItemType.Factory:
                sb.Append("가 지나면 아이템 생성횟수를 회복합니다.");
                break;
            case ItemType.Bubble:
            case ItemType.Ice:
                sb.Append("후 아이템이 제거됩니다.");
                break;
        }

        timerText.text = sb.ToString();
    }
    void ClearTimerText()
    {
        timerText.text = string.Empty;
    }
    public void OnSecondTick(long currentTime)
    {
        if (itemData == null)
        {
            return;
        }

        switch (itemData.type)
        {
            case ItemType.Bubble:
            case ItemType.Ice:
                SetTimerText(currentTime);
                break;

            case ItemType.Factory:
                if (FactoryDataBase.Instance.GetFactoryData(itemData.category).IsSpawnable(itemData) && itemData.remainSpawnCount == 0)
                {
                    SetTimerText(currentTime);
                }
                else
                {
                    ClearTimerText();
                }
                break;

            case ItemType.None:
            case ItemType.Normal:
            case ItemType.Reward:
                return;
        }
    }
    public void OnClickPurchase()
    {
        if(index == -1)
        {
            return;
        }
        ItemData itemData = BoardManager.Instance.GetItemData(index);
        PopupManager.Instance.bubblePurchase.Open(itemData);
    }
    public void OnClickSell()
    {
        if (index == -1)
        {
            return;
        }
        ItemSellType sellType = BoardManager.Instance.GetSellType(index);
        PopupManager.Instance.itemSell.Open(index, sellType);
    }
}
