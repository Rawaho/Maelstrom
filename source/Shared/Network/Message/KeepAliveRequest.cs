using System.IO;

namespace Shared.Network.Message
{
    [SubPacket(SubPacketType.KeepAliveRequest, false)]
    public class KeepAliveRequest : SubPacket
    {
        public uint Check;
        public uint Timestamp;

        public override void Read(BinaryReader reader)
        {
            Check     = reader.ReadUInt32();
            Timestamp = reader.ReadUInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Check);
            writer.Write(Timestamp);
        }
    }
}
