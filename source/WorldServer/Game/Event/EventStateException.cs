using System;

namespace WorldServer.Game.Event
{
    public class EventStateException : Exception
    {
        public EventStateException(string message)
            : base(message)
        {
        }
    }
}
