using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib
{
    public class TES4_DATA : SubRecord 
    {
        public const uint FieldType = 1096040772;
        public uint Value;
        internal TES4_DATA(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            this.Value = dataReader.ReadUInt32(0);
        }
        public TES4_DATA() : base(FieldType)
        {
        }
        public TES4_DATA(uint value) : base(FieldType)
        {
            this.Value = value;
        }
        protected override void WriteData(IWriter writer)
        {
            writer.WriteUInt32(this.Value);
        }
        protected override ushort DataLength()
        {
            return (ushort) (4);
        }
    }
}
