using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketType.ServerHelloWorld)]
    public class ServerHelloWorld : SubPacket
    {
        public uint ActorId;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(ActorId);
            writer.Pad(36u);
        }
    }
}
