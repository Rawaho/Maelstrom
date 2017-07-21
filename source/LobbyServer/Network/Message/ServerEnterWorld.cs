using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerEnterWorld, SubPacketDirection.Server)]
    public class ServerEnterWorld : SubPacket
    {
        public ulong Sequence;
        public uint ActorId;
        public ulong CharacterId;
        public string Token;
        public string Host;
        public ushort Port;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Sequence);
            writer.Write(ActorId);
            writer.Write(0u);
            writer.Write(CharacterId);
            writer.Write(0u);
            writer.WriteStringLength(Token, 0x42);
            writer.Write(Port);
            writer.WriteStringLength(Host, 0x30);
            writer.Write(0ul);
            writer.Write(0ul);
        }
    }
}
