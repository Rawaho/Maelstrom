using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message {
    [SubPacket(SubPacketServerOpcode.ServerContentFinderMemberStatus)]
    public class ServerContentFinderMemberStatus : SubPacket
    {
        public ushort ContentId;
        public byte Status;

        public byte CurrentTank;
        public byte CurrentDPS;
        public byte CurrentHealer;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(ContentId);
            writer.Pad(2);
            writer.Write(Status);
            writer.Write(CurrentTank);
            writer.Write(CurrentDPS);
            writer.Write(CurrentHealer);
            writer.Write(1); // unknown
            writer.Pad(3); // unknown
            writer.Pad(4); // unknown
        }
    }
}