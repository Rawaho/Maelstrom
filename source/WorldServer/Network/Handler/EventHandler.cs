using Shared.Game;
using Shared.Network;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class EventHandler
    {
        [SubPacketHandler(SubPacketOpcode.ClientEventGossip, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleEventGossip(WorldSession session, ClientEventGossip eventGossip)
        {
            session.Player.Event.OnGossip(eventGossip.EventId, eventGossip.ActorId);
        }

        [SubPacketHandler(SubPacketOpcode.ClientEventEmote, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleEventEmote(WorldSession session, ClientEventEmote eventEmote)
        {
            session.Player.Event.OnEmote(eventEmote.EventId, eventEmote.ActorId, eventEmote.EmoteId);
        }

        [SubPacketHandler(SubPacketOpcode.ClientEventAreaTrigger, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleOnAreaTrigger(WorldSession session, ClientEventAreaTrigger eventAreaTrigger)
        {
            session.Player.Event.OnAreaTrigger(eventAreaTrigger.EventId, new WorldPosition(session.Player.Position.TerritoryId, eventAreaTrigger.Position, 0f));
        }

        [SubPacketHandler(SubPacketOpcode.ClientEventOutOfBounds, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleEventOutOfBounds(WorldSession session, ClientEventOutOfBounds eventOutOfBounds)
        {
            session.Player.Event.OnOutOfBounds(eventOutOfBounds.EventId, new WorldPosition(session.Player.Position.TerritoryId, eventOutOfBounds.Position, 0f));
        }

        [SubPacketHandler(SubPacketOpcode.ClientEventTerritory, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleEventTerritory(WorldSession session, ClientEventTerritory territory)
        {
            session.Player.Event.OnTerritory(territory.EventId);
        }

        [SubPacketHandler(SubPacketOpcode.ClientEventSceneFinish, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleEventSceneFinish(WorldSession session, ClientEventSceneFinish eventSceneFinish)
        {
            session.Player.Event.OnSceneFinish(eventSceneFinish.SceneId);
        }
    }
}
