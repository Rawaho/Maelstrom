using Shared.Network;

namespace WorldServer.Network
{
    [Session(ConnectionChannel.Chat)]
    public class ChatSession : Session
    {
        public override void Send(SubPacket subPacket)
        {
            Send(0u, 0u, subPacket);
        }
    }
}
