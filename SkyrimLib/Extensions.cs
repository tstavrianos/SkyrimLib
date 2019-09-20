using System.IO;

namespace SkyrimLib
{
    public static class Extensions
    {
        public static IReader NextChunk(this BinaryReader reader, int length)
        {
            var data = new byte[length];
            var offset=0;
            var remaining = length;
            while (remaining > 0)
            {
                var read = reader.Read(data, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException($"End of stream reached with {remaining} bytes left to read");
                remaining -= read;
                offset += read;
            }
            return new ArrayReader(data, 0, length);
        }
    }
}