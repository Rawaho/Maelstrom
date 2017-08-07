using Shared.Network;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class MiscHandler
    {
        [SubPacketHandler(SubPacketOpcode.ClientLogout, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleClientLogout(Session session, SubPacket subPacket)
        {
            session.Send(new ServerLogout());
        }
    }
}
