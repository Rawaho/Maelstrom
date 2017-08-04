using System;

namespace WorldServer.Game.Achievement
{
    public class Achievement
    {
        public uint AchievementId { get; }
        public ulong Timestamp { get; }

        public Achievement(uint achievementId)
        {
            AchievementId = achievementId;
            Timestamp     = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
