namespace Shared.Network
{
    public class FloodInfo
    {
        // time in seconds before a new connection can be accepted if all slots full for address
        private const double NextConnectionTimeout = 30d;

        public byte Count { get; private set; }
        public double Timeout { get; private set; }

        public FloodInfo()
        {
            NewConnection();
        }

        public void NewConnection()
        {
            Count++;
            Timeout = NextConnectionTimeout;
        }
        
        public bool Update(double lastTick)
        {
            if (lastTick > Timeout)
            {
                Count--;
                if (Count == 0)
                    return true;

                Timeout = NextConnectionTimeout;
            }
            else
                Timeout -= lastTick;

            return false;
        }
    }
}
