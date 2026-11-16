using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoardUIManager : SingletonBehaviour<BoardUIManager>
{
    [SerializeField] BoardSlotElement[] boardSlotElements;
    [SerializeField] DynamicBoardItemImage[] dynamicItemImages;

    [SerializeField] Transform targetSlotTransform;
    [SerializeField] Transform inBoxUITransform;

    [SerializeField] ItemInformationTooltip ItemInformationTooltip;

    int dynamicItemImageIndex = 0;
    public void RestoreBoardUI(ItemData[] itemDatas, BoardState[] boardStates)
    {
        for (int i = 0; i < boardSlotElements.Length; i++)
        {
            RefreshUI(i, itemDatas[i]).Forget();
            RefreshHighLight(i, false);
            RefreshBoardOverlayImage(i, boardStates[i]).Forget();
        }
    }
    public async UniTask RefreshUI(int index, ItemData itemData)
    {
        bool isTimerActive =
            (itemData.type == ItemType.Bubble || itemData.type == ItemType.Ice) ||
            (itemData.type == ItemType.Factory && itemData.remainSpawnCount == 0 && FactoryDataBase.Instance.GetFactoryData(itemData.category).IsSpawnable(itemData));

        await RefreshItemImage(index, itemData);
        RefreshTimer(index, isTimerActive);
    }
    public void RefreshHighLight(int _index, bool _active)
    {
        boardSlotElements[_index].SetHighlight(_active);
    }

    public void RefreshTimer(int index, bool active)
    {
        boardSlotElements[index].SetTimer(active);
    }

    public void ClearSlotImage(int index)
    {
        boardSlotElements[index].ClearSlot();
    }

    public async UniTask RefreshItemImage(int _index, ItemData _data)
    {
        await boardSlotElements[_index].SetItemImage(_data);
    }

    public async UniTask RefreshBoardOverlayImage(int _index, BoardState _state)
    {
        await boardSlotElements[_index].SetOverlayImage(_state);
    }
    public async UniTask Merge(int _index, ItemData _beforeData, ItemData _data)
    {
        int index = GetDynamicItemImageIndex();
        BoardSlotElement slot = boardSlotElements[_index];

        slot.ClearItemImage();
        Vector3 pos = slot.ItemPosition;
        dynamicItemImages[index].SetVisibleAll(true);
        await UniTask.WhenAll(
            dynamicItemImages[index].SetImage(_beforeData),
            dynamicItemImages[index].MergeScaleAsync(pos, 1f, 0f)
        );
        await UniTask.WhenAll(
            dynamicItemImages[index].SetImage(_data),
            dynamicItemImages[index].MergeScaleAsync(pos, 0f, 1f)
        );
        await slot.SetItemImage(_data);
    }
    public async UniTask Spawn(int _from, int _to, ItemData _data)
    {
        Vector3 from = GetSlotPosition(_from);
        Vector3 to = GetSlotPosition(_to);
        int index = GetDynamicItemImageIndex();

        await SpawnAsync(index, from, to, _data);
        await RefreshUI(_to, _data);
    }
    public async UniTask Spawn(int _to, ItemData _data)
    {
        Vector3 from = inBoxUITransform.position;
        Vector3 to = GetSlotPosition(_to);
        int index = GetDynamicItemImageIndex();

        await SpawnAsync(index, from, to, _data);
        await RefreshUI(_to, _data);
    }
    public async UniTask Swap(int _from, int _to, ItemData _fromData, ItemData _toData)
    {
        Vector3 from = GetSlotPosition(_from);
        Vector3 to = GetSlotPosition(_to);
        int toIndex = GetDynamicItemImageIndex();

        BoardSlotElement fromSlot = boardSlotElements[_from];
        BoardSlotElement toSlot = boardSlotElements[_to];
        bool fromHighLightActive = fromSlot.IsHighlightActive;
        bool toHighLightActive = toSlot.IsHighlightActive;
        fromSlot.SetHighlight(toHighLightActive);
        toSlot.SetHighlight(fromHighLightActive);

        await UniTask.WhenAll(SwapAsync(toIndex, to, from, _toData), RefreshUI(_to, _fromData));
        await RefreshUI(_from, _toData);
    }
    private async UniTask SpawnAsync(int _index, Vector3 _from, Vector3 _to, ItemData _data)
    {
        dynamicItemImages[_index].SetVisibleAll(true);
        await UniTask.WhenAll(
            dynamicItemImages[_index].SetImage(_data),
            dynamicItemImages[_index].SpawnMoveAsync(_from, _to)
        );
    }
    private async UniTask SwapAsync(int _index, Vector3 _from, Vector3 _to, ItemData _data)
    {
        dynamicItemImages[_index].SetVisibleAll(true);
        await UniTask.WhenAll(
            dynamicItemImages[_index].SetImage(_data),
            dynamicItemImages[_index].SwapMoveAsync(_from, _to)
        );
    }
    private Vector3 GetSlotPosition(int _index)
    {
        return boardSlotElements[_index].ItemPosition;
    }
    private int GetDynamicItemImageIndex()
    {
        int index = dynamicItemImageIndex;
        dynamicItemImageIndex = (dynamicItemImageIndex + 1) % dynamicItemImages.Length;
        return index;
    }
    public void SetTargetSlot(int index)
    {
        ItemInformationTooltip.SetInformation(index);
        if (index == -1)
        {
            targetSlotTransform.position = new Vector3(4000, 4000, 4000);
            return;
        }
        targetSlotTransform.position = boardSlotElements[index].OverlayPosition;
    }
}
