using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientCharacterCreate)]
    public class ClientCharacterCreate : SubPacket
    {
        public ulong Sequence { get; private set; }
        public ushort RealmId { get; private set; }
        public byte Type { get; private set; }
        public string Name { get; private set; }
        public string Json { get; private set; }

        public override void Read(BinaryReader reader)
        {
            Sequence = reader.ReadUInt64();
            reader.Skip(0x11);
            Type     = reader.ReadByte();
            RealmId  = reader.ReadUInt16();
            Name     = reader.ReadStringLength(0x20);
            Json     = reader.ReadStringLength(0x194);
            reader.ReadUInt64();
            reader.ReadUInt64();
        }
    }
}
