using System.IO;
using Shared.Network;
using WorldServer.Game.Entity.Enums;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientInventoryAction)]
    public class ClientInventoryAction : SubPacket
    {
        public InventoryAction Action { get; private set; }
        public ContainerType Source { get; private set; }
        public uint SourceStackSize { get; private set; }
        public ushort SourceSlot { get; private set; }
        public ContainerType Destination { get; private set; }
        public ushort DestinationSlot { get; private set; }
        public uint DestinationStackSize { get; private set; }

        public override void Read(BinaryReader reader)
        {
            reader.ReadUInt16();
            reader.ReadUInt16();
            Action = (InventoryAction)reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadUInt32();
            
            Source = (ContainerType)reader.ReadUInt32();
            SourceSlot = reader.ReadUInt16();
            reader.ReadUInt16();
            SourceStackSize = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            
            Destination = (ContainerType)reader.ReadUInt32();
            DestinationSlot = reader.ReadUInt16();
            reader.ReadUInt16();
            DestinationStackSize = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
        }
    }
}
