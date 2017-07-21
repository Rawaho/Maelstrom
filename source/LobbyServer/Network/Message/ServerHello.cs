using System.IO;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketType.ServerHello)]
    public class ServerHello : SubPacket
    {
        public uint SessionHash;
        
        // TODO: research this more
        public override void Write(BinaryWriter writer)
        {
            writer.Write(SessionHash);
            writer.Pad(232u);
            writer.Write(SessionHash);
            writer.Pad(208u);
            writer.Write(SessionHash);
            writer.Pad(40u);
            writer.Write(SessionHash);
            writer.Pad(144u);
        }
    }
}
