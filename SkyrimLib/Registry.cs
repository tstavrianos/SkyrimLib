using System;
using System.Collections.Generic;
using SkyrimLib.Generated;

namespace SkyrimLib
{
    internal static class Registry
    {
        internal static readonly IReadOnlyDictionary<uint, Func<IReader, IReader, Record>> ParsedRecords;
        internal static readonly IReadOnlyDictionary<(uint record, uint subrecord), Func<IReader, IReader, uint, SubRecord>> ParsedSubRecords;

        static Registry()
        {
            ParsedRecords = new Dictionary<uint, Func<IReader, IReader, Record>>
            {
                {TES4.FieldType, (reader, reader1) => new TES4(reader, reader1)},
            };
            ParsedSubRecords = new Dictionary<(uint record, uint subrecord), Func<IReader, IReader, uint, SubRecord>>
            {
                {(TES4.FieldType, TES4_CNAM.FieldType), (reader, reader1, size) => new TES4_CNAM(reader, reader1, size)},
                {(TES4.FieldType, TES4_HEDR.FieldType), (reader, reader1, size) => new TES4_HEDR(reader, reader1, size)},
                {(TES4.FieldType, TES4_SNAM.FieldType), (reader, reader1, size) => new TES4_SNAM(reader, reader1, size)},
            };
        }
    }
}