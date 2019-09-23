using System;
using SkyrimLib.Generated;

namespace SkyrimLib.Test.Tests
{
    public static class TestGenerated
    {
        public static void Run(ModFile m)
        {
            var tes4 = m.Children[0] as TES4;
            Console.WriteLine(tes4.HEDR.version);
            Console.WriteLine(tes4.HEDR.numRecords);
            Console.WriteLine(tes4.CNAM?.Value);
            Console.WriteLine(tes4.SNAM?.Value);
        }
    }
}