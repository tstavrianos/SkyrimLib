// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable RedundantCast

using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib
{
    public sealed class TES4_CNAM : SubRecord 
    {
        public const uint FieldType = 1296125507;
        public string Value;
        internal TES4_CNAM(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            this.Value = dataReader.ReadStringZeroTerminated(0);
        }
        public TES4_CNAM() : base(FieldType)
        {
        }
        public TES4_CNAM(string value) : base(FieldType)
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
