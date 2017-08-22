using Shared.Network;
using System.IO;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerContentFinderRegister)]
    public class ServerContentFinderRegister : SubPacket
    {
        public uint Unknown1 = 0x381;
        public byte RouletteId;
        public byte Unknown2 = 0xC6;
        public ushort ContentId;
        
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write(RouletteId);
            writer.Write(Unknown2);
            writer.Write(ContentId);
        }
    }
}
