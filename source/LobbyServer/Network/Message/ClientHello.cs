using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketType.ClientHello)]
    public class ClientHello : SubPacket
    {
        public byte[] Seed { get; private set; } // this is hardcoded in the client
        public uint Time { get; private set; }

        public override void Read(BinaryReader reader)
        {
            reader.Skip(0x24u);
            Seed = reader.ReadBytes(0x20);
            reader.Skip(0x20u);
            Time = reader.ReadUInt32();
        }
    }
}
