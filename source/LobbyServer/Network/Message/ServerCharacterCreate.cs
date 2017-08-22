using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerCharacterCreate)]
    public class ServerCharacterCreate : SubPacket
    {
        public ulong Sequence;
        public byte Type;
        public ulong CharacterId;
        public string Name;
        public string Realm;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Sequence);
            writer.Write((byte)1);
            writer.Write((byte)1);
            writer.Write(Type);
            writer.Pad(1u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(CharacterId);
            writer.Write((ushort)1);
            writer.Write((ushort)1);
            writer.Write(0u);
            writer.Write((ushort)0);
            writer.WriteStringLength(Name, 0x20);
            writer.WriteStringLength(Realm, 0x20);
            writer.Pad(0x0866); // multiple?
        }
    }
}
