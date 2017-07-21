using Shared.SqPack.GameTable;

namespace Shared.SqPack
{
    public static class GameTableManager
    {
        public static ExdTable<ClassJobEntry> ClassJobs { get; private set; }
        public static ExdTable<PlaceNameEntry> PlaceNames { get; private set; }
        public static ExdTable<RaceEntry> Races { get; private set; }
        public static ExdTable<TerritoryTypeEntry> TerritoryTypes { get; private set; }

        public static void Initialise()
        {
            ClassJobs      = ExdTable<ClassJobEntry>.Load(@"data\\exd\\classjob.exh");
            PlaceNames     = ExdTable<PlaceNameEntry>.Load(@"data\\exd\\placename.exh");
            Races          = ExdTable<RaceEntry>.Load(@"data\\exd\\race.exh");
            TerritoryTypes = ExdTable<TerritoryTypeEntry>.Load(@"data\\exd\\territorytype.exh");
        }
    }
}
