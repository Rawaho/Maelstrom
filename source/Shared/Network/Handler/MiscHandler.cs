using System;
using Shared.Network.Message;

namespace Shared.Network.Handler
{
    public static class MiscHandler
    {
        [SubPacketHandler(SubPacketType.KeepAliveRequest)]
        public static void HandleClientKeepAliveRequest(Session session, KeepAliveRequest keepAlive)
        {
            session.Send(0u, 0u, new KeepAliveResponse
            {
                Check     = keepAlive.Check,
                Timestamp = (uint)DateTimeOffset.Now.ToUnixTimeSeconds()
            });
        }

        [SubPacketHandler(SubPacketType.KeepAliveResponse)]
        public static void HandleClientKeepAliveResponse(Session session, KeepAliveResponse keepAlive)
        {
            session.Heartbeat.CalculateLatency(keepAlive.Timestamp);
        }
    }
}
