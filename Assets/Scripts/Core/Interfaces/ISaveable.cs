using System.Threading;
using Cysharp.Threading.Tasks;

public interface ISaveable
{
    string SaveKey { get; }
    bool IsDirty { get; }

    UniTask SaveFileAsync(CancellationToken ct = default);
    UniTask<bool> LoadFileAsync(CancellationToken ct = default);

    // OnApplicationPause 등에서 await 없이 즉시 끝까지 기록해야 할 때 사용
    void SaveFileSyncOnQuit();

    void ClearDirty();
}