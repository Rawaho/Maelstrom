using System;

namespace Shared.Network
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SessionAttribute : Attribute
    {
        public ConnectionChannel Channel { get; }

        public SessionAttribute(ConnectionChannel channel)
        {
            Channel = channel;
        }
    }
}
