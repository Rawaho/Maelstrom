using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerRetainerList, SubPacketDirection.Server)]
    public class ServerRetainerList : SubPacket
    {
        public override void Write(BinaryWriter writer)
        {
        }
    }
}
