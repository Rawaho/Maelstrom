using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerUnknown0209)]
    public class ServerUnknown01FD : SubPacket
    {
        public override void Write(BinaryWriter writer)
        {
            writer.Pad(56u);
        }
    }
}
