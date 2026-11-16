using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DownloadManager : MonoBehaviour
{
    [SerializeField] List<string> Labels = new List<string>();


    long patchSize;
    List<object> labelKeys = new List<object>();
    private void Awake()
    {
        foreach (string key in Labels)
        {
            labelKeys.Add(key);
        }
    }

    public async UniTask<bool> TryInitAddressablesAsync()
    {
        try
        {
            DevelopUtility.Log("Init");
            await Addressables.InitializeAsync();
            DevelopUtility.Log("Init complete");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }
    public async UniTask<long> CheckDownloadSizeAsync()
    {
        var handle = Addressables.GetDownloadSizeAsync(labelKeys);
        await handle;
        patchSize = handle.Result;
        Addressables.Release(handle);

        return patchSize;
    }
    public async UniTask<bool> DownloadAsync(ProgressUI _progressUI)
    {
        _progressUI.SetPercentageText(0);

        var handle = Addressables.DownloadDependenciesAsync(labelKeys, Addressables.MergeMode.Union);

        while (!handle.IsDone)
        {
            var status = handle.GetDownloadStatus();

            float percentage = status.TotalBytes > 0 ? (float)status.DownloadedBytes / status.TotalBytes : 0;

            _progressUI.SetSlider(percentage);
            _progressUI.SetPercentageText((int)(percentage * 100));
            _progressUI.SetInfoText(status.DownloadedBytes);

            await UniTask.Delay(100);
        }

        bool success = handle.Status == AsyncOperationStatus.Succeeded;

        Addressables.Release(handle);

        return success;
    }
    public async UniTask CleanBundleCacheAsync()
    {
        AsyncOperationHandle<bool> handle = default;

        try
        {
            handle = Addressables.CleanBundleCache();

            await handle.ToUniTask();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogWarning($"CleanBundleCache failed: {handle.OperationException}");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
    public static string SetFileSizeText(long _size)
    {
        if (_size >= 1073741824.0)
        {
            return $"{_size / 1073741824.0:##.##} GB";
        }
        else if (_size >= 1048576.0)
        {
            return $"{_size / 1048576.0:##.##} MB";
        }
        else if (_size >= 1024.0)
        {
            return $"{_size / 1024.0:##.##} KB";
        }
        else
        {
            return $"{_size} Bytes";
        }
    }
    public bool IsNetworkConnected()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
    #region CustomEditor
    public void AddLabel(string _label)
    {
        Labels.Add(_label);
    }
    public void ClearLabel()
    {
        Labels.Clear();
    }
    #endregion
}
