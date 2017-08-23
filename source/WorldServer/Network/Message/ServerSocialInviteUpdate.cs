using System.IO;
using Shared.Network;
using WorldServer.Game.Social;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerSocialInviteUpdate)]
    public class ServerSocialInviteUpdate : SubPacket
    {
        public ulong CharacterId;
        public uint UnixTime;
        public SocialType SocialType;
        public SocialInviteUpdateType UpdateType;
        public byte Flags = 0x01;
        public string Name;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(CharacterId);
            writer.Write(UnixTime);
            writer.Write((byte)SocialType);
            writer.Write((byte)0);
            writer.Write((byte)UpdateType);
            writer.Write(Flags);
            writer.WriteStringLength(Name, 0x20);
        }
    }
}
