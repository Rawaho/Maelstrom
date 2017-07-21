using System.Data;

namespace Shared.Database.Datacentre
{
    public struct CharacterClassInfo
    {
        public const byte MaxClassId = 25;

        public byte Id { get; }
        public ushort Level { get; }
        public uint Experience { get; }

        public CharacterClassInfo(byte id)
        {
            Id         = id;
            Level      = 1;
            Experience = 0;
        }

        public CharacterClassInfo(DataRow row)
        {
            Id         = row.Read<byte>("classId");
            Level      = row.Read<ushort>("level");
            Experience = row.Read<uint>("xp");
        }

        public void SaveToDatabase(ulong id, DatabaseTransaction transaction)
        {
            transaction.AddPreparedStatement(DataCentreDatabase.DataCentrePreparedStatement.CharacterClassInsert,
                id, Id, Level, Experience);
        }
    }
}
