using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message {
    [SubPacket(SubPacketOpcode.ServerContentFinderMemberStatus, SubPacketDirection.Server)]
    public class ServerContentFinderMemberStatus : SubPacket
    {
        public ushort ContentId;
        public byte Status;

        public byte CurrentTank;
        public byte CurrentDPS;
        public byte CurrentHealer;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.ContentId);
            writer.Pad(2);
            writer.Write(this.Status);
            writer.Write(this.CurrentTank);
            writer.Write(this.CurrentDPS);
            writer.Write(this.CurrentHealer);
            writer.Write(1); // unknown
            writer.Pad(3); // unknown
            writer.Pad(4); // unknown
        }
    }
}