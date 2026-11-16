using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomSceneManager : DontDestroySingletonBehaviour<CustomSceneManager>
{
    const string titleSceneName = "TitleScene";
    const string databaseSceneName = "DBScene";
    const string mergeSceneName = "MergeScene";
    const string metaSceneName = "MetaScene";
    const string loadingSceneName = "LoadingScene";

    [SerializeField] Image fadeScreenImage;
    [SerializeField] float fadeOutDuration = 0.6f;
    [SerializeField] float fadeInDuration = 0.8f;

    string currentSceneName = titleSceneName;

    public bool isSceneChanged { get; private set; } = false;

    public void LoadMergeScene()
    {
        LoadScene(mergeSceneName, InitializeMergeSceneAsync).Forget();
    }

    public void LoadMetaScene()
    {
        LoadScene(metaSceneName).Forget();
    }
    async UniTask InitializeMergeSceneAsync()
    {
        MergeSceneInitializer initializer = FindFirstObjectByType<MergeSceneInitializer>();
        if(initializer == null)
        {
            DevelopUtility.Log("머지씬이니셜라이저가 존재하지않음");
            return;
        }
        await initializer.InitializeAsync();
    }
    public async UniTask LoadScene(string _scenename, Func<UniTask> onSceneLoaded = null)
    {
        if (isSceneChanged)
        {
            return;
        }

        isSceneChanged = true;

        try
        {
            await LoadSceneAsync(_scenename, onSceneLoaded);
        }
        catch (Exception ex)
        {
            DevelopUtility.Log(ex);

            try
            {
                await LoadTitleSceneAsync();
            }
            catch (Exception fallbackEx)
            {
                DevelopUtility.Log(fallbackEx);
                // 타이틀 복귀조차 실패한 최후 상황 - 화면이 영원히 가려진 채 멈추는 것만은 방지
                await FadeInScreen();
            }
        }
        finally
        {
            isSceneChanged = false;
        }
    }

    async UniTask LoadSceneAsync(string _scenename, Func<UniTask> onSceneLoaded)
    {
        await PreLoadingAsync();

        await UnloadSceneIfLoadedAsync(currentSceneName);

        AddressableManager.Instance.ReleaseCurrentSceneAssets();
        
        await SceneManager.LoadSceneAsync(_scenename, LoadSceneMode.Additive).ToUniTask(cancellationToken: destroyCancellationToken);
        currentSceneName = _scenename;

        if(onSceneLoaded != null)
        {
            await onSceneLoaded();
        }

        await UnloadSceneIfLoadedAsync(loadingSceneName);
        await FadeInScreen();
    }

    async UniTask LoadTitleSceneAsync()
    {
        await FadeOutScreen();

        // 이전 흐름이 어느 단계에서 실패했는지 알 수 없으므로,
        // 남아있을 수 있는 씬들을 전부 방어적으로 정리한다.
        await UnloadSceneIfLoadedAsync(currentSceneName);
        await UnloadSceneIfLoadedAsync(loadingSceneName);

        await SceneManager.LoadSceneAsync(titleSceneName, LoadSceneMode.Additive).ToUniTask(cancellationToken: destroyCancellationToken);
        currentSceneName = titleSceneName;

        await FadeInScreen();
    }

    async UniTask UnloadTitleSceneAsync()
    {
        await UnloadSceneIfLoadedAsync(titleSceneName);
    }

    public async UniTask LoadDatabaseSceneAsync()
    {
        if (isSceneChanged)
        {
            return;
        }

        isSceneChanged = true;

        try
        {
            await SceneManager.LoadSceneAsync(databaseSceneName, LoadSceneMode.Additive).ToUniTask(cancellationToken: destroyCancellationToken);
        }
        finally
        {
            isSceneChanged = false;
        }
    }

    async UniTask PreLoadingAsync()
    {
        await FadeOutScreen();
        await SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive).ToUniTask(cancellationToken: destroyCancellationToken);
    }

    async UniTask UnloadSceneIfLoadedAsync(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }

        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        await SceneManager.UnloadSceneAsync(scene).ToUniTask(cancellationToken: destroyCancellationToken);
    }

    UniTask FadeInScreen()
    {
        return FadeScreenAsync(0f, fadeInDuration);
    }

    UniTask FadeOutScreen()
    {
        return FadeScreenAsync(1f, fadeOutDuration);
    }

    async UniTask FadeScreenAsync(float targetAlpha, float duration)
    {
        Color color = fadeScreenImage.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;

            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeScreenImage.color = color;
            await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
        }

        color.a = targetAlpha;
        fadeScreenImage.color = color;
    }
}