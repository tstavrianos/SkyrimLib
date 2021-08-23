using System.Collections.Generic;

namespace EspAnalyser
{
    public class Field
    {
        public byte[] Type { get; }
        public ushort DataSize;
        public byte[] Data;
        public List<Field> Fields;
    }
}