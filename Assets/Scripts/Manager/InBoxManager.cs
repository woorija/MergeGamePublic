using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class InBoxManager : MonoBehaviour
{
    [SerializeField] InBoxUI inBoxUI;
    private List<(ItemCategory category, int level)> gifts;

    InBoxSaveData inBoxSaveData;
    private void Awake()
    {
        gifts = new List<(ItemCategory category, int level)>();
        inBoxSaveData = new InBoxSaveData($"{Application.persistentDataPath}/{SavePath.InBoxSavePath}", 2048, this);
    }
    public async UniTask InitializeAsync()
    {
        bool loaded = await inBoxSaveData.LoadFileAsync();

        if (!loaded)
        {
            InitDefualtData();
        }
        SaveManager.Instance.Register(inBoxSaveData);
        MarkDirty();
    }
    private void InitDefualtData()
    {
        AddGift(ItemCategory.Stamina, 1);
        AddGift(ItemCategory.Stamina, 2);
        AddGift(ItemCategory.Stamina, 3);
        AddGift(ItemCategory.Stamina, 4);
        AddGift(ItemCategory.Stamina, 5);
    }

    public void AddGift(ItemCategory category, int level)
    {
        var gift = (category, level);
        gifts.Add(gift);
        MarkDirty();
        inBoxUI.RefreshUI(gift);
    }
    public void TryGetGiftToBoard()
    {
        var gift = PeekGift();

        if (BoardManager.Instance.TryAcquireItem(gift.category, gift.level))
        {
            int lastIndex = gifts.Count - 1;
            gifts.RemoveAt(lastIndex);
            MarkDirty();
            if (gifts.Count == 0)
            {
                inBoxUI.gameObject.SetActive(false);
            }
            else
            {
                inBoxUI.RefreshUI(PeekGift());
            }
        }
        else
        {
            PopupManager.Instance.popup.SetPopupMessage("보드가 가득 찼습니다.");
        }
    }
    public (ItemCategory category, int level) PeekGift()
    {
        return gifts[^1];
    }
    #region SaveLoad
    public List<(ItemCategory category, int level)> GetGiftsForSerialization()
    {
        return gifts;
    }
    public void RestoreInBox()
    {
        var gift = PeekGift();
        inBoxUI.RefreshUI(gift);
    }
    private void MarkDirty()
    {
        inBoxSaveData.MarkDirty();
    }
    #endregion
}
