using System.Collections.Generic;
using System.IO;
using Shared.Database.Datacentre;
using Shared.Game;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerPlayerSpawn)]
    public class ServerPlayerSpawn : SubPacket
    {
        public byte SpawnIndex;
        public WorldPosition Position;
        public CharacterInfo Character;

        public ulong MainHandDisplayId;
        public ulong OffHandDisplayId;
        public IEnumerable<uint> VisibleItemDisplayIds;

        public override void Write(BinaryWriter writer)
        {
            writer.Write((ushort)0);    // title id
            writer.Write((ushort)0);
            writer.Write((byte)1);      // enables GM commands
            writer.Write((byte)0); 
            writer.Write((byte)0);
            writer.Write((byte)47);     // online Status
            writer.Write((byte)0);      // pose id
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write(0u);
            writer.Write(0ul);          // target
            writer.Write(0u);
            writer.Write(0u);

            // models
            writer.Write(MainHandDisplayId);
            writer.Write(OffHandDisplayId);
            writer.Write(0ul);          // crafting

            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);

            writer.Write(100u);         // HP
            writer.Write(100u);         // max HP
            writer.Write(0u);
            writer.Write((ushort)0);
            writer.Write((ushort)100);  // MP
            writer.Write((ushort)1000); // TP
            writer.Write((ushort)100);  // max MP
            writer.Write((ushort)0);
            writer.Write((ushort)0);

            writer.Write(Position.PackedOrientationShort);
            writer.Write((ushort)0);
            writer.Write(SpawnIndex);
            writer.Write((byte)1);      // state (1 = alive, 2 = dead, 3 = sitting)
            writer.Write((byte)0);
            writer.Write((byte)1);      // type (1 = Player, 2 = NPC, 3 = ??)
            writer.Write((byte)0);
            writer.Write(Character.Voice);
            writer.Write((ushort)0);
            writer.Write((byte)0);
            writer.Write((byte)Character.Classes[Character.ClassId].Level);
            writer.Write(Character.ClassJobId);
            writer.Write((byte)0);
            writer.Write((ushort)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write(0u);

            // aura
            for (uint i = 0u; i < 30u; i++)
            {
                writer.Write((ushort)0);
                writer.Pad(2u);
                writer.Write(0.0f);
                writer.Write(0u);
            }

            writer.Write(Position.Offset.X);
            writer.Write(Position.Offset.Y);
            writer.Write(Position.Offset.Z);
            
            foreach (uint displayId in VisibleItemDisplayIds)
                writer.Write(displayId);

            writer.WriteStringLength(Character.Name, 0x20u);
            writer.Write(Character.Appearance.Data);
            writer.WriteStringLength("FCTag", 6u);
            writer.Write(0u);
        }
    }
}
