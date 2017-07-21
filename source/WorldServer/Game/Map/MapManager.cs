using System;
using System.Collections.Generic;
using System.Diagnostics;
using Shared.SqPack;
using Shared.SqPack.GameTable;

namespace WorldServer.Game.Map
{
    public static class MapManager
    {
        private static readonly Dictionary<uint, Territory> territories = new Dictionary<uint, Territory>();

        public static void Initialise()
        {
            var sw = new Stopwatch();
            sw.Start();

            foreach (TerritoryTypeEntry entry in GameTableManager.TerritoryTypes.GetValues(ExdLanguage.None))
            {
                if (entry.Name == string.Empty)
                    continue;

                territories.Add(entry.Index, new Territory(entry));
            }

            Console.WriteLine($"Initialised {territories.Count} map(s) in {sw.ElapsedMilliseconds}ms.");
        }

        public static void AddToMap(Actor actor)
        {
            if (actor.Position == null)
                return;

            if (!territories.TryGetValue(actor.Position.TerritoryId, out Territory territory))
                return;

            territory.AddActor(actor);
        }

        public static void Update()
        {
            // TODO: change this to a multithreaded implementation
            foreach (KeyValuePair<uint, Territory> pair in territories)
                pair.Value.Update(0d);
        }
    }
}
