using UnityEngine;

public class GetFreeGemButton : MonoBehaviour
{
    [SerializeField] GameObject soldoutOverlay;

    public void SetSoldOutOverlay(bool active)
    {
        soldoutOverlay.SetActive(active);
    }
}
