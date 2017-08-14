using Shared.Game;
using WorldServer.Game.Entity.Enums;
using WorldServer.Game.Event;

namespace WorldServer.Script.Opening
{
    [EventScript(0x130001)]
    public class OpeningLimsaLominsa : EventScript
    {
        private static class Data
        {
            public const byte SceneOpening       = 0;
            public const byte SceneControlScheme = 1;
            public const byte SceneOutOfBounds   = 20;
            public const byte SceneLogin         = 40;
            public const uint NpcRyssfloh        = 0x3E9699;
        }

        public override uint GetParameter(EventType type)
        {
            switch (type)
            {
                case EventType.Territory:
                    return 0xB5;
                case EventType.OutOfBounds:
                    return Data.NpcRyssfloh;
                default:
                    return 0u;
            }
        }

        public override void OnOutOfBounds(WorldPosition position)
        {
            owner.Event.NewScene(Data.SceneOutOfBounds, (SceneFlags)0x2001, 0u, 1, Data.NpcRyssfloh, 9);
        }

        public override void OnEventTerritory()
        {
            if ((owner.FlagsCu & PlayerFlagsCu.FirstLogin) != 0)
                owner.Event.NewScene(Data.SceneOpening, (SceneFlags)0x4BAC05, 0u, 1);
            else
                owner.Event.NewScene(Data.SceneLogin, (SceneFlags)0x2001, 0u, 1, 1u, 1u);
        }

        public override void OnSceneFinish(ushort sceneId)
        {
            switch (sceneId)
            {
                case Data.SceneOpening:
                    owner.Event.NewScene(Data.SceneControlScheme, (SceneFlags)0x2001, 0u, 0, 2u, 0x2000u);
                    break;
                case Data.SceneControlScheme:
                case Data.SceneOutOfBounds:
                case Data.SceneLogin:
                    owner.Event.StopEvent();
                    break;
            }
        }
    }
}
