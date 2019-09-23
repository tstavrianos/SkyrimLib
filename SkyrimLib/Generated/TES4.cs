// ReSharper disable RedundantUsingDirective
// ReSharper disable RedundantCast
// ReSharper disable InvertIf
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable UseObjectOrCollectionInitializer
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib.Generated
{
    public class TES4 : Record 
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
    public class TES4_HEDR : SubRecord 
    {
        public const uint FieldType = 1380205896;
        public float version;
        public int numRecords;
        public uint nextObjectId;
        internal TES4_HEDR(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            var idx = 0;
            this.version = dataReader.ReadSingle(idx);
            idx += 4;
            this.numRecords = dataReader.ReadInt32(idx);
            idx += 4;
            this.nextObjectId = dataReader.ReadUInt32(idx);
            idx += 4;
        }
        internal TES4_HEDR() : base(FieldType)
        {
        }
        protected override void WriteData(IWriter writer)
        {
            writer.WriteSingle(this.version);
            writer.WriteInt32(this.numRecords);
            writer.WriteUInt32(this.nextObjectId);
        }
        protected override ushort DataLength()
        {
            return (ushort) (4 + 4 + 4);
        }
    }
    public class TES4_CNAM : SubRecord 
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
    public class TES4_SNAM : SubRecord 
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
    public class TES4_MAST : SubRecord 
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
    public class TES4_ONAM : SubRecord 
    {
        public const uint FieldType = 1296125519;
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
    public class TES4_INTV : SubRecord 
    {
        public const uint FieldType = 1448365641;
        public uint Value;
        internal TES4_INTV(IReader headerReader, IReader dataReader, uint overrideDataSize = 0) : base(headerReader, dataReader, overrideDataSize)
        {
            this.Value = dataReader.ReadUInt32(0);
        }
        internal TES4_INTV() : base(FieldType)
        {
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
    public class TES4_INCC : SubRecord 
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
