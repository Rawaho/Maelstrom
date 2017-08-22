using System.IO;
using Shared.Network;
using WorldServer.Game.Event;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerEventStop)]
    public class ServerEventStop : SubPacket
    {
        public Event Event;
        public byte State;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Event.Id);
            writer.Write((byte)Event.Type);
            writer.Write(State);
            writer.Pad(2u);
            writer.Write(Event.Parameter);
            writer.Pad(4u);
        }
    }
}
