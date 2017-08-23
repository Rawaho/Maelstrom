using System.IO;
using Shared.Network;
using WorldServer.Game.Social;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientSocialList)]
    public class ClientSocialList : SubPacket
    {
        public SocialListType ListType { get; private set; }
        public ushort Sequence { get; private set; }

        public override void Read(BinaryReader reader)
        {
            reader.Skip(10u);
            ListType = (SocialListType)reader.ReadByte();
            Sequence = reader.ReadByte();
            reader.Skip(4u);
        }
    }
}
