using Shared.Game;
using Shared.Network;
using WorldServer.Game.Map;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class ActorHandler
    {
        [SubPacketHandler(SubPacketClientOpcode.ClientNewWorld, SubPacketHandlerFlags.RequiresPlayer)]
        public static void HandleNewWorld(WorldSession session, SubPacket subPacket)
        {
            if (!session.Player.IsLogin || session.Player.InWorld)
                return;

            session.Player.OnLogin();
            MapManager.AddToMap(session.Player);
        }

        [SubPacketHandler(SubPacketClientOpcode.ClientTerritoryFinalise, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleClientTerritoryFinalise(WorldSession session, SubPacket subPacket)
        {
            session.Player.SendVisible();
        }

        [SubPacketHandler(SubPacketClientOpcode.ClientPlayerMove, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleClientPlayerMove(WorldSession session, ClientPlayerMove actorMove)
        {
            var newPosition = new WorldPosition((ushort)session.Player.Map.Entry.Index, actorMove.Position, actorMove.Orientation);
            session.Player.Relocate(newPosition);
        }
    }
}
