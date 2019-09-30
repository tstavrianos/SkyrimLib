using System;
// ReSharper disable ClassCanBeSealed.Global
namespace SkyrimLib
{
    public class SubRecord: IDisposable
    {
        // ReSharper disable once InconsistentNaming
        public const uint XXXX = 1482184792;
        public uint Type { get; }
        public ushort DataSize { get; private set; }

        internal SubRecord(IReader headerReader, IReader dataReader, uint overrideDataSize = 0)
        {
            this.Type = headerReader.ReadUInt32(0);
            this.DataSize = headerReader.ReadUInt16(4);
            var actualSize = overrideDataSize != 0 ? (int) overrideDataSize : this.DataSize;
        }

        protected SubRecord(uint type)
        {
            this.Type = type;
        }

        internal void Write(IWriter writer)
        {
            writer.WriteUInt32(this.Type);
            this.DataSize = this.DataLength();
            writer.WriteUInt16(this.DataSize);
            this.WriteData(writer);
        }

        protected virtual ushort DataLength()
        {
            return 0;
        }

        protected virtual void WriteData(IWriter writer)
        {
            
        }
        

        public virtual void Dispose()
        {
        }
    }
}