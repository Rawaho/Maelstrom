using System.IO;
using Shared;
using Shared.Database;

namespace WorldServer.Manager
{
    public struct WorldConfig
    {
        public struct ConfigMySql
        {
            public ConfigMySqlDatabase Authentication { get; set; }
            public ConfigMySqlDatabase DataCentre { get; set; }
            public ConfigMySqlDatabase World { get; set; }
        }

        public struct ConfigServer
        {
            public int WorldPort { get; set; }
            public string AssetPath { get; set; }
        }

        public ConfigMySql MySql { get; set; }
        public ConfigServer Server { get; set; }
    }

    public static class ConfigManager
    {
        public static WorldConfig Config { get; private set; }

        public static void Initialise()
        {
            Config = JsonProvider.DeserialiseObject<WorldConfig>(File.ReadAllText(@".\WorldConfig.json"));
        }
    }
}
