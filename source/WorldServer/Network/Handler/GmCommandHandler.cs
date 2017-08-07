using Shared.Network;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class GmCommandHandler
    {
        [SubPacketHandler(SubPacketOpcode.ClientGmCommandInt, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleGmCommandInt(WorldSession session, ClientGmCommandInt gmCommand)
        {
            GmCommandManager.Invoke(session, gmCommand);
        }

        [SubPacketHandler(SubPacketOpcode.ClientGmCommandString, SubPacketHandlerFlags.RequiresWorld)]
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

        [GmCommandHandler(GmCommand.Item)]
        public static void HandleGmCommandItem(WorldSession session, GmCommandParameters parameters)
        {
            session.Player.Inventory.NewItem(parameters.Parameters[0], parameters.Parameters[1]);
        }
    }
}
