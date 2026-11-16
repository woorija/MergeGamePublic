using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] DownloadManager downloadManager;

    [SerializeField] ProgressUI progressUI;
    [SerializeField] DownloadCheckUI checkUI;
    [SerializeField] NetworkCheckUI networkCheckUI;

    private async void Start()
    {
        Application.targetFrameRate = 60;
        await InitializeAsync();
    }

    async UniTask InitializeAsync()
    {
        DevelopUtility.Log("Init");
        progressUI.Close();
        checkUI.Close();
        networkCheckUI.Close();

        bool initSuccess = await TryInitWithNetworkCheckAsync();
        if (!initSuccess)
        {
            // 유저가 종료를 선택한 경우
            return;
        }

        long downloadSize = await downloadManager.CheckDownloadSizeAsync();

        if (downloadSize > 0)
        {
            checkUI.Open();

            checkUI.SetSizeText(downloadSize);

            await checkUI.downloadButton.OnClickAsync();

            checkUI.Close();
            progressUI.Open();

            progressUI.SetTotalSize(downloadSize);

            bool success = await downloadManager.DownloadAsync(progressUI);

            progressUI.Close();

            if (!success)
            {
                DevelopUtility.Log("다운로드 실패");
                return;
            }

            await downloadManager.CleanBundleCacheAsync();
        }

        await CustomSceneManager.Instance.LoadDatabaseSceneAsync();

        DBInitializer dbInitializer = FindFirstObjectByType<DBInitializer>();

        await dbInitializer.InitializeAsync(progressUI);

        //메타씬이 열려있으면 메타씬으로, 아니면 머지씬으로 이동
        CustomSceneManager.Instance.LoadMergeScene();
    }

    async UniTask<bool> TryInitWithNetworkCheckAsync()
    {
        while (true)
        {
            // 1차: 네트워크 인터페이스 자체가 죽어있는 경우
            if (!downloadManager.IsNetworkConnected())
            {
                networkCheckUI.Open();
                var choice = await networkCheckUI.WaitForUserChoiceAsync();
                networkCheckUI.Close();

                if (choice == NetworkCheckUI.Result.Quit)
                {
                    QuitGame();
                    return false;
                }

                continue; // 재연결 버튼 -> 다시 루프
            }
            // 2차: 네트워크는 붙어있지만 실제 카탈로그 통신이 실패하는 경우
            bool success = await downloadManager.TryInitAddressablesAsync();

            if (success)
            {
                return true;
            }

            // 실패 시에도 동일한 팝업으로 재시도 유도
            networkCheckUI.Open();
            var retryChoice = await networkCheckUI.WaitForUserChoiceAsync();
            networkCheckUI.Close();

            if (retryChoice == NetworkCheckUI.Result.Quit)
            {
                QuitGame();
                return false;
            }
        }
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
