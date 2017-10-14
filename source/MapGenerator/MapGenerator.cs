using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SaintCoinach.Xiv;
using Shared.SqPack;

namespace MapGenerator
{
    internal static class MapGenerator
    {
        public const string DebugDirectory    = "DebugMesh";
        public const string NaviMeshDirectory = "NaviMesh";

        public const string Title = "Maelstrom Navigation Mesh Generator";

        private static Parameters parameters;

        static void Main(string[] args)
        {
            Console.Title = Title;

            ParseParameters(args);
            if (parameters.AssetPath == null)
                throw new ArgumentException("No asset path supplied!");

            // debug mesh output directory
            if (parameters.SaveDebugMesh && !Directory.Exists(DebugDirectory))
                Directory.CreateDirectory(DebugDirectory);

            // navigation mesh output directory
            if (!Directory.Exists(NaviMeshDirectory))
                Directory.CreateDirectory(NaviMeshDirectory);

            GameTableManager.InitialiseMapGenerator(parameters.AssetPath);

            var sw = new Stopwatch();
            sw.Start();

            if (parameters.Territory == -1)
            {
                // all territories
                var territoryTypes = GameTableManager.TerritoryTypes.Where(t => t.Name != string.Empty).ToList();

                Console.WriteLine($"Building {territoryTypes.Count} navigation meshes...");
                for (int i = 0; i < territoryTypes.Count; i++)
                {
                    Console.Title = $"{Title} - Processing: ({i + 1}/{territoryTypes.Count})";
                    HandleTerritory(territoryTypes[i]);

                    GC.Collect();
                }
            }
            else
            {
                // single territory
                var territoryType = GameTableManager.TerritoryTypes.SingleOrDefault(t => t.Key == parameters.Territory);
                if (territoryType == null)
                    throw new ArgumentOutOfRangeException();

                Console.WriteLine("Building single navigation mesh...");
                HandleTerritory(territoryType);
            }

            Console.WriteLine($"Finished generating navigation maps in {sw.ElapsedMilliseconds}ms!");
            Console.ReadLine();
        }

        private static void HandleTerritory(TerritoryType territoryType)
        {
            try
            {
                TerritoryMesh territoryMesh = new TerritoryMesh(territoryType);
                territoryMesh.BuildTerrainCollisionMesh(parameters.SaveDebugMesh);
                territoryMesh.BuildNavigationMesh(parameters.SingleNavigationMesh);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[{territoryType.Name}] {exception.Message}");
            }
        }

        private static void ParseParameters(string[] args)
        {
            if (args.Length < 1)
                throw new ArgumentException("Invalid parameter count!");

            parameters = new Parameters();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-AssetPath":
                        parameters.AssetPath = args[++i];
                        break;
                    case "-Territory":
                        if (int.TryParse(args[++i], out int territory))
                            parameters.Territory = territory;
                        break;
                    case "-SingleNavigationMesh":
                        parameters.SingleNavigationMesh = true;
                        break;
                    case "-SaveDebugMesh":
                        parameters.SaveDebugMesh = true;
                        break;
                }
            }
        }
    }
}
