using System;
namespace SkyrimLib
{
    public sealed class SubRecord: IDisposable
    {
        // ReSharper disable once InconsistentNaming
        public const uint XXXX = 1482184792;
        public uint Type { get; }
        public ushort DataSize { get; }
        public byte[] Data { get; private set; }

        public SubRecord(IReader headerReader, IReader dataReader, uint overrideDataSize = 0)
        {
            this.Type = headerReader.ReadUInt32(0);
            this.DataSize = headerReader.ReadUInt16(4);
            var actualSize = overrideDataSize != 0 ? (int) overrideDataSize : this.DataSize;
            this.Data = new byte[actualSize];
            dataReader.ReadBytes(0, this.Data, 0, actualSize);
        }

        public void Write(IWriter writer)
        {
            writer.WriteUInt32(this.Type);
            writer.WriteUInt16((ushort)this.Data.Length);
            writer.WriteBytes(this.Data);
        }

        public void Dispose()
        {
            this.Data = null;
        }
    }
}