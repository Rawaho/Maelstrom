using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientGmCommandString)]
    public class ClientGmCommandString : SubPacket
    {
        public GmCommand Command { get; private set; }
        public GmCommandParameters Parameters { get; private set; }

        public override void Read(BinaryReader reader)
        {
            Command    = (GmCommand)reader.ReadUInt32();
            Parameters = new GmCommandParameters(reader.ReadBytes(16), 0, reader.ReadStringLength(0x20u, true));
            reader.Skip(20u);
        }
    }
}
