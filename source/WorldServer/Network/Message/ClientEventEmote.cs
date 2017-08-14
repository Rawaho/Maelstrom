using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ClientEventEmote, SubPacketDirection.Client)]
    public class ClientEventEmote : SubPacket
    {
        public ulong ActorId { get; private set; }
        public uint EventId { get; private set; }
        public ushort EmoteId { get; private set; }

        public override void Read(BinaryReader reader)
        {
            ActorId = reader.ReadUInt64();
            EventId = reader.ReadUInt32();
            EmoteId = reader.ReadUInt16();
        }
    }
}
