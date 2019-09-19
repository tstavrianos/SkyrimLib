using System;

namespace SkyrimLib
{
    public interface IWriter: IDisposable
    {
        long Position { get; }
        void WriteBool(bool value);
        void WriteByte(byte value);
        void WriteBytes(byte[] buffer);
        void WriteBytes(byte[] buffer, int index, int count);
        void WriteChar(char ch);
        void WriteDecimal(decimal value);
        void WriteDouble(double value);
        void WriteInt16(short value);
        void WriteInt32(int value);
        void WriteInt64(long value);
        void WriteSByte(sbyte value);
        void WriteSingle(float value);
        void WriteUInt16(ushort value);
        void WriteUInt32(uint value);
        void WriteUInt64(ulong value);
        void WriteStringZeroTerminated(string value);
    }
}