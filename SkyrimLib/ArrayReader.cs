using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SkyrimLib
{
    public sealed class ArrayReader: IReader
    {
        private readonly ReadonlyArrayWrapper<byte> _data;
        private readonly bool _rented;

        public ArrayReader(ReadonlyArrayWrapper<byte> data)
        {
            this._data = data;
            this.Length = data.Length;
            this._rented = data.Rented;
        }

        public ArrayReader(byte[] data, int offset, int length, bool rented = false)
        {
            this._data = new ReadonlyArrayWrapper<byte>(data, offset, length);
            this.Length = length;
            this._rented = rented;
        }
        
        public long Length { get; }

        private T ReadInternal<T>(int position) where T : unmanaged
        {
            return Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(this._data.Span));
        }
        
        public byte ReadByte(int position) => this._data.Span[position];

        public byte[] ReadBytes(int position, int count)
        {
            var ret = ArrayPool<byte>.Shared.Rent(count);
            this._data.Span.Slice(position, count).CopyTo(ret);
            return ret;
        }

        public char ReadChar(int position) => this.ReadInternal<char>(position);

        public double ReadDouble(int position) => this.ReadInternal<double>(position);

        public short ReadInt16(int position) => this.ReadInternal<short>(position);

        public long ReadInt64(int position) => this.ReadInternal<long>(position);

        public sbyte ReadSByte(int position) => (sbyte)this.ReadByte(position);

        public float ReadSingle(int position) => this.ReadInternal<float>(position);

        public ushort ReadUInt16(int position) => this.ReadInternal<ushort>(position);

        public uint ReadUInt32(int position) => this.ReadInternal<uint>(position);

        public ulong ReadUInt64(int position) => this.ReadInternal<ulong>(position);

        public int ReadInt32(int position) => this.ReadInternal<int>(position);

        public void ReadBytes(int position, byte[] buffer, int index, int count) => this._data.Span.Slice(position, count).CopyTo(buffer.AsSpan(index));

        public string ReadString(int position, int count) => Encoding.ASCII.GetString(this._data.Span.Slice(position, count));

        public string ReadStringPrefixLength8(int position)
        {
            var length = this.ReadByte(position);
            return this.ReadString(position + 1, length);
        }

        public string ReadStringPrefixLength16(int position)
        {
            var length = this.ReadUInt16(position);
            return this.ReadString(position + 2, length);
        }

        public string ReadStringZeroTerminated(int position)
        {
            var i = 0;
            while(this.ReadByte(position + i) != '\0')
            {
                i++;
            }

            return i <= 0 ? string.Empty : this.ReadString(position, i);
        }

        public IReader Slice(int offset)
        {
            return this.Slice(offset, (int)(this.Length - offset));
        }

        public IReader Slice(int offset, int length)
        {
            return new ArrayReader(new ReadonlyArrayWrapper<byte>(this._data, offset, length));
        }

        public ReadOnlySpan<byte> ReadSpan(int position, int length) => this._data.Span.Slice(position, length);

        public void Dispose()
        {
            if(this._rented) ArrayPool<byte>.Shared.Return(this._data.Array);
        }
    }
}