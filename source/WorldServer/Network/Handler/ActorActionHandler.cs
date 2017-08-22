using System;
using Shared.Network;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class ActorActionHandler
    {
        [SubPacketHandler(SubPacketClientOpcode.ClientActorAction, SubPacketHandlerFlags.RequiresWorld)]
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

        [ActorActionHandler(ActorAction.AchievementCriteriaRequest)]
        public static void HandleActorActionAchievementCriteriaRequest(WorldSession session, ClientActorAction actorAction)
        {
            session.Player.Achievement.SendAchievementCriteria(actorAction.Parameters[0]);
        }

        [ActorActionHandler(ActorAction.AchievementList)]
        public static void HandleActorActionAchievementList(WorldSession session, ClientActorAction actorAction)
        {
            session.Player.Achievement.SendAchievementList();
        }
    }
}
