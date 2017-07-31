using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Shared.SqPack
{
    public class BinaryReaderBigEndian : BinaryReader
    {
        public BinaryReaderBigEndian(Stream input) : base(input) { }
        public BinaryReaderBigEndian(Stream input, Encoding encoding) : base(input, encoding) { }
        public BinaryReaderBigEndian(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }

        public void Skip(int count)
        {
            BaseStream.Position += count;
        }

        public short ReadInt16Be()
        {
            byte[] data = ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public ushort ReadUInt16Be()
        {
            byte[] data = ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public int ReadInt32Be()
        {
            byte[] data = ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public uint ReadUInt32Be()
        {
            byte[] data = ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public ulong ReadUInt64Be()
        {
            byte[] data = ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToUInt64(data, 0);
        }

        public string ReadExdString()
        {
            var sb = new StringBuilder();
            while (true)
            {
                char c = ReadChar();
                if (c == 0x00)
                    break;

                // string parameter
                if (c == 0x02)
                {
                    ReadByte(); // type
                    byte[] payload = ReadBytes(ReadByte() - 1);
                    Debug.Assert(ReadByte() == 0x03);

                    // sb.AppendFormat(Encoding.UTF8.GetString(payload));
                }
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
