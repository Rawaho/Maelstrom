using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerUnknown01FB)]
    public class ServerUnknown01FB : SubPacket
    {
        public override void Write(BinaryWriter writer)
        {
            writer.Pad(8u);
        }
    }
}
