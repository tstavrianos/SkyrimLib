using System;
using System.Diagnostics;
using System.IO;

namespace SkyrimLib.Test
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var filename = @"..\..\..\..\data\Skyrim.esm";
            if (args.Length != 0 && File.Exists(args[0])) filename = args[0];
            Console.WriteLine($"Memory used before: {GC.GetTotalMemory(false)}");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var m = new ModFile(filename);
            var loading = stopWatch.ElapsedMilliseconds;
            Console.WriteLine($"Loading took: {loading}");
            Console.WriteLine($"Memory used after: {GC.GetTotalMemory(false)}");
            var fields = 0;
            var records = 0;
            var groups = 0;

            void FoundRecord(Record record)
            {
                records++;

                fields += record.Fields.Count;
            }

            void FoundGroup(Group group)
            {
                groups++;
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
            stopWatch.Restart();
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

            var parsing = stopWatch.ElapsedMilliseconds;
            stopWatch.Stop();
            
            Console.WriteLine($"Found Groups: {groups}, Records: {records}, Subrecords: {fields}");
            Console.WriteLine($"Parsing took: {parsing}");
            Console.WriteLine($"Memory usage before termination: {GC.GetTotalMemory(false)}");        
        }
    }
}