// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable RedundantCast
using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib
{
    public sealed class TES4_DATA : SubRecord 
    {
        public static readonly Signature FieldType = Signature.FromString("DATA");
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
