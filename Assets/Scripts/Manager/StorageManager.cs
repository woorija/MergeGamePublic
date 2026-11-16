using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class StorageManager : SingletonBehaviour<StorageManager>
{
    const int defaultCapacity = 1;
    const int maxCapacity = 100;

    ItemData[] itemDatas;
    private StorageSaveData storageSaveData;

    public event Action<int> OnStorageChanged;
    public event Action<int> OnCapacityChanged;
    public int capacity {  get; private set; }
    public bool isDirty {  get; private set; }
    public bool isFull => capacity >= maxCapacity;

    protected override void Awake()
    {
        base.Awake();
        itemDatas = new ItemData[maxCapacity];
        for (int i = 0; i < maxCapacity; i++)
        {
            itemDatas[i] = new ItemData();
        }
        storageSaveData = new StorageSaveData($"{Application.persistentDataPath}/{SavePath.StorageSavePath}", 4096, this);
    }
    public async UniTask InitializeAsync()
    {
        bool loaded = await storageSaveData.LoadFileAsync();

        if (!loaded)
        {
            InitDefaultData();
        }

        isDirty = true;
        SaveManager.Instance.Register(storageSaveData);
        MarkDirty();
    }
    private void InitDefaultData()
    {
        capacity = defaultCapacity;
    }

    private void OnDestroy()
    {
        SaveManager.Instance.Unregister(storageSaveData);
    }
    public StorageAddItemResult AddItem(ItemData itemData)
    {
        if(itemData.type == ItemType.Bubble ||  itemData.type == ItemType.Ice)
        {
            return StorageAddItemResult.SpecialItem;
        }
        int emptyIndex = FindEmptyIndex();
        if(emptyIndex == -1)
        {
            return StorageAddItemResult.NoEmptySlot;
        }
        itemDatas[emptyIndex].CopyFrom(itemData);
        isDirty = true;
        MarkDirty();
        return StorageAddItemResult.Success;
    }
    public void RemoveItem(int index)
    {
        itemDatas[index].Clear();
        MarkDirty();
    }
    public void TryMoveToBoard(int index)
    {
        ItemData itemData = itemDatas[index];

        if(itemData.type == ItemType.None)
        {
            return;
        }

        if (BoardManager.Instance.TryGetStorageItem(itemData))
        {
            itemData.Clear();
            MarkDirty();
            OnStorageChanged?.Invoke(index);
        }
        else
        {
            PopupManager.Instance.popup.SetPopupMessage("보드가 가득 찼습니다.");
        }
    }
    public void ExpandStoragePopupOpen() //ExpandStorage버튼에 할당
    {
        PopupManager.Instance.storageExpand.Open();
    }
    public void ExpandStorage() //ExpandStorage 팝업 ok버튼에 할당
    {
        int cost = GetExpandCost();
        if (PlayerDataManager.Instance.TryUseGem(cost))
        {
            capacity++;
            MarkDirty();
            OnCapacityChanged?.Invoke(capacity);
            return;
        }
        PopupManager.Instance.popup.SetPopupMessage("젬이 부족해 구매할 수 없습니다.");
    }
    private int FindEmptyIndex()
    {
        for (int i = 0; i < capacity; i++)
        {
            if (itemDatas[i].category == ItemCategory.None)
            {
                return i;
            }
        }

        return -1;
    }
    public int GetExpandCost()
    {
        return capacity;
    }
    public ItemData GetItem(int index)
    {
        return itemDatas[index];
    }
    public void ClearDirty()
    {
        isDirty = false;
    }
    #region SaveLoad
    public ItemData[] GetItemDatasForSerialization()
    {
        return itemDatas;
    }
    public void SetCapacity(int capacity)
    {
        this.capacity = capacity;
    }
    public void RestoreStorageState()
    {
        OnCapacityChanged?.Invoke(capacity);
    }
    private void MarkDirty()
    {
        storageSaveData.MarkDirty();
    }
    #endregion
}
