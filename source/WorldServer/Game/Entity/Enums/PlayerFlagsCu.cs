using System;

namespace WorldServer.Game.Entity.Enums
{
    [Flags]
    public enum PlayerFlagsCu : ushort
    {
        None       = 0x0000,
        FirstLogin = 0x0001,
        Invisible  = 0x0002
    }
}
