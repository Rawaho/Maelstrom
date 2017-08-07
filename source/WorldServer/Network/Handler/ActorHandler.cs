using Shared.Game;
using Shared.Network;
using WorldServer.Game.Map;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class ActorHandler
    {
        [SubPacketHandler(SubPacketOpcode.NewWorld, SubPacketHandlerFlags.RequiresPlayer)]
        public static void HandleNewWorld(WorldSession session, SubPacket subPacket)
        {
            if (!session.Player.IsLogin || session.Player.InWorld)
                return;

            session.Player.OnLogin();
            MapManager.AddToMap(session.Player);
        }

        [SubPacketHandler(SubPacketOpcode.ClientTerritoryFinalise, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleClientTerritoryFinalise(WorldSession session, SubPacket subPacket)
        {
            session.Player.SendVisible();
        }

        [SubPacketHandler(SubPacketOpcode.ClientPlayerMove, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleClientPlayerMove(WorldSession session, ClientPlayerMove actorMove)
        {
            var newPosition = new WorldPosition(session.Player.Map.Id, actorMove.Position, actorMove.Orientation);
            session.Player.Relocate(newPosition);
        }
    }
}
