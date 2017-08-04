using System.Collections.ObjectModel;
using System.Linq;
using Shared.SqPack.GameTable;

namespace Shared.SqPack
{
    public static class GameTableManager
    {
        public static ExdTable<AchievementEntry> Achievements { get; private set; }
        public static ExdTable<ClassJobEntry> ClassJobs { get; private set; }
        public static ExdTable<EquipSlotCategoryEntry> EquipSlotCategories { get; private set; }
        public static ExdTable<ItemEntry> Items { get; private set; }
        public static ExdTable<ItemUiCategoryEntry> ItemUiCategories { get; private set; }
        public static ExdTable<PlaceNameEntry> PlaceNames { get; private set; }
        public static ExdTable<RaceEntry> Races { get; private set; }
        public static ExdTable<TerritoryTypeEntry> TerritoryTypes { get; private set; }

        /// <summary>
        /// Contains AchievementEntry's grouped by CriteriaType.
        /// </summary>
        public static ReadOnlyDictionary<byte /*CriteriaType*/, ReadOnlyCollection<AchievementEntry>> AchievementXCriteriaType { get; private set; }

        /// <summary>
        /// Contains AchievementEntry's grouped by CriteriaCounterType.
        /// </summary>
        public static ReadOnlyDictionary<ushort /*CriteriaCounterType*/, ReadOnlyCollection<AchievementEntry>> AchievementXCriteriaCounterType { get; private set; }

        public static void Initialise()
        {
            string path = @"data\\exd\\";
            Achievements        = ExdTable<AchievementEntry>.Load($"{path}achievement.exh");
            ClassJobs           = ExdTable<ClassJobEntry>.Load($"{path}classjob.exh");
            EquipSlotCategories = ExdTable<EquipSlotCategoryEntry>.Load($"{path}equipslotcategory.exh");
            Items               = ExdTable<ItemEntry>.Load($"{path}item.exh");
            ItemUiCategories    = ExdTable<ItemUiCategoryEntry>.Load($"{path}itemuicategory.exh");
            PlaceNames          = ExdTable<PlaceNameEntry>.Load($"{path}placename.exh");
            Races               = ExdTable<RaceEntry>.Load($"{path}race.exh");
            TerritoryTypes      = ExdTable<TerritoryTypeEntry>.Load($"{path}territorytype.exh");
            
            AchievementXCriteriaType = new ReadOnlyDictionary<byte, ReadOnlyCollection<AchievementEntry>>(Achievements.GetValues(ExdLanguage.En)
                .GroupBy(e => e.CriteriaType)
                .OrderBy(g => g.Key)
                .ToDictionary(o => o.Key, o => new ReadOnlyCollection<AchievementEntry>(o.ToList())));
            
            AchievementXCriteriaCounterType = new ReadOnlyDictionary<ushort, ReadOnlyCollection<AchievementEntry>>(Achievements.GetValues(ExdLanguage.En)
                .Where(e => e.CriteriaType == 1)
                .GroupBy(w => w.CriteriaData.CriteriaCounterTypeId)
                .OrderBy(g => g.Key)
                .ToDictionary(o => (ushort)o.Key, o => new ReadOnlyCollection<AchievementEntry>(o.ToList())));
        }
    }
}
