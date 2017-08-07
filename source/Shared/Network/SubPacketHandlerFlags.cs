using System;

namespace Shared.Network
{
    [Flags]
    public enum SubPacketHandlerFlags : byte
    {
        None               = 0x00,

        /// <summary>
        /// Ensures lobby session has encryption enabled.
        /// </summary>
        RequiresEncryption = 0x01,

        /// <summary>
        /// Ensures lobby session has an assigned service account.
        /// </summary>
        RequiresAccount    = 0x02,

        /// <summary>
        /// Ensures world session has an assigned player character.
        /// </summary>
        RequiresPlayer     = 0x04,

        /// <summary>
        /// Ensures world session has an assigned player character that has been added to a territory(map).
        /// </summary>
        RequiresWorld      = 0x08
    }
}
