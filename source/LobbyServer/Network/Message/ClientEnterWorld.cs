using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ClientEnterWorld, SubPacketDirection.Client)]
    public class ClientEnterWorld : SubPacket
    {
        public ulong Sequence { get; private set; }
        public ulong CharacterId { get; private set; }

        public override void Read(BinaryReader reader)
        {
            Sequence    = reader.ReadUInt64();
            CharacterId = reader.ReadUInt64();
            reader.ReadUInt64();
            reader.ReadUInt64();
        }
    }
}
