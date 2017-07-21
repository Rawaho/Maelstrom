using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketType.ClientHelloWorld)]
    public class ClientHelloWorld : SubPacket
    {
        public uint ActorId;

        public override void Read(BinaryReader reader)
        {
            reader.ReadUInt32();
            string str = reader.ReadStringLength(0x24);
            uint.TryParse(str, out ActorId);
        }
    }
}
