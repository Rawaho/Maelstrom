using System.IO;
using Shared.Network;
using WorldServer.Game.Social;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientSocialInvite)]
    public class ClientSocialInvite : SubPacket
    {
        public SocialType SocialType { get; private set; }
        public string Invitee { get; private set; }

        public override void Read(BinaryReader reader)
        {
            SocialType = (SocialType)reader.ReadByte();
            Invitee    = reader.ReadStringLength(0x20);
            reader.Skip(3u);
            reader.ReadUInt32();
        }
    }
}
