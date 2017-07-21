using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Shared.SqPack
{
    public class Exd
    {
        public class Entry
        {
            public uint Index;
            public uint Offset;
            public object[] Data;
        }

        public Entry[] Entries { get; private set; }

        private static readonly byte[] magic = { 0x45, 0x58, 0x44, 0x46 }; // EXDF

        public void Load<T>(string name, Exh header, uint pageIndex) where T : Entry, new()
        {
            using (var steam = new MemoryStream(File.ReadAllBytes(name)))
            {
                using (var reader = new BinaryReaderBigEndian(steam))
                {
                    byte[] magicBytes = reader.ReadBytes(4);
                    Debug.Assert(magicBytes.SequenceEqual(magic));

                    ushort version = reader.ReadUInt16Be();
                    Debug.Assert(version == 2);

                    reader.ReadUInt16Be();

                    uint offsetSize = reader.ReadUInt32Be();
                    //Debug.Assert(offsetSize == header.Pages[pageIndex].Entries * 8);

                    uint dataSize = reader.ReadUInt32Be();
                    reader.Skip(16); // padding

                    // this the correct way to handle this??
                    uint entries = header.Pages[pageIndex].Entries;
                    if (header.Pages.Length == 1 && header.Pages[0].Entries > header.Entries)
                        entries = header.Entries;

                    Entries = new Entry[entries];
                    for (uint i = 0u; i < entries; i++)
                    {
                        Entries[i] = new T
                        {
                            Index  = reader.ReadUInt32Be(),
                            Offset = reader.ReadUInt32Be(),
                            Data   = new object[header.Columns.Length]
                        };
                    }

                    foreach (Entry entry in Entries)
                    {
                        reader.BaseStream.Position = entry.Offset;

                        reader.ReadUInt32Be(); // size
                        reader.ReadUInt16Be();

                        long dataTable = reader.BaseStream.Position;
                        for (uint i = 0u; i < header.Columns.Length; i++)
                        {
                            reader.BaseStream.Position = dataTable + header.Columns[i].Offset;
                            if ((uint)header.Columns[i].Type >= 0x19u)
                            {
                                int bitOffset = (int)header.Columns[i].Type - 0x19;
                                entry.Data[i] = (reader.ReadByte() & (1 << bitOffset)) != 0;
                            }
                            else
                            {
                                switch (header.Columns[i].Type)
                                {
                                    case ExdDataType.String:
                                        entry.Data[i] = reader.ReadUInt32Be();
                                        break;
                                    case ExdDataType.Bool:
                                        entry.Data[i] = reader.ReadBoolean();
                                        break;
                                    case ExdDataType.SByte:
                                        entry.Data[i] = reader.ReadSByte();
                                        break;
                                    case ExdDataType.Byte:
                                        entry.Data[i] = reader.ReadByte();
                                        break;
                                    case ExdDataType.Short:
                                        entry.Data[i] = reader.ReadInt16();
                                        break;
                                    case ExdDataType.UShort:
                                        entry.Data[i] = reader.ReadUInt16Be();
                                        break;
                                    case ExdDataType.Int:
                                        entry.Data[i] = reader.ReadInt32Be();
                                        break;
                                    case ExdDataType.UInt:
                                        entry.Data[i] = reader.ReadUInt32Be();
                                        break;
                                    case ExdDataType.Packed:
                                        reader.ReadUInt32Be();
                                        reader.ReadUInt32Be();
                                        break;
                                    default:
                                        Debug.Assert(false, $"Unhandled EXD data type {header.Columns[i].Type}");
                                        break;
                                }
                            }
                        }

                        long stringTable = dataTable + header.DataSize;
                        for (uint i = 0u; i < header.Columns.Length; i++)
                        {
                            if (header.Columns[i].Type != ExdDataType.String)
                                continue;

                            reader.BaseStream.Position = stringTable + (uint)entry.Data[i];
                            entry.Data[i] = reader.ReadExdString();
                        }
                    }
                }
            }
        }
    }
}
