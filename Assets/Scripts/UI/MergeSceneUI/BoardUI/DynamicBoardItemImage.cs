using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class DynamicBoardItemImage : BoardItemImage
{
    public async UniTask MergeScaleAsync(Vector3 _pos, float _startScale, float _endScale, CancellationToken _ct = default)
    {
        float duration = 0.1f;
        float elapsed = 0f;

        rt.position = _pos;
        rt.localScale = Vector3.one * _startScale;

        while (elapsed < duration)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, _ct);
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float scale = Mathf.Lerp(_startScale, _endScale, t);
            rt.localScale = Vector3.one * scale;
        }
        SetVisibleAll(false);
    }
    public async UniTask SpawnMoveAsync(Vector3 _from, Vector3 _to, CancellationToken _ct = default)
    {
        float duration = 0.5f;
        float jumpHeight = 100f;
        float elapsed = 0f;

        rt.position = _from;
        rt.localScale = Vector3.one * 0.1f;

        while (elapsed < duration)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, _ct);
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 4-3. 직선이동 - EaseOut
            float linearT = 1f - Mathf.Pow(1f - t, 2f);
            Vector3 linearPos = Vector3.LerpUnclamped(_from, _to, linearT);

            // 4-4. 점프이동 - Sin 곡선 (0과 0.5초에 0, 0.25초에 최고점)
            float jumpT = Mathf.Sin(t * Mathf.PI);
            Vector3 jumpPos = Vector3.up * jumpHeight * jumpT;

            // 4-1. 직선 + 점프 합산
            rt.position = linearPos + jumpPos;

            // 4-5. 스케일 0.1 → 1.0 (0.25초까지만 보간)
            float scaleT = Mathf.Clamp01(t / 0.5f);
            float scale = Mathf.Lerp(0.1f, 1f, scaleT);
            rt.localScale = Vector3.one * scale;
        }

        SetVisibleAll(false);
    }

    public async UniTask SwapMoveAsync(Vector3 _from, Vector3 _to, CancellationToken _ct = default)
    {
        float duration = 0.1f;
        float elapsed = 0f;

        rt.position = _from;

        while (elapsed < duration)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, _ct);
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 3. 등속이동
            rt.position = Vector3.Lerp(_from, _to, t);
        }

        SetVisibleAll(false);
    }
}
