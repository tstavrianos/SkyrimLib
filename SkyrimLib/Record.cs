using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;

namespace SkyrimLib
{
    public sealed class Record : IRecordOrGroup
    {
        public uint Type { get; }
        public uint Size { get; }
        public uint Flags { get; set; }
        public uint Id { get; set; }
        public uint Revision { get; set; }
        public ushort Version { get; set; }
        public ushort Unknown22 { get; }
        public List<SubRecord> Fields { get; }
        private bool Compressed => (this.Flags & 0b00000000000001000000000000000000) != 0;

        public Record(IReader headerReader, IReader dataReader)
        {
            this.Type = headerReader.ReadUInt32(0);
            this.Size = headerReader.ReadUInt32(4);
            this.Flags = headerReader.ReadUInt32(8);
            this.Id = headerReader.ReadUInt32(12);
            this.Revision = headerReader.ReadUInt32(16);
            this.Version = headerReader.ReadUInt16(20);
            this.Unknown22 = headerReader.ReadUInt16(22);

            this.Fields = new List<SubRecord>();

            IReader fields;
            ArrayReader uncompressedReader = null;
            if (this.Compressed)
            {
                var uncompressedLength = dataReader.ReadUInt32(0);
                var compressed = dataReader.ReadBytes(4, (int) this.Size - 4);
                var uncompressed = ZlibStream.UncompressBuffer(compressed);
                if (uncompressedLength != uncompressed.Length)
                    throw new Exception("Decompressed field does not match the stored length");
                uncompressedReader = new ArrayReader(uncompressed, 0, (int) uncompressedLength, false);
                fields = uncompressedReader;
            }
            else
            {
                fields = dataReader;
            }

            uint overrideDataSize = 0;
            while (true)
            {
                if (fields.Length < 4) break;
                var dataSize = fields.ReadUInt16(4);
                var actualSize = overrideDataSize != 0 ? overrideDataSize : dataSize;
                SubRecord field;
                using (var fieldHead = fields.Slice(0, 6))
                {
                    using (var fieldData = fields.Slice(6, (int) actualSize))
                        field = new SubRecord(fieldHead, fieldData, overrideDataSize);
                }

                this.Fields.Add(field);
                overrideDataSize = field.Type == SubRecord.XXXX ? fields.ReadUInt32(6) : 0;
                fields = fields.Slice((int) actualSize + 6);
            }

            if (this.Compressed)
            {
                uncompressedReader.Dispose();
            }

        }

        public void Write(IWriter writer)
        {
            using (var ms = new MemoryStream())
            {
                using (var fieldWriter = new StreamWriter(ms))
                {
                    foreach (var f in this.Fields)
                    {
                        f.Write(fieldWriter);
                    }

                    var dataSize = (uint) ms.Length;
                    byte[] compressed = null;
                    var uncompressed = ms.ToArray();
                    if (this.Compressed)
                    {
                        compressed = ZlibStream.CompressBuffer(uncompressed);
                        dataSize = (uint) compressed.Length + 4;
                    }

                    writer.WriteUInt32(this.Type);
                    writer.WriteUInt32(dataSize);
                    writer.WriteUInt32(this.Flags);
                    writer.WriteUInt32(this.Id);
                    writer.WriteUInt32(this.Revision);
                    writer.WriteUInt16(this.Version);
                    writer.WriteUInt16(this.Unknown22);

                    if (this.Compressed)
                    {
                        writer.WriteUInt32((uint) uncompressed.Length);
                        writer.WriteBytes(compressed);
                    }
                    else
                    {
                        writer.WriteBytes(uncompressed);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach(var child in this.Fields) child.Dispose();
        }
    }
}