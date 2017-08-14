using System;

namespace WorldServer.Game.Event
{
    [Flags]
    public enum SceneFlags : uint
    {
        None          = 0x00000000,
        HideInterface = 0x00000800
    }
}
