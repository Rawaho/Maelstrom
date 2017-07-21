using System.Data;

namespace Shared.Database.Datacentre
{
    public class RealmInfo
    {
        public ushort Id { get; }
        public string Name { get; }
        public uint Flags { get;}
        public string Host { get; }
        public ushort Port { get; }

        public RealmInfo(DataRow data)
        {
            Id    = data.Read<ushort>("id");
            Name  = data.Read<string>("name");
            Flags = data.Read<uint>("flags");
            Host  = data.Read<string>("host");
            Port  = data.Read<ushort>("port");
        }
    }
}
