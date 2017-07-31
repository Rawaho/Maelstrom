using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shared.Game;
using Shared.Network;
using WorldServer.Game.Map;
using WorldServer.Network.Message;

namespace WorldServer.Game.Entity
{
    public abstract class Actor
    {
        public uint Id { get; }
        public ActorType Type { get; }
        public WorldPosition Position { get; protected set; }
        public Territory Map { get; set; }

        public bool InWorld { get; private set; }

        public bool IsPlayer => Type == ActorType.Player;
        public bool IsNpc => Type == ActorType.Npc;
        public Player ToPlayer => IsPlayer ? (Player)this : null;

        protected readonly HashSet<Actor> visibleActors = new HashSet<Actor>();

        protected Actor(uint id, ActorType type)
        {
            Id   = id;
            Type = type;
        }

        public void AddToMap()
        {
            Debug.Assert(Position != null);
            MapManager.AddToMap(this);
        }

        public virtual void OnAddToMap()
        {
            InWorld = true;
            UpdateVision();
        }

        public void RemoveFromMap()
        {
            Debug.Assert(Map != null);
            Map?.RemoveActor(this);
        }

        public virtual void OnRemoveFromMap()
        {
            // TODO: broadcast removal
            ClearVision();
            InWorld = false;
        }

        public void Relocate(WorldPosition worldPosition)
        {
            // TODO: validate position
            Map.RelocateActor(this, worldPosition);
        }

        public virtual void OnRelocate(WorldPosition newPosition)
        {
            Position.Relocate(newPosition);
            UpdateVision();

            SendMessageToVisible(new ServerActorMove
            {
                Position = new WorldPosition(Map.Id, Position.Offset, Position.Orientation),
            });
        }

        public void UpdateVision()
        {
            List<Actor> intersectedActors;
            Map.Search(Position, 64f, new SearchCheckRange(Position, 64f), out intersectedActors);

            var actorsToRemove = new List<Actor>(visibleActors);
            foreach (Actor actor in intersectedActors)
            {
                if (!visibleActors.Contains(actor))
                {
                    AddVisibleActor(actor);
                    if (actor != this)
                        actor.AddVisibleActor(this);
                }
                else
                    actorsToRemove.Remove(actor);
            }

            foreach (Actor actor in actorsToRemove)
            {
                RemoveVisibleActor(actor);
                if (actor != this)
                    actor.RemoveVisibleActor(this);
            }
        }

        protected void ClearVision()
        {
            foreach (Actor actor in visibleActors)
                if (actor != this)
                    actor.RemoveVisibleActor(this);
            
            visibleActors.Clear();
        }

        public virtual void AddVisibleActor(Actor actor)
        {
            visibleActors.Add(actor);
        }

        public virtual void RemoveVisibleActor(Actor actor)
        {
            visibleActors.Remove(actor);
        }

        public void SendMessageToVisible(SubPacket subPacket, bool self = false)
        {
            foreach (Actor actor in visibleActors.Where(a => a.IsPlayer))
                actor.ToPlayer.Session.Send(Id, actor.Id, subPacket);
        }
    }
}
