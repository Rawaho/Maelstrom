using System;
using LobbyServer.Manager;
using Shared.Command;
using Shared.Database;
using Shared.Network;
using Shared.SqPack;

namespace LobbyServer
{
    internal class LobbyServer
    {
        #if DEBUG
            public const string Title = "Maelstrom: 4.05 - Lobby Server (Debug)";
        #else
            public const string Title = "Maelstrom: 4.05 - Lobby Server (Release)";
        #endif

        private static void Main()
        {
            Console.Title = Title;

            ConfigManager.Initialise();
            DatabaseManager.Initialise(ConfigManager.Config.MySql.Authentication, ConfigManager.Config.MySql.DataCentre, ConfigManager.Config.MySql.World);
            GameTableManager.Initialise();
            PacketManager.Initialise();
            NetworkManager.Initialise(ConfigManager.Config.Server.LobbyPort);
            AssetManager.Initalise();
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
