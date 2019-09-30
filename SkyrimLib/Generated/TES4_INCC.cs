// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable RedundantCast

using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib
{
    public sealed class TES4_INCC : SubRecord 
    {
        public const uint FieldType = 1128484425;
        public uint Value;
        internal TES4_INCC(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            this.Value = dataReader.ReadUInt32(0);
        }
        public TES4_INCC() : base(FieldType)
        {
        }
        public TES4_INCC(uint value) : base(FieldType)
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
