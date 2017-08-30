using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SaintCoinach.Xiv;
using Shared.SqPack;
using WorldServer.Game.Entity;

namespace WorldServer.Game.Map
{
    public static class MapManager
    {
        private static readonly Dictionary<uint, Territory> territories = new Dictionary<uint, Territory>();

        private static readonly List<Player> players = new List<Player>();
        private static readonly ReaderWriterLockSlim playerMutex = new ReaderWriterLockSlim();

        public static void Initialise()
        {
            var sw = new Stopwatch();
            sw.Start();

            foreach (TerritoryType entry in GameTableManager.TerritoryTypes)
            {
                if (entry.Name == string.Empty)
                    continue;

                territories.Add((uint)entry.Key, new Territory(entry));
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

        public static void _AddPlayer(Player player)
        {
            try
            {
                playerMutex.EnterWriteLock();
                players.Add(player);
            }
            finally
            {
                playerMutex.ExitWriteLock();
            }
        }

        public static void _RemovePlayer(Player player)
        {
            try
            {
                playerMutex.EnterWriteLock();
                players.Remove(player);
            }
            finally
            {
                playerMutex.ExitWriteLock();
            }
        }

        /// <summary>
        /// Find a player in the world (any territory).
        /// </summary>
        public static Player FindPlayer(string name)
        {
            try
            {
                playerMutex.EnterReadLock();
                return players.SingleOrDefault(p => p.Character.Name == name);
            }
            finally
            {
                playerMutex.ExitReadLock();
            }
        }

        /// <summary>
        /// Find a player in the world (any territory).
        /// </summary>
        public static Player FindPlayer(ulong characterId)
        {
            try
            {
                playerMutex.EnterReadLock();
                return players.SingleOrDefault(p => p.Character.Id == characterId);
            }
            finally
            {
                playerMutex.ExitReadLock();
            }
        }

        public static void Update()
        {
            // TODO: change this to a multithreaded implementation
            foreach (KeyValuePair<uint, Territory> pair in territories)
                pair.Value.Update(0d);
        }
    }
}
