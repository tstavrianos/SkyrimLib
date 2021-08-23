// Source: https://github.com/matortheeternal/esper/blob/master/esper/data/SignatureEncoding.cs

using System;

namespace SkyrimLib
{
    public class SignatureEncoding
    {
        public static byte[] Encode(string str)
        {
            var bytes = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                var c = str[i];
                bytes[i] = Convert.ToByte(c);
            }

            return bytes;
        }

        public static string Decode(byte[] data)
        {
            var chars = new char[4];
            for (var i = 0; i < 4; i++)
            {
                var b = data[i];
                chars[i] = Convert.ToChar(b);
            }

            return new string(chars);
        }
    }
}