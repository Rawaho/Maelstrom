using System;

namespace Shared.Network
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SubPacketAttribute : Attribute
    {
        public SubPacketClientOpcode ClientOpcode { get; }
        public SubPacketServerOpcode ServerOpcode { get; }
        public SubPacketType Type { get; }
        public bool Log { get; }

        public SubPacketAttribute(SubPacketClientOpcode opcode, bool log = true)
        {
            ClientOpcode = opcode;
            Log          = log;
        }

        public SubPacketAttribute(SubPacketServerOpcode opcode, bool log = true)
        {
            ServerOpcode = opcode;
            Log          = log;
        }

        public SubPacketAttribute(SubPacketType type, bool log = true)
        {
            Type = type;
            Log  = log;
        }
    }
}
