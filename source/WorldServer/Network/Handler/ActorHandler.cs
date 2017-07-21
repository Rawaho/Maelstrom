using System;
using Shared.Game;
using Shared.Network;
using WorldServer.Game.Map;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class ActorHandler
    {
        [SubPacketHandler(SubPacketOpcode.NewWorld)]
        public static void HandleNewWorld(WorldSession session, SubPacket subPacket)
        {
            if (!session.Player.IsLogin || session.Player.InWorld)
                return;

            session.Player.OnLogin();
            MapManager.AddToMap(session.Player);
        }

        [SubPacketHandler(SubPacketOpcode.ClientTerritoryFinalise)]
        public static void HandleClientTerritoryFinalise(WorldSession session, SubPacket subPacket)
        {
            session.Player.SendVisible();
        }

        [SubPacketHandler(SubPacketOpcode.ClientPlayerMove)]
        public static void HandleClientPlayerMove(WorldSession session, ClientPlayerMove actorMove)
        {
            var newPosition = new WorldPosition(session.Player.Map.Id, actorMove.Position, actorMove.Orientation);
            session.Player.Relocate(newPosition);
        }

        [SubPacketHandler(SubPacketOpcode.ClientActorAction)]
        public static void HandleActorAction(WorldSession session, ClientActorAction actorAction)
        {
            Console.WriteLine($"Got Actor Action: {actorAction.Action}(0x{actorAction.Action:X}), {actorAction.Parameters[0]}, "
                + $"{actorAction.Parameters[1]}, {actorAction.Parameters[2]}, {actorAction.Parameters[3]}, {actorAction.Parameters[4]}");

            ActorActionManager.Invoke(session, actorAction);
        }

        [ActorActionHandler(ActorAction.Action00C9)]
        public static void HandleActorAction00C9(WorldSession session, ClientActorAction actorAction)
        {
            session.Player.IsLoading = false;
            session.Player.IsLogin   = false;
        }
    }
}
