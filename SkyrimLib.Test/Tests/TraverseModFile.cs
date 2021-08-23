using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SkyrimLib.Test.Tests
{
    public static class TraverseModFile
    {
        private static int _fields;
        private static int _records;
        private static int _groups;
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static Dictionary<string, HashSet<string>> _fieldsInRecords;

        private static void FoundField(SubRecord record)
        {
        }

        private static void FoundRecord(Record record)
        {
            if (!_fieldsInRecords.TryGetValue(record.Type.ToString(), out var fields))
            {
                fields = new HashSet<string>();
                _fieldsInRecords.Add(record.Type.ToString(), fields);
            }
            _records++;
            foreach (var child in record.Fields)
            {
                fields.Add(child.Type.ToString());
                FoundField(child);
            }

            _fields += record.FieldCount;
        }

        private static void FoundGroup(Group group)
        {
            _groups++;
            foreach (var child in group.Children)
                switch (child)
                {
                    case Group group1:
                        FoundGroup(group1);
                        break;
                    case Record record1:
                        FoundRecord(record1);
                        break;
                }
        }

        public static void Run(ModFile m)
        {
            _fieldsInRecords = new Dictionary<string, HashSet<string>>();
            Stopwatch.Start();
            foreach (var child in m.Children)
            {
                //Console.WriteLine(child.Type);
                switch (child)
                {
                    case Group group1:
                        FoundGroup(group1);
                        break;
                    case Record record1:
                        FoundRecord(record1);
                        break;
                }
            }

            var parsing = Stopwatch.ElapsedMilliseconds;
            Stopwatch.Stop();

            Console.WriteLine($"Found Groups: {_groups}, Records: {_records}, Subrecords: {_fields}");
            Console.WriteLine($"Tranversing took: {parsing}ms");

            foreach (var kv in _fieldsInRecords)
            {
                Console.WriteLine($"{kv.Key} => {string.Join(',', kv.Value)}");
            }
        }
    }
}