using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerActorAction3)]
    public class ServerActorAction3 : SubPacket
    {
        public ActorAction Action;
        public uint Parameter1;
        public uint Parameter2;
        public uint Parameter3;
        public ulong Parameter4;
        public uint Parameter5;
        public uint Parameter6;

        public override void Write(BinaryWriter writer)
        {
            writer.Write((uint)Action);
            writer.Write(Parameter1);
            writer.Write(Parameter2);
            writer.Write(Parameter3);
            writer.Write(Parameter4);
            writer.Write(Parameter5);
            writer.Write(Parameter6);
        }
    }
}
