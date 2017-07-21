using System;

namespace Shared.Network
{
    public enum ConnectionHeartbeatResult
    {
        Ok,
        Pulse,
        Flatline
    }

    public class ConnectionHeartbeat
    {
        private const double ConnectionPulse    = 10d;
        private const double ConnectionFlatline = ConnectionPulse * 2;

        public uint Latency { get; private set; }

        private uint pulseTime;
        private double pulseTimer;
        private double flatlineTimer = ConnectionFlatline;

        public void CalculateLatency(uint responseTime)
        {
            if (responseTime >= pulseTime)
                Latency = responseTime - pulseTime;

            flatlineTimer = ConnectionFlatline;
        }
        
        public (ConnectionHeartbeatResult result, uint pulseTime) Update(double lastTick)
        {
            flatlineTimer -= lastTick;
            if (flatlineTimer <= 0d)
                return (ConnectionHeartbeatResult.Flatline, 0u);

            pulseTimer -= lastTick;
            if (pulseTimer <= 0d)
            {
                pulseTimer = ConnectionPulse;
                pulseTime  = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();
                return (ConnectionHeartbeatResult.Pulse, pulseTime);
            }
            
            return (ConnectionHeartbeatResult.Ok, 0u);
        }
    }
}
