using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerChat)]
    public class ServerChat : SubPacket
    {
        public ushort Type;
        public ulong CharacterId;
        public string Name;
        public string Message;
        
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Type);
            writer.Pad(2u);
            writer.Write(0);
            writer.Write(CharacterId);
            writer.Write(0);
            writer.WriteStringLength(Name, 0x20);
            writer.WriteStringLength(Message, 1028);
        }
    }
}
