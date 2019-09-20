namespace SkyrimLib.Generated
{
    public sealed class TES4_HEDR: SubRecord
    {
        public const uint FieldType = 1380205896;
        public float Version;
        public int NumRecords;
        public ulong NextObjectId;
        
        public TES4_HEDR(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            this.Version = dataReader.ReadSingle(0);
            this.NumRecords = dataReader.ReadInt32(4);
            this.NextObjectId = dataReader.ReadUInt64(8);
        }
        
        public TES4_HEDR() : base(FieldType) {}
        
        protected override void WriteData(IWriter writer)
        {
            writer.WriteSingle(this.Version);
            writer.WriteInt32(this.NumRecords);
            writer.WriteUInt64(this.NextObjectId);
        }

        protected override ushort DataLength()
        {
            return 4 + 4 + 8;
        }
    }
}