using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerQuestJournalCompleteList)]
    public class ServerQuestJournalCompleteList : SubPacket
    {
        public override void Write(BinaryWriter writer)
        {
            writer.Pad(408);
        }
    }
}
