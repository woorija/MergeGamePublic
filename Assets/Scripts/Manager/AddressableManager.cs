using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : DontDestroySingletonBehaviour<AddressableManager>
{
    /// <summary>키 하나에 대한 핸들 / 인스턴스 / 해제 예약 상태를 한 곳에서 관리</summary>
    sealed class AssetEntry
    {
        public AsyncOperationHandle Handle;
        public List<GameObject> Instances;   // 필요할 때만 생성
        public bool PendingRelease;          // 로딩 중 해제 요청이 들어온 경우
    }

    // scope(enum 값)를 인덱스로 사용. 새 scope를 enum에 추가하면 자동으로 딕셔너리가 하나 늘어남
    readonly Dictionary<string, AssetEntry>[] entriesByScope = CreateEntryTable();
    readonly List<string> keyBuffer = new List<string>(64);   // 순회 중 수정 방지용 재사용 버퍼

    static Dictionary<string, AssetEntry>[] CreateEntryTable()
    {
        int max = 0;
        foreach (AddressableAssetScope scope in Enum.GetValues(typeof(AddressableAssetScope)))
        {
            max = Math.Max(max, (int)scope);
        }

        var table = new Dictionary<string, AssetEntry>[max + 1];
        for (int i = 0; i < table.Length; i++)
        {
            table[i] = new Dictionary<string, AssetEntry>();
        }
        return table;
    }

    Dictionary<string, AssetEntry> GetEntries(AddressableAssetScope _scope)
    {
        return entriesByScope[(int)_scope];
    }

    // ───────────────────────── Load ─────────────────────────

    public void LoadAsset<T>(string _key, Action<T> _onComplete)
    {
        LoadWithCallback(_key, AddressableAssetScope.Global, _onComplete);
    }

    public void LoadLocalSceneAsset<T>(string _key, Action<T> _onComplete)
    {
        LoadWithCallback(_key, AddressableAssetScope.Local, _onComplete);
    }

    void LoadWithCallback<T>(string _key, AddressableAssetScope _scope, Action<T> _onComplete)
    {
        if (TryGetLoaded<T>(_key, _scope, out var asset))
        {
            _onComplete?.Invoke(asset);
            return;
        }
        LoadAssetWithAction(_key, _scope, _onComplete).Forget();
    }

    async UniTaskVoid LoadAssetWithAction<T>(string _key, AddressableAssetScope _scope, Action<T> _onComplete)
    {
        try
        {
            var result = await LoadAssetAsync<T>(_key, _scope);
            _onComplete?.Invoke(result);
        }
        catch (Exception e)
        {
            DevelopUtility.Log($"LoadAssetWithAction 실패 - Key:{_key}, Error:{e.Message}");
        }
    }

    public async UniTask<T> LoadAssetAsync<T>(string _key, AddressableAssetScope _scope, CancellationToken cancellationToken = default)
    {
        var entries = GetEntries(_scope);

        if (entries.TryGetValue(_key, out var entry))
        {
            if (entry.Handle.IsValid() && entry.Handle.Status != AsyncOperationStatus.Failed)
            {
                // 로딩 중이든 완료든 같은 핸들을 공유. 해제 예약이 있었다면 다시 사용하는 것이므로 취소
                entry.PendingRelease = false;
            }
            else
            {
                // 실패했거나 이미 무효화된 핸들 -> 정리 후 재시도
                DevelopUtility.Log($"어드레서블로딩실패 - Key:{_key}");
                RemoveAndRelease(_key, entries, entry);
                entry = null;
            }
        }

        if (entry == null)
        {
            entry = StartLoad<T>(_key, entries);
        }

        return await WaitForResultAsync<T>(_key, entry, cancellationToken);
    }

    AssetEntry StartLoad<T>(string _key, Dictionary<string, AssetEntry> _entries)
    {
        var handle = Addressables.LoadAssetAsync<T>(_key);
        var entry = new AssetEntry { Handle = handle };
        _entries.Add(_key, entry);

        // 로딩 후 처리(실패 정리 / 해제 예약 처리)는 생성자 한 곳에서만 담당
        handle.Completed += _ => FinalizeLoadAsync(_key, _entries, entry).Forget();
        return entry;
    }

    /// <summary>
    /// 로딩 완료 후 한 프레임 뒤에 실행.
    /// 같은 프레임에 await 중이던 호출자들이 모두 결과를 받은 뒤에 해제하기 위함.
    /// </summary>
    async UniTaskVoid FinalizeLoadAsync(string _key, Dictionary<string, AssetEntry> _entries, AssetEntry _entry)
    {
        await UniTask.Yield();

        // 이미 해제되었거나 같은 키로 다시 만들어진 엔트리면 무시
        if (!_entries.TryGetValue(_key, out var current) || current != _entry) return;

        if (!_entry.Handle.IsValid() || _entry.Handle.Status == AsyncOperationStatus.Failed)
        {
            DevelopUtility.Log($"어드레서블로딩실패 - Key:{_key}");
            RemoveAndRelease(_key, _entries, _entry);
            return;
        }

        if (_entry.PendingRelease)
        {
            DevelopUtility.Log($"{_key}에 대한 릴리즈 확인 및 실행");
            RemoveAndRelease(_key, _entries, _entry);
        }
    }

    async UniTask<T> WaitForResultAsync<T>(string _key, AssetEntry _entry, CancellationToken _ct)
    {
        var handle = _entry.Handle;

        if (!handle.IsDone)
        {
            try
            {
                // 취소는 '대기'만 취소. 핸들은 다른 대기자가 쓸 수 있으므로 건드리지 않음
                await handle.WithCancellation(_ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                // 실패 여부는 아래에서 Status로 판단
                DevelopUtility.Log($"어드레서블 대기 중 예외 - Key:{_key}, Error:{e.Message}");
            }
        }

        if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
        {
            throw new Exception($"{_key} 로딩이 실패했습니다.");
        }

        if (handle.Result is not T result)
        {
            throw new InvalidCastException($"Type mismatch: key={_key}, result={handle.Result?.GetType()}, requested={typeof(T)}");
        }

        return result;
    }

    bool TryGetLoaded<T>(string _key, AddressableAssetScope _scope, out T _asset)
    {
        _asset = default;

        if (!GetEntries(_scope).TryGetValue(_key, out var entry)) return false;

        var handle = entry.Handle;
        if (!handle.IsValid()) return false;
        if (handle.Status != AsyncOperationStatus.Succeeded) return false;

        if (handle.Result is not T typedResult)
        {
            DevelopUtility.Log($"Type mismatch: key={_key}, result={handle.Result?.GetType()}, requested={typeof(T)}");
            return false;
        }

        entry.PendingRelease = false;
        _asset = typedResult;
        return true;
    }

    // ───────────────────────── Instantiate ─────────────────────────

    public UniTask<GameObject> InstantiateAsync(string _key)
    {
        return InstantiateInternalAsync(_key, AddressableAssetScope.Local);
    }

    public UniTask<GameObject> InstantiateGlobalAsync(string _key)
    {
        return InstantiateInternalAsync(_key, AddressableAssetScope.Global);
    }

    async UniTask<GameObject> InstantiateInternalAsync(string _key, AddressableAssetScope _scope)
    {
        var prefab = await LoadAssetAsync<GameObject>(_key, _scope);

        if (prefab == null)
        {
            DevelopUtility.Log($"{_key}의 프리팹 로딩 실패");
            return null;
        }

        // 대기하는 사이 해제된 경우, 해제된 프리팹으로 Instantiate하지 않도록 방어
        if (!GetEntries(_scope).TryGetValue(_key, out var entry))
        {
            DevelopUtility.Log($"{_key}가 로딩 중 해제되어 Instantiate를 취소");
            return null;
        }

        var instance = Instantiate(prefab);
        (entry.Instances ??= new List<GameObject>(8)).Add(instance);
        return instance;
    }

    // ───────────────────────── Release ─────────────────────────

    public void ReleaseCurrentSceneAssets()
    {
        ReleaseScopeAssets(AddressableAssetScope.Local);
    }

    public void ReleaseGlobalAssets()
    {
        ReleaseScopeAssets(AddressableAssetScope.Global);
    }

    public void ReleaseAllAssets()
    {
        for (int i = 0; i < entriesByScope.Length; i++)
        {
            ReleaseScopeAssets((AddressableAssetScope)i);
        }
    }

    public void ReleaseScopeAssets(AddressableAssetScope _scope)
    {
        var entries = GetEntries(_scope);
        if (entries.Count == 0) return;

        keyBuffer.Clear();
        keyBuffer.AddRange(entries.Keys);
        for (int i = 0; i < keyBuffer.Count; i++)
        {
            ReleaseEntry(keyBuffer[i], entries);
        }
        keyBuffer.Clear();
    }

    public void ReleaseAsset(string _key)
    {
        for (int i = 0; i < entriesByScope.Length; i++)
        {
            var entries = entriesByScope[i];
            if (entries.ContainsKey(_key))
            {
                ReleaseEntry(_key, entries);
                return;
            }
        }

        DevelopUtility.Log($"{_key}키가 없음");
    }

    void ReleaseEntry(string _key, Dictionary<string, AssetEntry> _entries)
    {
        if (!_entries.TryGetValue(_key, out var entry)) return;

        // 로딩 중이면 지금 해제하지 않고 예약만 걸어둠 (완료 후 FinalizeLoadAsync에서 처리)
        if (entry.Handle.IsValid() && !entry.Handle.IsDone)
        {
            entry.PendingRelease = true;
            return;
        }

        RemoveAndRelease(_key, _entries, entry);
    }

    void RemoveAndRelease(string _key, Dictionary<string, AssetEntry> _entries, AssetEntry _entry)
    {
        _entries.Remove(_key);

        var instances = _entry.Instances;
        if (instances != null)
        {
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] != null)
                {
                    Destroy(instances[i]);
                }
            }
            instances.Clear();
            _entry.Instances = null;
        }

        if (_entry.Handle.IsValid())
        {
            DevelopUtility.Log($"Release:{_key}");
            Addressables.Release(_entry.Handle);
        }
    }
}