using System.IO;
using Shared.Network;
using WorldServer.Game;
using WorldServer.Game.Social;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerSocialMessage)]
    public class ServerSocialMessage : SubPacket
    {
        public LogMessage LogMessage;
        public SocialType SocialType;
        public byte Byte1;
        public byte Byte2;
        public string Name;

        public override void Write(BinaryWriter writer)
        {
            writer.Write((uint)LogMessage);
            writer.Write((byte)SocialType);
            writer.Write(Byte1);
            writer.Write(Byte2);
            writer.WriteStringLength(Name, 0x20);
            writer.Pad(1u);
        }
    }
}
