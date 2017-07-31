using System.IO;
using Shared.Network;
using WorldServer.Game.Entity;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerItemUpdate, SubPacketDirection.Server)]
    public class ServerItemUpdate : SubPacket
    {
        public uint Index;
        public ContainerType ContainerType;
        public ushort Slot;
        public uint StackSize;
        public uint ItemId;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(0u);
            writer.Write((ushort)ContainerType);
            writer.Write(Slot);
            writer.Write(StackSize);
            writer.Write(ItemId);
            writer.Pad(14u);
            writer.Write(30000);
            writer.Pad(26u);
        }
    }
}
