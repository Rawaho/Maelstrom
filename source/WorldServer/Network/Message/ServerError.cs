using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerError)]
    public class ServerError : SubPacket
    {
        public ulong Sequence;
        public uint ErrorId;
        public uint Value;          // some error messages contain a parameter (eg: queue position)
        public ushort ExdErrorId;   // error.exd

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Sequence);
            writer.Write(ErrorId);
            writer.Write(Value);
            writer.Write(ExdErrorId);
            writer.Write((ushort)1);
            writer.Pad(516u);
        }
    }
}
