using System.IO;
using System.Text;

namespace SkyrimLib
{
    public sealed class StreamWriter : IWriter
    {
        private readonly BinaryWriter _writer;

        public StreamWriter(Stream stream)
        {
            this._writer = new BinaryWriter(stream, Encoding.ASCII, true);
        }

        public long Position => this._writer.BaseStream.Position;

        public void WriteBool(bool value) => this._writer.Write(value);

        public void WriteByte(byte value) => this._writer.Write(value);

        public void WriteBytes(byte[] buffer) => this._writer.Write(buffer);

        public void WriteBytes(byte[] buffer, int index, int count) => this._writer.Write(buffer, index, count);

        public void WriteChar(char ch) => this._writer.Write(ch);

        public void WriteDecimal(decimal value) => this._writer.Write(value);

        public void WriteDouble(double value) => this._writer.Write(value);

        public void WriteInt16(short value) => this._writer.Write(value);

        public void WriteInt32(int value) => this._writer.Write(value);

        public void WriteInt64(long value) => this._writer.Write(value);

        public void WriteSByte(sbyte value) => this._writer.Write(value);

        public void WriteSingle(float value) => this._writer.Write(value);

        public void WriteUInt16(ushort value) => this._writer.Write(value);

        public void WriteUInt32(uint value) => this._writer.Write(value);

        public void WriteUInt64(ulong value) => this._writer.Write(value);

        public void WriteStringZeroTerminated(string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            this.WriteBytes(bytes);
            this.WriteByte((byte)'\0');
        }

        public void Dispose() => this._writer.Dispose();
    }
}