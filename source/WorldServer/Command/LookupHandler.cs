using System;
using System.Collections.Generic;
using Shared.Command;
using Shared.Game;
using Shared.SqPack;
using Shared.SqPack.GameTable;
using WorldServer.Network;

namespace WorldServer.Command
{
    public static class LookupHandler
    {
        // lookup_territory name/partial name
        [CommandHandler("lookup_territory", SecurityLevel.Developer)]
        public static void HandleActorTeleport(WorldSession session, params string[] parameters)
        {
            if (parameters.Length == 0)
                return;

            string searchString = string.Join(" ", parameters);

            var matches = new List<(uint Index, string Name)>();
            foreach (TerritoryTypeEntry territoryEntry in GameTableManager.TerritoryTypes.GetValues(ExdLanguage.None))
            {
                if (territoryEntry.Name == string.Empty)
                    continue;

                if (!GameTableManager.PlaceNames.TryGetValue(territoryEntry.PlaceNameId, ExdLanguage.En, out PlaceNameEntry placeNameEntry))
                    continue;

                if (placeNameEntry.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    matches.Add((territoryEntry.Index, placeNameEntry.Name));
            }

            foreach ((uint Index, string Name) match in matches)
                Console.WriteLine($"Match: {match.Index} - {match.Name}");
        }
    }
}
