using System.Collections.Generic;
using System.Linq;

namespace SkyrimLib
{
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
}
