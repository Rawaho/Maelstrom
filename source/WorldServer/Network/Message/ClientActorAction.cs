using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ClientActorAction, SubPacketDirection.Client, false)]
    public class ClientActorAction : SubPacket
    {
        public ActorAction Action { get; private set; }
        public uint[] Parameters { get; } = new uint[5];

        public override void Read(BinaryReader reader)
        {
            Action = (ActorAction)reader.ReadUInt16();
            reader.ReadUInt16();

            for (uint i = 0u; i < 5u; i++)
                Parameters[i] = reader.ReadUInt32();
        }
    }
}
