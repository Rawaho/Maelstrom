using System.IO;
using Shared.Network;
using WorldServer.Game.Social;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerSocialInviteResponse)]
    public class ServerSocialInviteResponse : SubPacket
    {
        public SocialType SocialType;
        public byte Flags;
        public string Invitee;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(0u);
            writer.Write((byte)SocialType);
            writer.Write(Flags);
            writer.WriteStringLength(Invitee, 0x20);
            writer.Pad(2u);
        }
    }
}
