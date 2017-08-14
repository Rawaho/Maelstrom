using System.Diagnostics;
using WorldServer.Script;

namespace WorldServer.Game.Event
{
    public class Event
    {
        public class Scene
        {
            public ushort Id { get; }
            public SceneFlags Flags { get; }
            public bool IsComplete { get; set; }

            public Scene(ushort id, SceneFlags flags)
            {
                Id    = id;
                Flags = flags;
            }
        }

        public uint Id { get; }
        public EventType Type { get; }
        public ulong ActorId { get; }
        public uint Parameter { get; }

        public EventScript Script { get; }
        public Scene ActiveScene { get; private set; }
        
        public Event(uint id, EventType type, ulong actorId)
        {
            Id        = id;
            Type      = type;
            ActorId   = actorId;
            Script    = ScriptManager.NewEventScript(id);
            Parameter = Script?.GetParameter(type) ?? 0u;
        }

        public void SceneNew(ushort sceneId, SceneFlags flags)
        {
            ActiveScene = new Scene(sceneId, flags);
        }

        public void SceneFinish()
        {
            Debug.Assert(!ActiveScene.IsComplete);
            ActiveScene.IsComplete = true;

            Script?.OnSceneFinish(ActiveScene.Id);
        }
    }
}
