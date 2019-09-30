// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable RedundantCast

using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib
{
    public sealed class TES4_MAST : SubRecord 
    {
        public const uint FieldType = 1414742349;
        public string Value;
        internal TES4_MAST(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            this.Value = dataReader.ReadStringZeroTerminated(0);
        }
        public TES4_MAST() : base(FieldType)
        {
        }
        public TES4_MAST(string value) : base(FieldType)
        {
            this.Value = value;
        }
        protected override void WriteData(IWriter writer)
        {
            writer.WriteStringZeroTerminated(this.Value);
        }
        protected override ushort DataLength()
        {
            return (ushort) (this.Value.Length + 1);
        }
    }
}
