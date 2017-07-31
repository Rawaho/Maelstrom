using System.Collections.Generic;
using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerActorAppearanceUpdate, SubPacketDirection.Server)]
    public class ServerActorAppearanceUpdate : SubPacket
    {
        public ulong MainHandDisplayId;
        public ulong OffHandDisplayId;
        public IEnumerable<uint> VisibleItemDisplayIds;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(MainHandDisplayId);
            writer.Write(OffHandDisplayId);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Pad(1u);

            foreach (uint displayId in VisibleItemDisplayIds)
                writer.Write(displayId);
            
            writer.Pad(4u);
        }
    }
}
