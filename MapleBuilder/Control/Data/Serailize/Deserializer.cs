using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapleBuilder.Control.Data.Serailize;

public class Deserializer
{
    private byte[] bytes;
    private int offset;
    
    internal Deserializer(string path)
    {
        offset = 0;
        try
        {
            bytes = File.ReadAllBytes(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deserializer에서 {path}를 읽는 도중 오류가 발생하였습니다. :: {ex.Message}");
            bytes = Array.Empty<byte>();
        }
    }

    public int GetOffset()
    {
        return offset;
    }

    public byte ReadByte()
    {
        return bytes[offset++];
    }

    public int ReadInt()
    {
        return BitConverter.ToInt32(new[] {ReadByte(), ReadByte(), ReadByte(), ReadByte()});
    }

    public bool ReadBool()
    {
        return ReadByte() == 1;
    }

    public string? ReadString()
    {
        List<byte> encoding = new List<byte>();
        bool connectZero = false;
        byte b;
        while (true)
        {
            b = ReadByte();
            if (b == 0)
            {
                if (connectZero)
                {
                    encoding.RemoveAt(encoding.Count - 1);
                    break;
                }

                connectZero = true;
            }
            else connectZero = false;
            encoding.Add(b);
        }

        return encoding.Count == 0 ? null : Encoding.UTF8.GetString(encoding.ToArray());
    }

    public bool CheckSignature(byte[] signature)
    {
        foreach(byte t in signature)
            if (ReadByte() != t)
                return false;
        return true;
    }
}