using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ClientEventGossip, SubPacketDirection.Client)]
    public class ClientEventGossip : SubPacket
    {
        public ulong ActorId { get; private set; }
        public uint EventId { get; private set; }

        public override void Read(BinaryReader reader)
        {
            ActorId = reader.ReadUInt64();
            EventId = reader.ReadUInt32();
            reader.Skip(4u);
        }
    }
}
