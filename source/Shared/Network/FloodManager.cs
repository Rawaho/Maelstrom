using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace Shared.Network
{
    public static class FloodManager
    {
        private const uint MaxConnectionsPerAddress = 5u;
        
        private static readonly ConcurrentDictionary<string, FloodInfo> floodStore = new ConcurrentDictionary<string, FloodInfo>();

        public static bool CanAccept(IPAddress address)
        {
            if (!floodStore.TryGetValue(address.ToString(), out FloodInfo floodinfo))
                return true;

            return floodinfo.Count < MaxConnectionsPerAddress;
        }

        public static void Accept(IPAddress address)
        {
            if (floodStore.TryGetValue(address.ToString(), out FloodInfo floodInfo))
                floodInfo.NewConnection();
            else
                floodStore.TryAdd(address.ToString(), new FloodInfo());
        }

        public static void Update(double lastTick)
        {
            foreach (KeyValuePair<string, FloodInfo> pair in floodStore.ToArray())
                if (pair.Value.Update(lastTick))
                    floodStore.TryRemove(pair.Key, out FloodInfo oldInfo);
        }
    }
}
