using System.IO;
using Shared.Network;
using WorldServer.Game.Event;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerEventStart, SubPacketDirection.Server)]
    public class ServerEventStart : SubPacket
    {
        public Event Event;
        public byte State;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Event.ActorId);
            writer.Write(Event.Id);
            writer.Write((byte)Event.Type);
            writer.Write(State);
            writer.Pad(2u);
            writer.Write(Event.Parameter);
            writer.Pad(4u);
        }
    }
}
