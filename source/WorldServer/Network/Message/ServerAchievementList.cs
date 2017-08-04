using System.Collections;
using System.IO;
using System.Linq;
using Shared.Network;
using WorldServer.Game;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerAchievementList, SubPacketDirection.Server)]
    public class ServerAchievementList : SubPacket
    {
        public BitArray AchievementMask;
        public FixedQueue<ushort> LatestAchievements;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(AchievementMask.ToArray());

            foreach (ushort achievementId in LatestAchievements.Reverse())
                writer.Write(achievementId);
            
            writer.Pad(6u);
        }
    }
}
