// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable RedundantCast
using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib
{
    public sealed class TES4_ONAM : SubRecord 
    {
        public static readonly Signature FieldType = Signature.FromString("ONAM");
        public readonly List<uint> Values = new List<uint>();
        internal TES4_ONAM(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            var idx = 0;
            while (dataReader.Length > idx + 4){
                this.Values.Add(dataReader.ReadUInt32(idx));
                idx += 4;
            }
        }
        internal TES4_ONAM() : base(FieldType)
        {
        }
        protected override void WriteData(IWriter writer)
        {
            foreach(var value in this.Values) writer.WriteUInt32(value);
        }
        protected override ushort DataLength()
        {
            return (ushort) (this.Values.Count * 4);
        }
    }
}
