using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerRetainerList)]
    public class ServerRetainerList : SubPacket
    {
        public override void Write(BinaryWriter writer)
        {
        }
    }
}
