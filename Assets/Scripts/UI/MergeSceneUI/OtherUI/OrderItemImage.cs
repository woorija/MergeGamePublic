using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class OrderItemImage : MonoBehaviour
{
    [SerializeField] BaseItemImage baseImage; // 아이템 이미지
    [SerializeField] Image checkImage;

    public async UniTask SetImage(OrderRequirement _data)
    {
        gameObject.SetActive(true);
        await baseImage.SetBaseImage(_data.category, _data.level);
    }
    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
    public void SetCheck(bool isComplete)
    {
        checkImage.gameObject.SetActive(isComplete);
    }
}
