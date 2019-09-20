using System;
using System.Collections.Generic;
using System.IO;

namespace SkyrimLib
{
    public sealed class ModFile : IDisposable
    {
        public List<IRecordOrGroup> Children { get; }

        public ModFile(string filename)
        {
            this.Children = new List<IRecordOrGroup>();
            
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 8192,
                FileOptions.SequentialScan))
            {
                using (var br = new BinaryReader(fs))
                {
                    while (true)
                    {
                        if (fs.Position + 4 >= fs.Length) break;
                        var header = br.NextChunk(24);

                        var type = header.ReadUInt32(0);
                        var size = header.ReadUInt32(4);

                        if (type == Group.GRUP)
                        {
                            size -= 24;
                        }

                        var data = br.NextChunk((int) size);

                        IRecordOrGroup item;
                        if (type == Group.GRUP)
                        {
                            item = new Group(header, data);
                        }
                        else
                        {
                            item = Registry.ParsedRecords.TryGetValue(type, out var constructor) ? constructor(header, data) : new Record(header, data);
                        }
                        this.Children.Add(item);
                    }
                }
            }
        }

        public void Write(string filename)
        {
            using (var fs = File.Create(filename))
            {
                using (var writer = new StreamWriter(fs))
                {
                    foreach (var child in this.Children)
                    {
                        child.Write(writer);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var child in this.Children) child.Dispose();
            this.Children.Clear();
        }
    }
}