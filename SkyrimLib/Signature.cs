// Source: https://github.com/matortheeternal/esper/blob/master/esper/data/Signature.cs

using System.Collections.Generic;

namespace SkyrimLib
{
    public readonly struct Signature
    {
        private static readonly SignatureEncoding Encoding = new SignatureEncoding();

        private readonly byte _b0;
        private readonly byte _b1;
        private readonly byte _b2;
        private readonly byte _b3;

        public byte[] Bytes => new[] {this._b0, this._b1, this._b2, this._b3};

        public Signature(byte b0, byte b1, byte b2, byte b3)
        {
            this._b0 = b0;
            this._b1 = b1;
            this._b2 = b2;
            this._b3 = b3;
        }

        public Signature(IReadOnlyList<byte> b)
        {
            this._b0 = b[0];
            this._b1 = b[1];
            this._b2 = b[2];
            this._b3 = b[3];
        }

        public static Signature FromString(string str)
        {
            var bytes = SignatureEncoding.Encode(str);
            return new Signature(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        public override string ToString()
        {
            return SignatureEncoding.Decode(this.Bytes);
        }

        public static bool operator ==(Signature a, Signature b)
        {
            return a._b0 == b._b0 &&
                   a._b1 == b._b1 &&
                   a._b2 == b._b2 &&
                   a._b3 == b._b3;
        }

        public static bool operator !=(Signature a, Signature b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this == (Signature) obj;
        }

        public override int GetHashCode()
        {
            return (this._b3 << 24) | (this._b2 << 16) | (this._b1 << 8) | this._b0;
        }

        internal static Signature Read(int pos, IReader source)
        {
            return new Signature(source.ReadBytes(pos, 4));
        }
    }
}