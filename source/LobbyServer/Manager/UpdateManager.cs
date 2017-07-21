using System;
using System.Diagnostics;
using System.Threading;
using Shared.Network;

namespace LobbyServer.Manager
{
    public static class UpdateManager
    {
        public static void Initialise()
        {
            new Thread(() =>
            {
                var sw = new Stopwatch();
                double lastTick = 0d;

                while (true)
                {
                    sw.Restart();
                    
                    NetworkManager.Update(lastTick);
                    FloodManager.Update(lastTick);

                    Thread.Sleep(1);
                    lastTick = (double)sw.ElapsedTicks / Stopwatch.Frequency;

                    #if DEBUG
                        Console.Title = $"{LobbyServer.Title} (Update Time: {lastTick})";
                    #endif
                }
            })
            {
                IsBackground = true
            }.Start();
        }
    }
}
