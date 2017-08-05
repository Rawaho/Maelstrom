using System;
using Shared.Game.Enum;
using Shared.Network;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class ContentFinderHandler
    {
        [SubPacketHandler(SubPacketOpcode.ClientContentFinderRequestInfo)]
        public static void HandleChat(WorldSession session, ClientContentFinderRequestInfo info)
        {
            #if DEBUG
            Console.WriteLine("Client requested content finder info");
            #endif
            
            // TODO
            session.Send(new ServerContentFinderDutyInfo {
                PenaltyTime = 0
            });
            
            session.Send(new ServerContentFinderPlayerInNeed {
                InNeed = new ClassJobRole[0x10]
            });
        }
    }
}
