using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using SaintCoinach;
using SaintCoinach.Ex;
using SaintCoinach.IO;
using SaintCoinach.Xiv;

namespace Shared.SqPack
{
    public static class GameTableManager
    {
        public static IXivSheet<Achievement> Achievements { get; private set; }
        public static IXivSheet<ClassJob> ClassJobs { get; private set; }
        public static IXivSheet<EquipSlotCategory> EquipSlotCategories { get; private set; }
        public static IXivSheet<Item> Items { get; private set; }
        public static IXivSheet<ItemUICategory> ItemUiCategories { get; private set; }
        public static IXivSheet<Quest> Quests { get; private set; }
        public static IXivSheet OpeningEvents { get; private set; }
        public static IXivSheet<PlaceName> PlaceNames { get; private set; }
        public static IXivSheet<Race> Races { get; private set; }
        public static IXivSheet<TerritoryType> TerritoryTypes { get; private set; }

        /// <summary>
        /// Contains AchievementEntry's grouped by CriteriaType.
        /// </summary>
        public static ReadOnlyDictionary<byte /*CriteriaType*/, ReadOnlyCollection<Achievement>> AchievementXCriteriaType { get; private set; }

        /// <summary>
        /// Contains AchievementEntry's grouped by CriteriaCounterType.
        /// </summary>
        public static ReadOnlyDictionary<ushort, ReadOnlyCollection<Achievement>> AchievementXCriteriaCounterType { get; private set; }

        private static ARealmReversed realm;

        private static void Initialise(string assetPath)
        {
            Console.WriteLine("Initialising GameTables...");

            realm = new ARealmReversed(assetPath, "SaintCoinach.History.zip", Language.English);
            realm.Packs.GetPack(new PackIdentifier("exd", PackIdentifier.DefaultExpansion, 0)).KeepInMemory = true;

            Console.WriteLine($"Version (Game): {realm.GameVersion}, Version (Definition): {realm.DefinitionVersion}");
        }

        private static IXivSheet<T> LoadSheet<T>() where T : XivRow
        {
            var sheet = realm.GameData.GetSheet<T>();
            sheet.Preload();
            return sheet;
        }

        public static void InitialiseLobby(string assetPath)
        {
            Initialise(assetPath);

            var sw = new Stopwatch();
            sw.Start();

            ClassJobs = LoadSheet<ClassJob>();
            Races     = LoadSheet<Race>();

            Console.WriteLine($"Initialised GameTables in {sw.ElapsedMilliseconds}ms.");
        }

        public static void InitialiseWorld(string assetPath)
        {
            Initialise(assetPath);

            var sw = new Stopwatch();
            sw.Start();

            Achievements        = LoadSheet<Achievement>();
            ClassJobs           = LoadSheet<ClassJob>();
            EquipSlotCategories = LoadSheet<EquipSlotCategory>();
            Items               = LoadSheet<Item>();
            ItemUiCategories    = LoadSheet<ItemUICategory>();
            Quests              = LoadSheet<Quest>();
            OpeningEvents       = realm.GameData.GetSheet("Opening");
            PlaceNames          = LoadSheet<PlaceName>();
            Races               = LoadSheet<Race>();
            TerritoryTypes      = LoadSheet<TerritoryType>();

            AchievementXCriteriaType = new ReadOnlyDictionary<byte, ReadOnlyCollection<Achievement>>(Achievements
                .GroupBy(e => e.Type)
                .OrderBy(g => g.Key)
                .ToDictionary(o => o.Key, o => new ReadOnlyCollection<Achievement>(o.ToList())));

            AchievementXCriteriaCounterType = new ReadOnlyDictionary<ushort, ReadOnlyCollection<Achievement>>(Achievements
                .Where(e => e.Type == 1)
                .GroupBy(w => w.Data[0])
                .OrderBy(g => g.Key)
                .ToDictionary(o => (ushort)o.Key, o => new ReadOnlyCollection<Achievement>(o.ToList())));

            Console.WriteLine($"Initialised GameTables in {sw.ElapsedMilliseconds}ms.");
        }
    }
}
