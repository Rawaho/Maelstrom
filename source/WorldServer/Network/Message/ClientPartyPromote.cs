using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientPartyPromote)]
    public class ClientPartyPromote : SubPacket
    {
        public string Name { get; private set; }

        public override void Read(BinaryReader reader)
        {
            Name = reader.ReadStringLength(0x20, true);
            reader.Skip(16u);
        }
    }
}
