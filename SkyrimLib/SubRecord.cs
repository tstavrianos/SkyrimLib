using System;
// ReSharper disable ClassCanBeSealed.Global
namespace SkyrimLib
{
    public class SubRecord: IDisposable
    {
        // ReSharper disable once InconsistentNaming
        public static readonly Signature XXXX = Signature.FromString("XXXX");
        public Signature Type { get; }
        public ushort DataSize { get; private set; }

        internal SubRecord(IReader headerReader, IReader dataReader, uint overrideDataSize = 0)
        {
            this.Type = Signature.Read(0, headerReader);
            this.DataSize = headerReader.ReadUInt16(4);
            var actualSize = overrideDataSize != 0 ? (int) overrideDataSize : this.DataSize;
        }

        protected SubRecord(Signature type)
        {
            this.Type = type;
        }

        internal void Write(IWriter writer)
        {
            writer.WriteBytes(this.Type.Bytes);
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