using System.Collections.Generic;
using Shared.Game;
using WorldServer.Game.Entity;

namespace WorldServer.Game.Social
{
    public static class SocialManager
    {
        private static readonly Dictionary<SocialType, QueuedCounter<uint>> socialCounters = new Dictionary<SocialType, QueuedCounter<uint>>();
        private static readonly Dictionary<ulong, SocialBase> socialEntities = new Dictionary<ulong, SocialBase>();

        public static void Initialise()
        {
            InitialiseParties();
        }

        private static void InitialiseParties()
        {
            // TODO: load parties and counter from DB
            socialCounters[SocialType.Party] = new QueuedCounter<uint>(0u);
        }

        public static void NewSocialEntity(SocialBase socialBase)
        {
            socialEntities.Add(socialBase.Key, socialBase);
        }

        public static void RemoveSocialEntity(SocialBase socialBase)
        {
            socialEntities.Remove(socialBase.Key);
        }

        /// <summary>
        /// Find a base social entity casted to supplied generic type.
        /// </summary>
        public static T FindSocialEntity<T>(SocialType type, uint id)
            where T : SocialBase
        {
            if (!socialEntities.TryGetValue(((uint)type << 56) | id, out SocialBase socialEntity))
                return null;
            return (T)socialEntity;
        }

        /// <summary>
        /// Create a new party with the specified player as the leader.
        /// </summary>
        public static Party NewParty(Player leader)
        {
            var party = new Party(socialCounters[SocialType.Party].DequeueValue(), leader);
            NewSocialEntity(party);
            return party;
        }

        public static void Update(double lastTick)
        {
            foreach (KeyValuePair<ulong, SocialBase> pair in socialEntities)
                pair.Value.Update(lastTick);
        }
    }
}
