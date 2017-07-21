using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerLogout, SubPacketDirection.Server)]
    public class ServerLogout : SubPacket
    {
    }
}
