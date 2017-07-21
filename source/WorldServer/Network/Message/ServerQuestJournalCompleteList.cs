using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerQuestJournalCompleteList, SubPacketDirection.Server)]
    public class ServerQuestJournalCompleteList : SubPacket
    {
        public override void Write(BinaryWriter writer)
        {
            writer.Pad(408);
        }
    }
}
