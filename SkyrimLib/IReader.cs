using System;

namespace SkyrimLib
{
    public interface IReader: IDisposable
    {
        long Length { get; }
        byte ReadByte(int position);
        byte[] ReadBytes(int position, int count);
        char ReadChar(int position);
        double ReadDouble(int position);
        short ReadInt16(int position);
        long ReadInt64(int position);
        int ReadInt32(int position);
        sbyte ReadSByte(int position);
        float ReadSingle(int position);
        ushort ReadUInt16(int position);
        uint ReadUInt32(int position);
        ulong ReadUInt64(int position);
        void ReadBytes(int position, byte[] buffer, int index, int count);
        string ReadString(int position, int count);
        string ReadStringPrefixLength8(int position);
        string ReadStringPrefixLength16(int position);
        string ReadStringZeroTerminated(int position);

        IReader Slice(int offset);
        IReader Slice(int offset, int length);
        
        ReadOnlySpan<byte> ReadSpan(int position, int length);
    }
}