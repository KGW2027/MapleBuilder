using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapleBuilder.Control.Data.Serailize;



interface IWzSerializable
{
    void Serialize(WzSerializer serializer);
}

public class WzSerializer : LinkedList<byte>
{
    internal WzSerializer()
    {
    }

    internal void InsertSignature(byte[] signature)
    {
        for (int idx = signature.Length - 1; idx >= 0; idx--)
            AddFirst(signature[idx]);
    }

    internal void SerializeIntFirst(int num)
    {
        for (int shift = 24; shift >= 0; shift -= 8)
            AddFirst((byte) ((num >> shift) & 0xFF));
    }

    internal void SerializeByte(byte num)
    {
        AddLast(num);
    }

    internal void SerializeInt(int num)
    {
        for (int shift = 0; shift < 32; shift += 8)
            AddLast((byte) ((num >> shift) & 0xFF));
    }

    internal void SerializeString(string? str)
    {
        if (str != null)
        {
            foreach (byte b in Encoding.UTF8.GetBytes(str))
                AddLast(b);
        }
        AddLast(0);
        AddLast(0);
    }

    internal void Save(string path)
    {
        using FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        foreach (byte b in this)
            fs.WriteByte(b);
    }

    public static WzSerializer operator +(WzSerializer lhs, WzSerializer rhs)
    {
        foreach (byte data in rhs)
            lhs.AddLast(data);
        return lhs;
    }
}

public class WzDeserializer : Deserializer
{
    internal WzDeserializer(string path) : base(path)
    {
    }
}
