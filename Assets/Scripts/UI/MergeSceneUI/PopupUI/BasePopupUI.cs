using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BasePopupUI : MonoBehaviour
{
    [SerializeField] CanvasGroup contentsGroup;

    RectTransform frameRT;

    CancellationTokenSource animationCTS;

    const float openDuration = 0.2f;
    const float closeDuration = 0.2f;

    const float openScale = 0.2f;
    const float closeScale = 0f;

    private void Awake()
    {
        frameRT = GetComponent<RectTransform>();
    }

    private void OnDestroy()
    {
        animationCTS?.Cancel();
        animationCTS?.Dispose();
    }
    public virtual void Open()
    {
        OpenAsync().Forget();
    }
    public virtual void Close()
    {
        CloseAsync().Forget();
    }
    protected async virtual UniTask OpenAsync()
    {
        if (!gameObject.activeSelf)
        {
            PopupManager.Instance.OnPopupOpened();
        }
        gameObject.SetActive(true);

        RestartAnimationCTS();

        contentsGroup.alpha = 0f;

        await OpenAnimation();
    }

    protected async virtual UniTask CloseAsync()
    {
        RestartAnimationCTS();

        contentsGroup.alpha = 0f;

        await CloseAnimation();

        gameObject.SetActive(false);
        PopupManager.Instance.OnPopupClosed();
    }

    public async UniTask OpenAnimation()
    {
        await ScaleAsync(openScale, 1f, 0f, 1f, openDuration, animationCTS.Token);
    }

    public async UniTask CloseAnimation()
    {
        await ScaleAsync(1f, closeScale, 1f, 0f, closeDuration, animationCTS.Token);
    }
    public virtual void CloseImmediate()
    {
        animationCTS?.Cancel();

        frameRT.localScale = Vector3.one * closeScale;

        contentsGroup.alpha = 0f;

        gameObject.SetActive(false);
    }

    protected async UniTask ScaleAsync(float from, float to, float alphaFrom, float alphaTo, float duration, CancellationToken token)
    {
        frameRT.localScale = Vector3.one * from;
        contentsGroup.alpha = alphaFrom;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            token.ThrowIfCancellationRequested();

            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // Ease Out Cubic
            float easeT = 1f - Mathf.Pow(1f - t, 3f);

            float scale = Mathf.LerpUnclamped(from, to, easeT);

            frameRT.localScale = Vector3.one * scale;
            contentsGroup.alpha = Mathf.Lerp(alphaFrom, alphaTo, easeT);

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        frameRT.localScale = Vector3.one * to;
        contentsGroup.alpha = alphaTo;
    }

    private void RestartAnimationCTS()
    {
        animationCTS?.Cancel();
        animationCTS?.Dispose();

        animationCTS = new CancellationTokenSource();
    }
}