using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerActorAction2, SubPacketDirection.Server)]
    public class ServerActorAction2 : SubPacket
    {
        public ActorAction Action;
        public uint Parameter1;
        public uint Parameter2;
        public uint Parameter3;
        public uint Parameter4;

        public override void Write(BinaryWriter writer)
        {
            writer.Write((uint)Action);
            writer.Write(Parameter1);
            writer.Write(Parameter2);
            writer.Write(Parameter3);
            writer.Write(Parameter4);
        }
    }
}
