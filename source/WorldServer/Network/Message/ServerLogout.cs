using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerLogout)]
    public class ServerLogout : SubPacket
    {
    }
}
