using System.IO;
using Shared.Network;
using WorldServer.Game.Social;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientSocialInviteResponse)]
    public class ClientSocialInviteResponse : SubPacket
    {
        public ulong CharacterId { get; private set; }
        public SocialType SocialType { get; private set; }
        public byte Result { get; private set; }

        public override void Read(BinaryReader reader)
        {
            CharacterId = reader.ReadUInt64();
            SocialType  = (SocialType)reader.ReadByte();
            Result      = reader.ReadByte();
            reader.Skip(2u);
            reader.ReadUInt32();
        }
    }
}
