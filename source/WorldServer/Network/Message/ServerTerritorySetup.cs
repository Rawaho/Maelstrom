using System.IO;
using Shared.Game;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerTerritorySetup)]
    public class ServerTerritorySetup : SubPacket
    {
        public byte WeatherId;
        public WorldPosition WorldPosition;

        public override void Write(BinaryWriter writer)
        {
            writer.Write((ushort)0);
            writer.Write(WorldPosition.TerritoryId);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(WeatherId);
            writer.Write((byte)1);
            writer.Write((byte)0x2A);
            writer.Write((byte)0xE1);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(WorldPosition.Offset.X);
            writer.Write(WorldPosition.Offset.Y);
            writer.Write(WorldPosition.Offset.Z);
        }
    }
}
