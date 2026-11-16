using Cysharp.Threading.Tasks;
using UnityEngine;

public class InBoxUI : MonoBehaviour
{
    [SerializeField] BaseItemImage itemImage;

    public void RefreshUI((ItemCategory category, int level) data)
    {
        gameObject.SetActive(true);
        itemImage.SetBaseImage(data.category, data.level).Forget();
    }
}
