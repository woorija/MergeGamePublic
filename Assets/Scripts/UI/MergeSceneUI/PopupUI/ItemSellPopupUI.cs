using TMPro;
using UnityEngine;

public class ItemSellPopupUI : BasePopupUI
{
    [SerializeField] TMP_Text tooltipText;
    int index;
    ItemSellType sellType;
    public void Open(int index, ItemSellType sellType)
    {
        this.index = index;
        this.sellType = sellType;
        SetText();
        Open();
    }
    public override void Close()
    {
        index = -1;
        base.Close();
    }
    void SetText()
    {
        switch (sellType)
        {
            case ItemSellType.None:
                tooltipText.text = "알 수 없는 이유로 문제가 발생했습니다";
                return;
            case ItemSellType.ToGoldItem:
                tooltipText.text = "해당 아이템을 판매 시 골드 아이템 Lv.1을 획득합니다.";
                return;
            case ItemSellType.RewardGold:
                tooltipText.text = "해당 아이템을 판매 시 1골드를 획득합니다.";
                return;
            case ItemSellType.NoReward:
                tooltipText.text = "아이템을 버리시겠습니까?";
                return;
        }
    }
    public void Sell()
    {
        ItemSellResult result = BoardManager.Instance.SellItem(index);

        Close();
        switch (result)
        {
            case ItemSellResult.Success:
            case ItemSellResult.NotSellable:
                return;
            case ItemSellResult.StateChanged:
                PopupManager.Instance.popup.SetPopupMessage("이미 사라진 아이템입니다.");
                return;
        }
    }
}
