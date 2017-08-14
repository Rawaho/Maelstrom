using Shared.Game;
using WorldServer.Game.Entity;

namespace WorldServer.Game.Event
{
    public abstract class EventScript
    {
        protected Player owner;

        public void Initialise(Player player)
        {
            owner = player;
        }

        public virtual uint GetParameter(EventType type) { return 0u; }

        public virtual void OnGossip(ulong actorId) { }
        public virtual void OnEmote(ulong actorId, ushort emoteId) { }
        public virtual void OnAreaTrigger(WorldPosition position) { }
        public virtual void OnOutOfBounds(WorldPosition position) { }
        public virtual void OnEventTerritory() { }
        public virtual void OnSceneFinish(ushort sceneId) { }
    }
}
