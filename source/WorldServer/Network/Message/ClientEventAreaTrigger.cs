using System.IO;
using System.Numerics;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ClientEventAreaTrigger, SubPacketDirection.Client)]
    public class ClientEventAreaTrigger : SubPacket
    {
        public uint EventId { get; private set; }
        public uint ActorId { get; private set; }
        public Vector3 Position { get; private set; }

        public override void Read(BinaryReader reader)
        {
            ActorId = reader.ReadUInt32();
            EventId = reader.ReadUInt32();
            Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.Skip(4u);
        }
    }
}
