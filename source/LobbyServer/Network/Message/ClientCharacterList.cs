using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientCharacterList)]
    public class ClientCharacterList : SubPacket
    {
        public ulong Sequence { get; private set; }
        public byte ServiceAccount { get; private set; }

        public override void Read(BinaryReader reader)
        {
            Sequence       = reader.ReadUInt64();
            ServiceAccount = reader.ReadByte();
            reader.ReadUInt32();
            reader.Skip(3u);
            reader.ReadUInt64();
        }
    }
}
