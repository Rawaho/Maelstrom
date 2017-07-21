using System;
using System.Diagnostics;
using Shared.Command;
using Shared.Game;
using WorldServer.Network;

namespace WorldServer.Command
{
    public static class MiscHandler
    {
        [CommandHandler("grid_vision_test", SecurityLevel.Developer, 0)]
        public static void HandleGridVisionTest(WorldSession session, params string[] parameters)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            for (uint i = 0u; i < 1_000_000u; i++)
                session.Player.UpdateVision();

            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [CommandHandler("test", SecurityLevel.Developer, 1)]
        public static void HandleTest(WorldSession session, params string[] parameters)
        {
        }
    }
}
