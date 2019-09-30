// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable RedundantCast

using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib
{
    public sealed class TES4_SNAM : SubRecord 
    {
        public const uint FieldType = 1296125523;
        public string Value;
        internal TES4_SNAM(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            this.Value = dataReader.ReadStringZeroTerminated(0);
        }
        public TES4_SNAM() : base(FieldType)
        {
        }
        public TES4_SNAM(string value) : base(FieldType)
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
