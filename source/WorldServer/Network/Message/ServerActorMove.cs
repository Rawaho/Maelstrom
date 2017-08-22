using System.IO;
using System.Numerics;
using Shared.Game;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerActorMove, false)]
    public class ServerActorMove : SubPacket
    {
        public WorldPosition Position;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Position.PackedOrientationByte);
            writer.Write((byte)0x7F);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0x3C);
            writer.Write((byte)0xEA);

            Vector3 packedOffset = Position.PackedOffsetShort;
            writer.Write((ushort)packedOffset.X);
            writer.Write((ushort)packedOffset.Y);
            writer.Write((ushort)packedOffset.Z);

            writer.Write(0u);
        }
    }
}
