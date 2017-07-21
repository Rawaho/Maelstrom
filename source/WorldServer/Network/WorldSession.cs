using Shared.Network;
using WorldServer.Game;

namespace WorldServer.Network
{
    [Session(ConnectionChannel.World)]
    public class WorldSession : Session
    {
        public Player Player { get; set; }

        public override void Send(SubPacket subPacket)
        {
            uint actorId = Player?.Character.ActorId ?? 0u;
            Send(actorId, actorId, subPacket);
        }

        public override void Disconnect()
        {
            base.Disconnect();
            Player?.RemoveFromMap();
        }
    }
}
