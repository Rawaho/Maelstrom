using System.Collections;
using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerContentFinderDutyInfo, SubPacketDirection.Server)]
    public class ServerContentFinderDutyInfo : SubPacket {
        public byte PenaltyTime;
        
        public override void Write(BinaryWriter writer) {
            writer.Write(PenaltyTime);
            writer.Pad(3);
            writer.Write((byte)0x20); // Unknown
            writer.Pad(3);
        }
    }
}
 