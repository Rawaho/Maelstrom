using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientPartyLeave)]
    public class ClientPartyLeave : SubPacket
    {
        public uint Timestamp { get; private set; }

        public override void Read(BinaryReader reader)
        {
            reader.Skip(4u);
            reader.ReadUInt32();
            Timestamp = reader.ReadUInt32();
            reader.Skip(8u);
            reader.ReadUInt32();
        }
    }
}
