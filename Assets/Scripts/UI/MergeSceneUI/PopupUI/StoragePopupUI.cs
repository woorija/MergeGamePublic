using UnityEngine;
using UnityEngine.UI;

public class StoragePopupUI : BasePopupUI
{
    [SerializeField] StorageSlot[] storageSlots;
    [SerializeField] Button storageExpandButton;
    StorageManager storageManager;
    public void Init()
    {
        storageManager = StorageManager.Instance;

        storageManager.OnStorageChanged += RefreshUI;
        storageManager.OnCapacityChanged += OpenStorageSlot;

        for (int i = 0; i < storageSlots.Length; i++)
        {
            storageSlots[i].Init(i);
        }
        InitializeSlots();
    }
    private void InitializeSlots()
    {
        for (int i = 0; i < storageManager.capacity; i++)
        {
            storageSlots[i].gameObject.SetActive(true);
        }
        for(int i = storageManager.capacity; i < storageSlots.Length; i++)
        {
            storageSlots[i].gameObject.SetActive(false);
        }
        if (storageManager.isFull)
        {
            storageExpandButton.gameObject.SetActive(false);
        }
    }
    public override void Open()
    {
        base.Open();
        if (storageManager.isDirty)
        {
            RefreshAllUI();
            storageManager.ClearDirty();
        }
    }
    public override void Close()
    {
        base.Close();
    }
    private void RefreshAllUI()
    {
        for(int i = 0; i < storageManager.capacity; i++)
        {
            storageSlots[i].SetImage(storageManager.GetItem(i));
        }
    }
    public void RefreshUI(int index)
    {
        storageSlots[index].SetImage(storageManager.GetItem(index));
    }
    private void OpenStorageSlot(int capacity)
    {
        int index = capacity - 1;
        storageSlots[index].gameObject.SetActive(true);
        RefreshUI(index);
        storageExpandButton.gameObject.SetActive(!storageManager.isFull);
    }
}
