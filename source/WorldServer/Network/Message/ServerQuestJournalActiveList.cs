using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerQuestJournalActiveList, SubPacketDirection.Server)]
    public class ServerQuestJournalActiveList : SubPacket
    {
        public override void Write(BinaryWriter writer)
        {
            writer.Pad(360);
        }
    }
}
