using System.Data;
using Shared.Game.Enum;

namespace Shared.Database.Authentication
{
    public struct ServiceAccountInfo
    {
        public uint Id { get; }
        public string Name { get; }
        public Expansion Expansion { get; }
        public ushort RealmCharacterLimit { get; }
        public ushort AccountCharacterLimit { get; }

        public ServiceAccountInfo(DataRow data)
        {
            Id                    = data.Read<uint>("id");
            Name                  = data.Read<string>("Name");
            Expansion             = data.Read<Expansion>("expansion");
            RealmCharacterLimit   = data.Read<ushort>("realmCharacterLimit");
            AccountCharacterLimit = data.Read<ushort>("accountCharacterLimit");
        }
    }
}
