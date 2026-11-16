using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCheckUI : MonoBehaviour
{
    [SerializeField] Button retryButton;
    [SerializeField] Button quitButton;

    public enum Result { Retry, Quit }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public async UniTask<Result> WaitForUserChoiceAsync()
    {
        var (retryTask, quitTask) = (retryButton.OnClickAsync(), quitButton.OnClickAsync());

        int index = await UniTask.WhenAny(retryTask, quitTask);

        return index == 0 ? Result.Retry : Result.Quit;
    }
}