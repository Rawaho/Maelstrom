using System;

namespace Shared.Network
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubPacketHandlerAttribute : Attribute
    {
        public SubPacketClientOpcode ClientOpcode { get; }
        public SubPacketType Type { get; }
        public SubPacketHandlerFlags Flags { get; }

        public SubPacketHandlerAttribute(SubPacketClientOpcode opcode, SubPacketHandlerFlags flags = SubPacketHandlerFlags.None)
        {
            ClientOpcode = opcode;
            Flags        = flags;
        }

        public SubPacketHandlerAttribute(SubPacketType type, SubPacketHandlerFlags flags = SubPacketHandlerFlags.None)
        {
            Type  = type;
            Flags = flags;
        }
    }
}
