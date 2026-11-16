using Cysharp.Threading.Tasks;
using UnityEngine;

public class StorageSlot : MonoBehaviour
{
    [SerializeField] BaseItemImage itemImage;

    int index;
    public void Init(int index)
    {
        this.index = index;
    }
    public void SetImage(ItemData itemData)
    {
        itemImage.SetBaseImage(itemData).Forget();
    }
    public void ClearImage()
    {
        itemImage.SetClearImage();
    }
    public void OnClick() // 동일 오브젝트의 버튼에 등록
    {
        StorageManager.Instance.TryMoveToBoard(index);
    }
}
