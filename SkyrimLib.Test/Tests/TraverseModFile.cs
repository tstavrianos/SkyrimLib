using System;
using System.Diagnostics;

namespace SkyrimLib.Test.Tests
{
    public static class TraverseModFile
    {
        private static int _fields;
        private static int _records;
        private static int _groups;
        private static readonly Stopwatch Stopwatch = new Stopwatch();

        private static void FoundRecord(Record record)
        {
            _records++;

            _fields += record.FieldCount;
        }

        private static void FoundGroup(Group group)
        {
            _groups++;
            foreach (var child in group.Children)
            {
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
        }
        
        public static void Run(ModFile m)
        {
            Stopwatch.Start();
            foreach (var child in m.Children)
            {
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
        }
    }
}