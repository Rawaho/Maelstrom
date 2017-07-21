using System;

namespace WorldServer.Network
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GmCommandHandlerAttribute : Attribute
    {
        public GmCommand Command { get; }

        public GmCommandHandlerAttribute(GmCommand command)
        {
            Command = command;
        }
    }
}
