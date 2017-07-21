using System;

namespace WorldServer.Network
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ActorActionHandlerAttribute : Attribute
    {
        public ActorAction Action { get; }

        public ActorActionHandlerAttribute(ActorAction action)
        {
            Action = action;
        }
    }
}
