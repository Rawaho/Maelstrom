using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientPartyDisband)]
    public class ClientPartyDisband : SubPacket
    {
        public uint Timestamp { get; private set; }

        public override void Read(BinaryReader reader)
        {
            reader.Skip(8u);
            Timestamp = reader.ReadUInt32();
            reader.Skip(12u);
        }
    }
}
