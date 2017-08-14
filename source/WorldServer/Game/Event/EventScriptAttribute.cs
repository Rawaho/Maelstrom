using System;

namespace WorldServer.Game.Event
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventScriptAttribute : Attribute
    {
        public uint EventId { get; }

        public EventScriptAttribute(uint eventId)
        {
            EventId = eventId;
        }
    }
}
