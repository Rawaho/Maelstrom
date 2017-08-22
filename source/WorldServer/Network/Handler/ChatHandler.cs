using Shared.Command;
using Shared.Network;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class ChatHandler
    {
        [SubPacketHandler(SubPacketClientOpcode.ClientChat, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleChat(WorldSession session, ClientChat chat)
        {
            if (chat.Message.StartsWith("."))
            {
                CommandManager.ParseCommand(chat.Message.Remove(0, 1), out string command, out string[] parameters);
                if (CommandManager.GetCommand(session, command, parameters, out CommandManager.CommandHandler handler))
                    handler.Invoke(session, parameters);
            }
            else
            {
                // TODO
            }
        }
    }
}
