using Shared.Network;
using System.IO;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerContentFinderNotify)]
    public class ServerContentFinderNotify : SubPacket
    {
        public uint State;
        public uint CancelledReason = 0x3;
        public byte ClassJobId;
        public ushort[] ContentId;
        public ContentFinderLanguage Languages;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(State);
            writer.Write(CancelledReason);
            writer.Write((uint)ClassJobId);
            writer.Write(0x20442); // unknown
            writer.Write((byte)Languages);
            writer.Pad(3); // unknown
            writer.Write((ushort)0xC6); // unknown
            
            for (var i = 0; i < ContentId.Length; i++)
            {
                if (i > 5)
                {
                    break;
                }

                writer.Write(ContentId[i]);   
            }
        }
    }
}
