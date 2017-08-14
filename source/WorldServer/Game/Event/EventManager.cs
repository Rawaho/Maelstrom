using System;
using System.Diagnostics;
using Shared.Game;
using WorldServer.Game.Entity;
using WorldServer.Network.Message;
using WorldServer.Script;

namespace WorldServer.Game.Event
{
    public class EventManager
    {
        private readonly Player owner;
        private Event activeEvent;

        public EventManager(Player player)
        {
            owner = player;
        }

        /// <summary>
        /// Initialise a new client event.
        /// </summary>
        public void NewEvent(uint eventId, EventType eventType, ulong actorId)
        {
            #if DEBUG
                Console.WriteLine($"{owner.Character.Name} starting event {eventId:X4} with type {eventType}.");
            #endif

            if (!ScriptManager.IsValidEvent(eventId))
                throw new ArgumentException($"Invalid event {eventId:X4}!", nameof(eventId));

            if (activeEvent != null)
                throw new EventStateException($"Failed to start event {eventId:X4}, an existing event is still in progress!");

            activeEvent = new Event(eventId, eventType, actorId);
            activeEvent.Script?.Initialise(owner);

            owner.Session.Send(new ServerEventStart
            {
                Event = activeEvent,
                State = 0
            });

            // client softlocks waiting for event, to prevent this complete unhandled events instantly
            if (activeEvent.Script == null)
            {
                #if DEBUG
                    Console.WriteLine($"No assigned script for event {eventId:X4}!");
                #endif
                StopEvent();
            }
        }

        public void StopEvent()
        {
            #if DEBUG
                Console.WriteLine($"{owner.Character.Name} stopping event {activeEvent.Id:X4}.");
            #endif

            Debug.Assert(activeEvent != null);

            if (activeEvent.ActiveScene != null && !activeEvent.ActiveScene.IsComplete)
                activeEvent.SceneFinish();

            owner.Session.Send(new ServerEventStop
            {
                Event = activeEvent,
                State = 1
            });

            activeEvent = null;

            // TODO: handle player states properly
            owner.Session.Send(new ServerPlayerStateFlags
            {
            });
        }

        /// <summary>
        /// Starts a new scene for the current client event.
        /// </summary>
        public void NewScene(ushort sceneId, SceneFlags flags, uint unk1 = 0u, byte unk2 = 0, uint unk3 = 0u, uint unk4 = 0u, uint unk5 = 0u)
        {
            #if DEBUG
                Console.WriteLine($"{owner.Character.Name} starting scene {sceneId} for event {activeEvent.Id:X4}.");
            #endif

            if (activeEvent == null)
                throw new EventStateException($"Scene {sceneId} has no existing event in progress!");

            if (!ScriptManager.IsValidEventScene(activeEvent.Id, sceneId))
                throw new ArgumentException($"Invalid scene {sceneId} for event {activeEvent.Id:X4}!", nameof(sceneId));

            activeEvent.SceneNew(sceneId, flags);

            owner.Session.Send(new ServerEventSceneStart
            {
                Event = activeEvent,
                Flags = flags,
                Unk1  = unk1,
                Unk2  = unk2,
                Unk3  = unk3,
                Unk4  = unk4,
                Unk5  = unk5
            });
        }

        public void OnSceneFinish(ushort sceneId)
        {
            #if DEBUG
                Console.WriteLine($"{owner.Character.Name} finishing scene {activeEvent.ActiveScene.Id} for event {activeEvent.Id:X4}.");
            #endif

            if (activeEvent.ActiveScene.Id != sceneId)
                throw new EventStateException($"Scene {sceneId} doesn't match what the server expected!");

            if (activeEvent.ActiveScene.IsComplete)
                throw new EventStateException($"Scene {sceneId} is already complete!");

            activeEvent.SceneFinish();
        }

        public void OnGossip(uint eventId, ulong actorId)
        {
            NewEvent(eventId, EventType.Gossip, actorId);
            activeEvent?.Script.OnGossip(actorId);
        }

        public void OnEmote(uint eventId, ulong actorId, ushort emoteId)
        {
            // TODO
        }

        public void OnAreaTrigger(uint eventId, WorldPosition position)
        {
            // TODO
        }

        public void OnOutOfBounds(uint eventId, WorldPosition position)
        {
            NewEvent(eventId, EventType.OutOfBounds, owner.Character.ActorId);
            activeEvent?.Script.OnOutOfBounds(position);
        }

        public void OnTerritory(uint eventId)
        {
            NewEvent(eventId, EventType.Territory, owner.Character.ActorId);
            activeEvent?.Script.OnEventTerritory();
        }
    }
}
