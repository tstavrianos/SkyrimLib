using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Medallion.Collections;
using Mutagen.Bethesda.Skyrim;

namespace MyLoot
{
    class Program
    {
        private static readonly Regex CreationClubRegeg = new Regex(@"cc[A-Z]{3}SSE[0-9]{3}.*\.es(l|m)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static IEnumerable<string> ParsePlugins(string filename, string path)
        {
            yield return "Skyrim.esm";
            yield return "Update.esm";
            yield return "Dawnguard.esm";
            yield return "HearthFires.esm";
            yield return "Dragonborn.esm";

            foreach (var name in Directory.EnumerateFiles(path, "cc*.es?").Select(Path.GetFileName)
                .Where(name => CreationClubRegeg.IsMatch(name)))
            {
                yield return name;
            }

            if (!File.Exists(filename)) yield break;
            var data = File.ReadAllLines(filename);
            foreach (var l in from line in data
                where !string.IsNullOrWhiteSpace(line)
                select line.Trim()
                into l
                where l[0] != '#'
                where l[0] == '*'
                select l.Substring(1))
            {
                yield return l;
            }
        }

        private class Mod
        {
            public string Name;
            public List<Mod> DependsOn;
            public int OriginalOrder;
        }

        private static void Main(string[] args)
        {
            var plugins = Path.Combine(
                Environment.GetEnvironmentVariable("LocalAppData")!, "Skyrim Special Edition/Plugins.txt");
            var gamePath = @"e:\keizaal\Game Root\Data";
            var mods = new List<Mod>();

            var i = 0;
            foreach (var modName in ParsePlugins(plugins, gamePath))
            {
                mods.Add(new Mod() {Name = modName, OriginalOrder = i});
                i++;
            }

            foreach (var mod in mods)
            {
                var m = SkyrimMod.CreateFromBinaryOverlay(Path.Combine(gamePath, mod.Name), SkyrimRelease.SkyrimSE);
                mod.DependsOn = new List<Mod>();
                if (m.MasterReferences is not {Count: > 0}) continue;
                foreach (var mast in m.MasterReferences)
                {
                    if (!mods.Any(x => x.Name.Equals(mast.Master.FileName, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine(mast.Master.FileName);
                    }

                    mod.DependsOn.Add(
                        mods.First(x => x.Name.Equals(mast.Master.FileName, StringComparison.OrdinalIgnoreCase)));
                }
            }

            var ordered = mods.StableOrderTopologicallyBy(x => x.DependsOn).ToList();
            for (i = 0; i < ordered.Count; i++)
            {
                Console.WriteLine($"{ordered[i].OriginalOrder} => {i} {ordered[i].Name}");
            }
        }
    }
}