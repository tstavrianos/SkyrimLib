// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable RedundantCast

using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib
{
    public sealed class TES4 : Record 
    {
        public const uint FieldType = 877872468;
        public readonly TES4_HEDR HEDR = new TES4_HEDR();
        public TES4_CNAM CNAM;
        public TES4_SNAM SNAM;
        public readonly List<TES4_MAST> MAST = new List<TES4_MAST>();
        public readonly List<TES4_DATA> DATA = new List<TES4_DATA>();
        public readonly TES4_ONAM ONAM = new TES4_ONAM();
        public readonly TES4_INTV INTV = new TES4_INTV();
        public TES4_INCC INCC;
        internal TES4(IReader headerReader, IReader dataReader) : base(headerReader, dataReader)
        {
            this.HEDR = this.Fields.First(x => x.Type == TES4_HEDR.FieldType) as TES4_HEDR;
            this.CNAM = this.Fields.FirstOrDefault(x => x.Type == TES4_CNAM.FieldType) as TES4_CNAM;
            this.SNAM = this.Fields.FirstOrDefault(x => x.Type == TES4_SNAM.FieldType) as TES4_SNAM;
            this.MAST.AddRange(this.Fields.Where(x => x.Type == TES4_MAST.FieldType).Cast<TES4_MAST>());
            this.DATA.AddRange(this.Fields.Where(x => x.Type == TES4_DATA.FieldType).Cast<TES4_DATA>());
            this.ONAM = this.Fields.First(x => x.Type == TES4_ONAM.FieldType) as TES4_ONAM;
            this.INTV = this.Fields.First(x => x.Type == TES4_INTV.FieldType) as TES4_INTV;
            this.INCC = this.Fields.FirstOrDefault(x => x.Type == TES4_INCC.FieldType) as TES4_INCC;
            this.Fields.Clear();
        }
        public TES4() : base(FieldType)
        {
        }
        protected override IReadOnlyList<SubRecord> GetSubRecordsForWriting()
        {
            var ret = new List<SubRecord>();
            ret.Add(this.HEDR);
            if(this.CNAM != null) ret.Add(this.CNAM);
            if(this.SNAM != null) ret.Add(this.SNAM);
            while(this.DATA.Count > this.MAST.Count) this.DATA.RemoveAt(this.DATA.Count - 1);
            while(this.DATA.Count < this.MAST.Count) this.DATA.Add(new TES4_DATA());
            for(var i = 0; i < this.MAST.Count; i++){
                ret.Add(this.MAST[i]);
                ret.Add(this.DATA[i]);
            }
            if(this.ONAM.Values.Count > 0) ret.Add(this.ONAM);
            ret.Add(this.INTV);
            if(this.INCC != null) ret.Add(this.INCC);
            return ret;
        }
    }
}
