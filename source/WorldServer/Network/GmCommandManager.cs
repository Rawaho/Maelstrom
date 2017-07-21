using System;
using System.Collections.Generic;
using System.Reflection;
using WorldServer.Network.Message;

namespace WorldServer.Network
{
    public class GmCommandManager
    {
        public delegate void GmCommandHandler(WorldSession session, GmCommandParameters parameters);

        private static readonly Dictionary<GmCommand, GmCommandHandler> gmCommandHandlers = new Dictionary<GmCommand, GmCommandHandler>();

        public static void Initalise()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                foreach (MethodInfo methodInfo in type.GetMethods())
                    foreach (GmCommandHandlerAttribute attribute in methodInfo.GetCustomAttributes<GmCommandHandlerAttribute>())
                        gmCommandHandlers[attribute.Command] = (GmCommandHandler)Delegate.CreateDelegate(typeof(GmCommandHandler), methodInfo);
        }

        public static void Invoke(WorldSession session, ClientGmCommandInt gmCommand)
        {
            Invoke(session, gmCommand.Command, gmCommand.Parameters);
        }

        public static void Invoke(WorldSession session, ClientGmCommandString gmCommand)
        {
            Invoke(session, gmCommand.Command, gmCommand.Parameters);
        }

        private static void Invoke(WorldSession session, GmCommand command, GmCommandParameters parameters)
        {
            // TODO: check security level
            GmCommandHandler handler;
            if (!gmCommandHandlers.TryGetValue(command, out handler))
                return;

            handler.Invoke(session, parameters);
        }
    }
}