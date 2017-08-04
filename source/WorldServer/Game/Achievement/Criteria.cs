using System;

namespace WorldServer.Game.Achievement
{
    public class Criteria
    {
        public uint Value { get; private set; }
        public long StartTimestamp { get; }
        public long UpdateTimestamp { get; private set; }

        public Criteria(uint value)
        {
            Value          = value;
            StartTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public void IncrementValue(uint value)
        {
            checked
            {
                Value += value;
            }
            
            UpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
