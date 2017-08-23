using System;
using Shared.Command;
using Shared.Database;
using Shared.Network;
using Shared.SqPack;
using WorldServer.Game.Map;
using WorldServer.Game.Social;
using WorldServer.Manager;
using WorldServer.Network;
using WorldServer.Script;

namespace WorldServer
{
    internal class WorldServer
    {
        #if DEBUG
            public const string Title = "Maelstrom: 4.05 - World Server (Debug)";
        #else
            public const string Title = "Maelstrom: 4.05 - World Server (Release)";
        #endif

        private static void Main()
        {
            Console.Title = Title;

            ConfigManager.Initialise();
            DatabaseManager.Initialise(ConfigManager.Config.MySql.Authentication, ConfigManager.Config.MySql.DataCentre, ConfigManager.Config.MySql.World);
            GameTableManager.Initialise();
            PacketManager.Initialise();
            ActorActionManager.Initalise();
            GmCommandManager.Initalise();
            AssetManager.Initialise();
            ScriptManager.Initialise();
            MapManager.Initialise();
            SocialManager.Initialise();
            NetworkManager.Initialise(ConfigManager.Config.Server.WorldPort);
            UpdateManager.Initialise();
            CommandManager.Initialise();
        }

        public static void Shutdown()
        {
            NetworkManager.Shutdown = true;
            // remaining managers are background threads
        }
    }
}
