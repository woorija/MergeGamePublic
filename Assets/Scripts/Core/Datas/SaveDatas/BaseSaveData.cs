using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public abstract class BaseSaveData : ISaveable, IDisposable
{
    protected byte[] buffer;
    protected readonly MemoryStream stream;
    protected readonly BinaryWriter writer;
    protected readonly BinaryReader reader;
    protected readonly string path;
    protected readonly string tempPath;
    protected int dataVersion;

    private readonly SemaphoreSlim ioLock = new SemaphoreSlim(1, 1);
    private bool isDirty;

    public string SaveKey { get; }
    public bool IsDirty => isDirty;

    protected BaseSaveData(string path, int bufferSize)
    {
        this.path = path;
        tempPath = $"{path}.tmp";
        SaveKey = Path.GetFileNameWithoutExtension(path);

        buffer = new byte[bufferSize];
        stream = new MemoryStream(buffer, true);
        writer = new BinaryWriter(stream);
        reader = new BinaryReader(stream);
    }
    protected void Padding()
    {
        while (stream.Position % 4 != 0)
        {
            writer.Write((byte)0);
        }
    }
    protected void ResetStream()
    {
        stream.Position = 0;
    }
    public void MarkDirty()
    {
        isDirty = true;
        SaveManager.Instance.NotifyDirty();
    }
    public void ClearDirty()
    {
        isDirty = false;
    }
    public async UniTask<bool> LoadFileAsync(CancellationToken ct = default)
    {
        await ioLock.WaitAsync(ct);

        try
        {
            bool mainExists = File.Exists(path);

            if (mainExists)
            {
                if (await TryLoadFromFileAsync(path, ct))
                {
                    return true;
                }

                Debug.LogWarning($"[SaveData] 메인 파일 로드 실패, tmp 복구 확인: {path}");
            }

            if (File.Exists(tempPath))
            {
                Debug.LogWarning($"[SaveData] tmp 파일 복구 시도: {path}");

                if (await TryLoadFromFileAsync(tempPath, ct))
                {
                    Debug.Log($"[SaveData] tmp 파일에서 복구 성공: {path}");
                    TryRestoreFromTemp();
                    return true;
                }

                Debug.LogError($"[SaveData] tmp 파일 로드 실패: {tempPath}");
            }

            if (!mainExists)
            {
                Debug.Log($"[SaveData] 세이브 파일이 없어 최초 데이터를 사용합니다: {path}");
            }
            else
            {
                Debug.LogError($"[SaveData] 로드 및 복구 모두 실패, 데이터 유실 가능성: {path}");
            }

            ResetStream();
            return false;
        }
        finally
        {
            ioLock.Release();
        }
    }
    private async UniTask<bool> TryLoadFromFileAsync(string filePath, CancellationToken ct)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                int length = (int)fs.Length;
                if (length > buffer.Length)
                {
                    throw new InvalidDataException($"저장 파일 크기({length})가 버퍼 크기({buffer.Length})를 초과했습니다: {filePath}");
                }

                int readTotal = 0;

                while (readTotal < length)
                {
                    int read = await fs.ReadAsync(buffer, readTotal, length - readTotal, ct);
                    if (read == 0)
                    {
                        break;
                    }
                    readTotal += read;
                }

                EncryptionUtility.DecryptXXTEA(buffer, length);
            }

            ResetStream();
            Deserialize();
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveData] 파일 읽기/파싱 실패: {filePath}\n{e}");
            return false;
        }
    }
    public async UniTask SaveFileAsync(CancellationToken ct = default)
    {
        await ioLock.WaitAsync(ct);

        try
        {
            ResetStream();
            Serialize();
            Padding();

            int length = (int)stream.Position;

            if (length > buffer.Length)
            {
                throw new InvalidOperationException("저장할 데이터가 버퍼 크기를 초과했습니다.");
            }

            EncryptionUtility.EncryptXXTEA(buffer, length);

            using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                await fs.WriteAsync(buffer, 0, length, ct);
                await fs.FlushAsync(ct);
            }

            ReplaceAtomically();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            // 버퍼 크기 초과는 설계/사이징 오류이므로 조용히 삼키지 않고 호출자에게 전파.
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveData] Save 실패: {path}\n{e}");
        }
        finally
        {
            ioLock.Release();
        }
    }
    public void SaveFileSyncOnQuit()
    {
        if (!ioLock.Wait(0))
        {
            // 이미 비동기 저장이 진행 중이면 그 저장이 책임진다.
            return;
        }

        try
        {
            ResetStream();
            Serialize();
            Padding();

            int length = (int)stream.Position;

            if (length > buffer.Length)
            {
                Debug.LogError($"[SaveData] 종료 시점 저장 실패 - 버퍼 크기 초과: {path}");
                return;
            }

            EncryptionUtility.EncryptXXTEA(buffer, length);

            using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.Write(buffer, 0, length);
                fs.Flush(true);
            }

            ReplaceAtomically();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveData] 종료 시점 동기 저장 실패: {path}\n{e}");
        }
        finally
        {
            ioLock.Release();
        }
    }
    private void ReplaceAtomically()
    {
        if (File.Exists(path))
        {
            File.Replace(tempPath, path, null);
        }
        else
        {
            File.Move(tempPath, path);
        }
    }
    private void TryRestoreFromTemp()
    {
        if (!File.Exists(tempPath))
        {
            return;
        }

        try
        {
            File.Copy(tempPath, path, true);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveData] 임시 파일 복구 실패, 다음 저장에서 새로 생성됨: {path}\n{e}");
        }
    }
    protected abstract void Serialize();
    protected abstract void Deserialize();
    protected void Save(int value)
    {
        BinaryUtility.Write(writer, value);
    }
    protected void Save(long value)
    {
        BinaryUtility.Write(writer, value);
    }
    protected void Save(bool value)
    {
        BinaryUtility.Write(writer, value);
    }
    protected void Save(float value)
    {
        BinaryUtility.Write(writer, value);
    }
    protected void Save(string value)
    {
        BinaryUtility.Write(writer, value);
    }
    protected void Save(Vector2 value)
    {
        BinaryUtility.Write(writer, value);
    }
    protected void Save(Vector3 value)
    {
        BinaryUtility.Write(writer, value);
    }
    protected void Save(int[] array)
    {
        BinaryUtility.Write(writer, array);
    }
    protected void Save(bool[] array)
    {
        BinaryUtility.Write(writer, array);
    }
    protected void Save(List<int> list)
    {
        BinaryUtility.Write(writer, list);
    }
    protected void Save(List<bool> list)
    {
        BinaryUtility.Write(writer, list);
    }
    protected int LoadInt()
    {
        return BinaryUtility.ReadInt(reader);
    }
    protected long LoadLong()
    {
        return BinaryUtility.ReadLong(reader);
    }
    protected bool LoadBool()
    {
        return BinaryUtility.ReadBool(reader);
    }
    protected float LoadFloat()
    {
        return BinaryUtility.ReadFloat(reader);
    }
    protected string LoadString()
    {
        return BinaryUtility.ReadString(reader);
    }
    protected Vector2 LoadVector2()
    {
        return BinaryUtility.ReadVector2(reader);
    }
    protected Vector3 LoadVector3()
    {
        return BinaryUtility.ReadVector3(reader);
    }
    protected void LoadInto(List<int> list)
    {
        BinaryUtility.ReadInto(reader, list);
    }
    protected void LoadInto(List<bool> list)
    {
        BinaryUtility.ReadInto(reader, list);
    }
    protected void LoadInto(int[] array)
    {
        BinaryUtility.ReadInto(reader, array);
    }
    protected void LoadInto(bool[] array)
    {
        BinaryUtility.ReadInto(reader, array);
    }
    public void Dispose()
    {
        writer?.Dispose();
        reader?.Dispose();
        stream?.Dispose();
        ioLock?.Dispose();
        GC.SuppressFinalize(this);
    }
}