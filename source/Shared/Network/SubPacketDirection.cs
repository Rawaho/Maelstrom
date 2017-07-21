using System;

namespace Shared.Network
{
    [Flags]
    public enum SubPacketDirection
    {
        Client = 0x01, // C->S
        Server = 0x02  // S->C
    }
}
