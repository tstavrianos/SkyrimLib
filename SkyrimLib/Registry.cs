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
                {TES4.FieldType, (headerReader, dataReader) => new TES4(headerReader, dataReader)}
            };
            ParsedSubRecords = new Dictionary<(uint record, uint subrecord), Func<IReader, IReader, uint, SubRecord>>
            {
                {(TES4.FieldType, TES4_HEDR.FieldType), (headerReader, dataReader, size) => new TES4_HEDR(headerReader, dataReader, size)},
                {(TES4.FieldType, TES4_CNAM.FieldType), (headerReader, dataReader, size) => new TES4_CNAM(headerReader, dataReader, size)},
                {(TES4.FieldType, TES4_SNAM.FieldType), (headerReader, dataReader, size) => new TES4_SNAM(headerReader, dataReader, size)},
                {(TES4.FieldType, TES4_MAST.FieldType), (headerReader, dataReader, size) => new TES4_MAST(headerReader, dataReader, size)},
                {(TES4.FieldType, TES4_DATA.FieldType), (headerReader, dataReader, size) => new TES4_DATA(headerReader, dataReader, size)},
                {(TES4.FieldType, TES4_ONAM.FieldType), (headerReader, dataReader, size) => new TES4_ONAM(headerReader, dataReader, size)},
                {(TES4.FieldType, TES4_INTV.FieldType), (headerReader, dataReader, size) => new TES4_INTV(headerReader, dataReader, size)},
                {(TES4.FieldType, TES4_INCC.FieldType), (headerReader, dataReader, size) => new TES4_INCC(headerReader, dataReader, size)}
            };
        }
    }
}