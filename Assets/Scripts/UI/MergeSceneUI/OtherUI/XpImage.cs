using UnityEngine;
using UnityEngine.UI;

public class XpImage : MonoBehaviour
{
    [SerializeField] Image image;

    public void SetFilled(float _value)
    {
        image.fillAmount = _value;
    }
}
