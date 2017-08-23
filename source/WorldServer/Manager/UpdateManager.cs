using System;
using System.Diagnostics;
using System.Threading;
using Shared.Network;
using WorldServer.Game.Map;
using WorldServer.Game.Social;

namespace WorldServer.Manager
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
                    SocialManager.Update(lastTick);
                    MapManager.Update();

                    Thread.Sleep(1);
                    lastTick = (double)sw.ElapsedTicks / Stopwatch.Frequency;

                    #if DEBUG
                        Console.Title = $"{WorldServer.Title} (Update Time: {lastTick})";
                    #endif
                }
            })
            {
                IsBackground = true
            }.Start();
        }
    }
}
