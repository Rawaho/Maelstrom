using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerSocialBlacklist)]
    public class ServerSocialBlacklist : SubPacket
    {
        public override void Write(BinaryWriter writer)
        {
            for (int i = 0; i < 20; i++)
            {
                writer.Write(0ul); // characterId
                writer.WriteStringLength("", 0x20);
            }

            writer.Write(0ul);
        }
    }
}
