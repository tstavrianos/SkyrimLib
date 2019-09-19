using System.Collections.Generic;
using System.IO;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace SkyrimLib
{
    public sealed class Group: IRecordOrGroup
    {
        // ReSharper disable once InconsistentNaming
        public const uint GRUP = 1347768903;
        public uint Type { get; }
        public uint Size { get; }
        public uint Label { get; set; }
        public int GroupType { get; set; }
        public ushort Stamp { get; set; }
        public ushort Unknown1 { get; set; }
        public ushort Version { get; set; }
        public ushort Unknown2 { get; set; }
        public List<IRecordOrGroup> Children { get; }
        
        public Group(IReader headerReader, IReader dataReader)
        {
            this.Type = headerReader.ReadUInt32(0);
            this.Size = headerReader.ReadUInt32(4);
            this.Label = headerReader.ReadUInt32(8);
            this.GroupType = headerReader.ReadInt32(12);
            this.Stamp = headerReader.ReadUInt16(16);
            this.Unknown1 = headerReader.ReadUInt16(18);
            this.Version = headerReader.ReadUInt16(20);
            this.Unknown2 = headerReader.ReadUInt16(22);
            
            this.Children = new List<IRecordOrGroup>();

            var children = dataReader;
            while (true)
            {                
                if (children.Length < 4) break;
                var type = children.ReadUInt32(0);
                var size = children.ReadUInt32(4);
                IRecordOrGroup item;
                var actualSize = size;
                using (var childHeader = children.Slice(0, 24))
                {
                    if (type == GRUP)
                    {
                        using (var childData = children.Slice(24, (int) size - 24))
                            item = new Group(childHeader, childData);
                    }
                    else
                    {
                        actualSize += 24;
                        using (var childData = children.Slice(24, (int) size))
                            item = new Record(childHeader, childData);
                    }
                }

                this.Children.Add(item);
                children = children.Slice((int) actualSize);
            }
        }

        public void Write(IWriter writer)
        {
            using (var ms = new MemoryStream())
            {
                using (var childWriter = new StreamWriter(ms))
                {
                    foreach (var child in this.Children)
                    {
                        child.Write(childWriter);
                    }

                    var children = ms.ToArray();

                    writer.WriteUInt32(this.Type);
                    writer.WriteUInt32((uint) children.Length + 24);
                    writer.WriteUInt32(this.Label);
                    writer.WriteInt32(this.GroupType);
                    writer.WriteUInt16(this.Stamp);
                    writer.WriteUInt16(this.Unknown1);
                    writer.WriteUInt16(this.Version);
                    writer.WriteUInt16(this.Unknown2);
                    writer.WriteBytes(children);
                }
            }
        }

        public void Dispose()
        {
            foreach(var child in this.Children) child.Dispose();
        }
    }
}