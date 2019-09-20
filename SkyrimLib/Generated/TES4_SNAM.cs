namespace SkyrimLib.Generated
{
    public sealed class TES4_SNAM: SubRecord
    {
        public const uint FieldType = 1296125523;
        public string Description;
        public TES4_SNAM(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            this.Description = dataReader.ReadStringZeroTerminated(0);
        }
        
        public TES4_SNAM() : base(FieldType) {}

        protected override void WriteData(IWriter writer)
        {
            writer.WriteStringZeroTerminated(this.Description);
        }

        protected override ushort DataLength()
        {
            return (ushort) (this.Description.Length + 1);
        }
    }
}