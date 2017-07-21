using System;
using System.Collections.Generic;
using System.Reflection;
using WorldServer.Network.Message;

namespace WorldServer.Network
{
    public static class ActorActionManager
    {
        public delegate void ActorActionHandler(WorldSession session, ClientActorAction actorAction);

        private static readonly Dictionary<ActorAction, ActorActionHandler> actorActionHandlers = new Dictionary<ActorAction, ActorActionHandler>();

        public static void Initalise()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                foreach (MethodInfo methodInfo in type.GetMethods())
                    foreach (ActorActionHandlerAttribute attribute in methodInfo.GetCustomAttributes<ActorActionHandlerAttribute>())
                        actorActionHandlers[attribute.Action] = (ActorActionHandler)Delegate.CreateDelegate(typeof(ActorActionHandler), methodInfo);
        }

        public static void Invoke(WorldSession session, ClientActorAction actorAction)
        {
            ActorActionHandler handler;
            if (!actorActionHandlers.TryGetValue(actorAction.Action, out handler))
                return;

            handler.Invoke(session, actorAction);
        }
    }
}
