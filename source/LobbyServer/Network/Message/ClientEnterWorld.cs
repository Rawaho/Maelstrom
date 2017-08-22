using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientEnterWorld)]
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
