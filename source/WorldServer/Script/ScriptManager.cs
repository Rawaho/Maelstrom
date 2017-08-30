using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Shared.SqPack;
using WorldServer.Game.Event;

namespace WorldServer.Script
{
    public static class ScriptManager
    {
        private static ReadOnlyDictionary<uint, Type> eventScripts;

        public static void Initialise()
        {
            InitialiseEventScripts();
        }

        private static void InitialiseEventScripts()
        {
            var scripts = new Dictionary<uint, Type>();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                foreach (EventScriptAttribute attribute in type.GetCustomAttributes<EventScriptAttribute>())
                    scripts.Add(attribute.EventId, type);

            eventScripts = new ReadOnlyDictionary<uint, Type>(scripts);
        }

        public static bool IsValidEvent(uint eventId)
        {
            switch ((EventHiType)(eventId >> 16))
            {
                case EventHiType.Quest:
                    return GameTableManager.Quests.ContainsRow((int)eventId);
                case EventHiType.Opening:
                    return GameTableManager.OpeningEvents.ContainsRow((int)eventId);
                default:
                    throw new ArgumentException($"Unhandled EventHiType: {eventId >> 16}!", nameof(eventId));
            }
        }

        public static bool IsValidEventScene(uint eventId, ushort sceneId)
        {
            // TODO
            return true;
        }

        public static EventScript NewEventScript(uint eventId)
        {
            if (!eventScripts.TryGetValue(eventId, out Type scriptType))
            {
                #if DEBUG
                    Console.WriteLine($"Event {eventId} has no assigned script!");
                #endif
                return null;
            }

            return (EventScript)Activator.CreateInstance(scriptType);
        }
    }
}
