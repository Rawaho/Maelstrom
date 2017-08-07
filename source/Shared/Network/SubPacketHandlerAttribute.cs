using System;

namespace Shared.Network
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubPacketHandlerAttribute : Attribute
    {
        public SubPacketOpcode Opcode { get; }
        public SubPacketType Type { get; }
        public SubPacketHandlerFlags Flags { get; }

        public SubPacketHandlerAttribute(SubPacketOpcode opcode, SubPacketHandlerFlags flags = SubPacketHandlerFlags.None)
        {
            Opcode = opcode;
            Flags  = flags;
        }

        public SubPacketHandlerAttribute(SubPacketType type, SubPacketHandlerFlags flags = SubPacketHandlerFlags.None)
        {
            Type  = type;
            Flags = flags;
        }
    }
}
