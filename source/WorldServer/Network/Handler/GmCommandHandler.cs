using Shared.Network;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class GmCommandHandler
    {
        [SubPacketHandler(SubPacketOpcode.ClientGmCommandInt)]
        public static void HandleGmCommandInt(WorldSession session, ClientGmCommandInt gmCommand)
        {
            GmCommandManager.Invoke(session, gmCommand);
        }

        [SubPacketHandler(SubPacketOpcode.ClientGmCommandString)]
        public static void HandleGmCommandString(WorldSession session, ClientGmCommandString gmCommand)
        {
            GmCommandManager.Invoke(session, gmCommand);
        }

        [GmCommandHandler(GmCommand.Inspect)]
        public static void HandleGmCommandInspect(WorldSession session, GmCommandParameters parameters)
        {
            if (parameters.TargetActorId != 0u)
            {
            }
            else if (parameters.TargetActorName != string.Empty)
            {
            }
        }
    }
}
