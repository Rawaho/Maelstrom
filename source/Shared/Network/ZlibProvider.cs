using System;
using System.IO;
using System.IO.Compression;

namespace Shared.Network
{
    public static class ZlibProvider
    {
        private enum CompressionSpeed : byte
        {
            Fastest,
            Fast,
            Default,
            Maximum
        }

        private enum CompressionMethod : byte
        {
            Deflate = 8
        }

        private const byte HeaderLength  = 2;
        private const byte Adler32Length = 4;

        public static bool Deflate(byte[] data, out byte[] decompressed)
        {
            decompressed = new byte[0];
            if (data.Length < HeaderLength + Adler32Length)
                return false;

            byte cmf = data[0];
            if ((cmf & 0xF) != (byte)CompressionMethod.Deflate)
                return false;

            // byte cinfo = (byte)(cmf >> 4);

            byte flg = data[1];
            if ((cmf * 256 + flg) % 31 != 0)
                return false;

            // no support for dictonary
            if ((flg & 0x20) != 0)
                return false;

            if (flg >> 6 != (byte)CompressionSpeed.Default)
                return false;

            // uint adler32 = BitConverter.ToUInt32(data, data.Length - Adler32Length);

            byte[] deflateData = new byte[data.Length - (HeaderLength + Adler32Length)];
            Buffer.BlockCopy(data, HeaderLength, deflateData, 0, data.Length - (HeaderLength + Adler32Length));

            using (var compressedStream = new MemoryStream(deflateData))
            {
                using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        deflateStream.CopyTo(outputStream);
                        decompressed = outputStream.ToArray();
                    }
                }
            }

            return true;
        }
    }
}
