using Shared.SqPack.GameTable;

namespace Shared.SqPack
{
    public static class GameTableManager
    {
        public static ExdTable<ClassJobEntry> ClassJobs { get; private set; }
        public static ExdTable<EquipSlotCategoryEntry> EquipSlotCategories { get; private set; }
        public static ExdTable<ItemEntry> Items { get; private set; }
        public static ExdTable<ItemUiCategoryEntry> ItemUiCategories { get; private set; }
        public static ExdTable<PlaceNameEntry> PlaceNames { get; private set; }
        public static ExdTable<RaceEntry> Races { get; private set; }
        public static ExdTable<TerritoryTypeEntry> TerritoryTypes { get; private set; }
        
        public static void Initialise()
        {
            string path = @"data\\exd\\";
            ClassJobs           = ExdTable<ClassJobEntry>.Load($"{path}classjob.exh");
            EquipSlotCategories = ExdTable<EquipSlotCategoryEntry>.Load($"{path}equipslotcategory.exh");
            Items               = ExdTable<ItemEntry>.Load($"{path}item.exh");
            ItemUiCategories    = ExdTable<ItemUiCategoryEntry>.Load($"{path}itemuicategory.exh");
            PlaceNames          = ExdTable<PlaceNameEntry>.Load($"{path}placename.exh");
            Races               = ExdTable<RaceEntry>.Load($"{path}race.exh");
            TerritoryTypes      = ExdTable<TerritoryTypeEntry>.Load($"{path}territorytype.exh");
        }
    }
}
