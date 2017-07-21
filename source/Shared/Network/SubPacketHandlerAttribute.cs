using System;

namespace Shared.Network
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubPacketHandlerAttribute : Attribute
    {
        public SubPacketOpcode Opcode { get; }
        public SubPacketType Type { get; }

        public SubPacketHandlerAttribute(SubPacketOpcode opcode)
        {
            Opcode = opcode;
        }

        public SubPacketHandlerAttribute(SubPacketType type)
        {
            Type  = type;
        }
    }
}
