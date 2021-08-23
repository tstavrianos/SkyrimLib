using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SkyrimLib.Test.Tests;

namespace SkyrimLib.Test
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var filename = @"e:\SteamLibrary\steamapps\common\Skyrim Special Edition\Data\Skyrim.esm";
            if (args.Length != 0 && File.Exists(args[0])) filename = args[0];
            var stopWatch = new Stopwatch();
            Console.WriteLine($"Memory used before: {GC.GetTotalMemory(false)} bytes");
            stopWatch.Start();
            var m = new ModFile(filename);
            var loading = stopWatch.ElapsedMilliseconds;
            Console.WriteLine($"Loading took: {loading}ms");
            Console.WriteLine($"Memory used after: {GC.GetTotalMemory(false)} bytes");

            //TraverseModFile.Run(m);
            TestGenerated.Run(m);

            Console.WriteLine($"Memory usage before dispose: {GC.GetTotalMemory(false)} bytes");

            m.Dispose();

            Console.WriteLine($"Memory usage after dispose: {GC.GetTotalMemory(false)} bytes");
            m = null;
            GC.Collect();

            Console.WriteLine($"Memory usage after gc collect: {GC.GetTotalMemory(false)} bytes");
        }
    }
}