using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace SkyrimLib.Generated
{
    public sealed class TES4: Record
    {
        public const uint FieldType = 877872468;
        public TES4_HEDR HEDR { get; }
        public TES4_CNAM CNAM { get; set; }
        public TES4_SNAM SNAM { get; set; }
        public TES4(IReader headerReader, IReader dataReader) : base(headerReader, dataReader)
        {
            this.HEDR = this.Fields.First(x => x.Type == TES4_HEDR.FieldType) as TES4_HEDR;
            this.CNAM = this.Fields.FirstOrDefault(x => x.Type == TES4_CNAM.FieldType) as TES4_CNAM;
            this.SNAM = this.Fields.FirstOrDefault(x => x.Type == TES4_SNAM.FieldType) as TES4_SNAM;
            this.Fields.Clear();
        }

        public TES4() : base(FieldType)
        {
            this.HEDR = new TES4_HEDR();
        }

        protected override IReadOnlyList<SubRecord> GetSubRecordsForWriting()
        {
            var ret = new List<SubRecord>();
            ret.Add(this.HEDR);
            if(this.CNAM != null) ret.Add(this.CNAM);
            if(this.SNAM != null) ret.Add(this.SNAM);
            return ret;
        }
    }
}