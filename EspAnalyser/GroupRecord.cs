using System.Collections.Generic;

namespace EspAnalyser
{
    public class GroupRecord : IElement
    {
        public byte[] Type { get; }
        public uint Size { get; }
        public byte[] Label;
        public int GroupType;
        public byte[] VersionControlInfo;
        public uint Unknown;
        public byte[] Data;
        public List<IElement> Elements;
    }
}