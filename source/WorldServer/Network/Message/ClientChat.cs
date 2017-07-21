using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.Chat, SubPacketDirection.Client)]
    public class ClientChat : SubPacket
    {
        public ushort Type { get; private set; }
        public string Message { get; private set; }

        public override void Read(BinaryReader reader)
        {
            reader.ReadUInt32();
            reader.ReadBytes(0x14);
            Type    = reader.ReadUInt16();
            Message = reader.ReadStringLength(0x0416);
        }
    }
}
