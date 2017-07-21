using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Shared.SqPack
{
    public class Exh
    {
        public struct Column
        {
            public ExdDataType Type;
            public ushort Offset;
        }

        public struct Page
        {
            public uint Id;
            public uint Entries;
        }

        public ushort DataSize { get; }
        public Column[] Columns { get; }
        public Page[] Pages { get; }
        public ExdLanguage[] Languages { get; }
        public uint Entries { get; }

        private static readonly byte[] magic = { 0x45, 0x58, 0x48, 0x46 }; // EXHF

        public Exh(string name)
        {
            using (var steam = new MemoryStream(File.ReadAllBytes(name)))
            {
                using (var reader = new BinaryReaderBigEndian(steam))
                {
                    byte[] magicBytes = reader.ReadBytes(4);
                    Debug.Assert(magicBytes.SequenceEqual(magic));

                    ushort version = reader.ReadUInt16Be();
                    Debug.Assert(version == 3);

                    DataSize  = reader.ReadUInt16Be();
                    Columns   = new Column[reader.ReadUInt16Be()];
                    Pages     = new Page[reader.ReadUInt16Be()];
                    Languages = new ExdLanguage[reader.ReadUInt16Be()];
                    reader.ReadUInt16Be();
                    reader.ReadUInt32Be();
                    Entries   = reader.ReadUInt32Be();
                    reader.Skip(8); // padding

                    for (uint i = 0u; i < Columns.Length; i++)
                    {
                        Columns[i] = new Column
                        {
                            Type   = (ExdDataType)reader.ReadUInt16Be(),
                            Offset = reader.ReadUInt16Be()
                        };
                    }

                    //Columns = Columns.OrderBy(c => c.Offset).ToArray();

                    for (uint i = 0u; i < Pages.Length; i++)
                    {
                        Pages[i] = new Page
                        {
                            Id      = reader.ReadUInt32Be(),
                            Entries = reader.ReadUInt32Be()
                        };
                    }

                    for (uint i = 0u; i < Languages.Length; i++)
                        Languages[i] = (ExdLanguage)reader.ReadUInt16();
                }
            }
        }
    }
}
