using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace MapGenerator.File
{
    public class PcbListFile
    {
        public class PcbEntry
        {
            public uint Id { get; }
            public Vector3 Min { get; }
            public Vector3 Max { get; }

            public PcbEntry(BinaryReader reader)
            {
                Min = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Max = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                reader.ReadUInt32();
                Id  = reader.ReadUInt32();
            }
        }

        public List<PcbEntry> PcbEntries { get; } = new List<PcbEntry>();

        public PcbListFile(SaintCoinach.IO.File listFile)
        {
            using (MemoryStream stream = new MemoryStream(listFile.GetData()))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    uint pcbCount = reader.ReadUInt32();
                    for (int i = 0; i < pcbCount; i++)
                        PcbEntries.Add(new PcbEntry(reader));
                }
            }
        }
    }
}
