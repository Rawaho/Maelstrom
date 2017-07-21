using System;

namespace Shared.Network
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SubPacketAttribute : Attribute
    {
        public SubPacketOpcode Opcode { get; }
        public SubPacketType Type { get; }
        public SubPacketDirection Direction { get; }
        public bool Log { get; }

        public SubPacketAttribute(SubPacketOpcode opcode, SubPacketDirection direction, bool log = true)
        {
            Opcode    = opcode;
            Direction = direction;
            Log       = log;
        }

        public SubPacketAttribute(SubPacketType type, bool log = true)
        {
            Type = type;
            Log  = log;
        }
    }
}
