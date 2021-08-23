namespace EspAnalyser
{
    public class Record : IElement
    {
        public byte[] Type { get; }
        public uint Size { get; }
        public uint Flags;
        public uint Id;
        public byte[] VersionControlInfo;
        public ushort Version;
        public ushort Unknown;
        public byte[] Data;
        
        
    }
}