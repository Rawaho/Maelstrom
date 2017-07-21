using System.Numerics;
using Shared.Command;
using Shared.Game;
using WorldServer.Network;
using WorldServer.Network.Message;

namespace WorldServer.Command
{
    public static class ActorHandler
    {
        // actor_action typeId
        [CommandHandler("actor_action", SecurityLevel.Developer, 1)]
        public static void HandleActorControl(WorldSession session, params string[] parameters)
        {
            if (!uint.TryParse(parameters[0], out uint type))
                return;

            session.Send(new ServerActorAction2
            {
                Action = (ActorAction)type
            });
        }

        // actor_teleport x y z territoryId
        [CommandHandler("actor_teleport", SecurityLevel.Developer, 4)]
        public static void HandleActorTeleport(WorldSession session, params string[] parameters)
        {
            float[] offset = new float[3];
            for (int i = 0; i < 3; i++)
                if (!float.TryParse(parameters[i], out offset[i]))
                    return;

            if (!ushort.TryParse(parameters[3], out ushort territoryId))
                return;

            session.Player.TeleportTo(new WorldPosition(territoryId, new Vector3(offset[0], offset[1], offset[2]), 0f));
        }
    }
}