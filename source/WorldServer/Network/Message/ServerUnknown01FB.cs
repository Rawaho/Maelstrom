using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerUnknown01FB, SubPacketDirection.Server)]
    public class ServerUnknown01FB : SubPacket
    {
        public override void Write(BinaryWriter writer)
        {
            writer.Pad(8u);
        }
    }
}
