using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerSocialAction)]
    public class ServerSocialAction : SubPacket
    {
        public ulong ObjectParameter1;
        public ulong ObjectParameter2;
        public byte Byte1;
        public byte Byte2;
        public byte Action;
        public byte Byte4;
        public string PlayerParameter1;
        public string PlayerParameter2;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(ObjectParameter1);
            writer.Write(ObjectParameter2);
            writer.Write(Byte1);
            writer.Write(Byte2);
            writer.Write(Action);
            writer.Write(Byte4);
            writer.WriteStringLength(PlayerParameter1, 0x20);
            writer.WriteStringLength(PlayerParameter2, 0x20);
        }
    }
}
