using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BinaryUtility
{
    public static void Write(BinaryWriter writer, int value)
    {
        writer.Write(value);
    }

    public static void Write(BinaryWriter writer, long value)
    {
        writer.Write(value);
    }

    public static void Write(BinaryWriter writer, bool value)
    {
        writer.Write(value);
    }

    public static void Write(BinaryWriter writer, float value)
    {
        writer.Write(value);
    }

    public static void Write(BinaryWriter writer, string value)
    {
        writer.Write(value);
    }

    public static void Write(BinaryWriter writer, Vector2 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
    }

    public static void Write(BinaryWriter writer, Vector3 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
    }

    public static void Write(BinaryWriter writer, int[] array)
    {
        writer.Write(array.Length);

        for (int i = 0; i < array.Length; i++)
        {
            writer.Write(array[i]);
        }
    }

    public static void Write(BinaryWriter writer, bool[] array)
    {
        writer.Write(array.Length);

        for (int i = 0; i < array.Length; i++)
        {
            writer.Write(array[i]);
        }
    }

    public static void Write(BinaryWriter writer, List<int> list)
    {
        writer.Write(list.Count);

        for (int i = 0; i < list.Count; i++)
        {
            writer.Write(list[i]);
        }
    }

    public static void Write(BinaryWriter writer, List<bool> list)
    {
        writer.Write(list.Count);

        for (int i = 0; i < list.Count; i++)
        {
            writer.Write(list[i]);
        }
    }

    public static int ReadInt(BinaryReader reader)
    {
        return reader.ReadInt32();
    }

    public static long ReadLong(BinaryReader reader)
    {
        return reader.ReadInt64();
    }

    public static bool ReadBool(BinaryReader reader)
    {
        return reader.ReadBoolean();
    }

    public static float ReadFloat(BinaryReader reader)
    {
        return reader.ReadSingle();
    }

    public static string ReadString(BinaryReader reader)
    {
        return reader.ReadString();
    }
    public static Vector2 ReadVector2(BinaryReader reader)
    {
        return new Vector2(
            reader.ReadSingle(),
            reader.ReadSingle());
    }
    public static Vector3 ReadVector3(BinaryReader reader)
    {
        return new Vector3(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle());
    }

    public static void ReadInto(BinaryReader reader, List<int> list)
    {
        int count = reader.ReadInt32();

        list.Clear();

        if (list.Capacity < count)
        {
            list.Capacity = count;
        }

        for (int i = 0; i < count; i++)
        {
            list.Add(reader.ReadInt32());
        }
    }
    public static void ReadInto(BinaryReader reader, List<bool> list)
    {
        int count = reader.ReadInt32();

        list.Clear();

        if (list.Capacity < count)
        {
            list.Capacity = count;
        }

        for (int i = 0; i < count; i++)
        {
            list.Add(reader.ReadBoolean());
        }
    }

    public static void ReadInto(BinaryReader reader, int[] array)
    {
        int length = reader.ReadInt32();

        if (array.Length != length)
        {
            throw new InvalidDataException(
                $"Array length mismatch. Expected: {array.Length}, Actual: {length}");
        }

        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadInt32();
        }
    }

    public static void ReadInto(BinaryReader reader, bool[] array)
    {
        int length = reader.ReadInt32();

        if (array.Length != length)
        {
            throw new InvalidDataException(
                $"Array length mismatch. Expected: {array.Length}, Actual: {length}");
        }

        for (int i = 0; i < length; i++)
        {
            array[i] = reader.ReadBoolean();
        }
    }
}