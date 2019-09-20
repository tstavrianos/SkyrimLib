namespace SkyrimLib.Generated
{
    public sealed class TES4_CNAM: SubRecord
    {
        public const uint FieldType = 1296125507;
        public string Author;
        public TES4_CNAM(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            this.Author = dataReader.ReadStringZeroTerminated(0);
        }
        
        public TES4_CNAM() : base(FieldType) {}
        
        protected override void WriteData(IWriter writer)
        {
            writer.WriteStringZeroTerminated(this.Author);
        }

        protected override ushort DataLength()
        {
            return (ushort) (this.Author.Length + 1);
        }
    }
}