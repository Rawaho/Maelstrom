using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Shared.SqPack;
using WorldServer.Game.Entity;
using WorldServer.Network;
using WorldServer.Network.Message;

namespace WorldServer.Game.Achievement
{
    public class AchievementManager
    {
        public const int AchievmentMaskSize    = 256 * 8;
        public const int LatestAchievementSize = 5;
        
        private readonly Player owner;
        
        private readonly List<Achievement> completedAchievements = new List<Achievement>();
        private readonly FixedQueue<ushort> latestAchievements = new FixedQueue<ushort>(LatestAchievementSize);

        // any criteria type not Counter isn't stored directly in the achievement manager
        private readonly Dictionary<CriteriaCounterType, Criteria> counterCriteria = new Dictionary<CriteriaCounterType, Criteria>();

        public AchievementManager(Player player)
        {
            owner = player;
            // TODO: load from DB
        }

        /// <summary>
        /// Update all achievement criteria related to type.
        /// </summary>
        public void UpdateCriteria(CriteriaType type, params int[] parameters)
        {
            if (!GameTableManager.AchievementXCriteriaType.TryGetValue((byte)type, out ReadOnlyCollection<SaintCoinach.Xiv.Achievement> achievementEntries))
                throw new ArgumentException($"Invalid CriteriaType: {type}!");

            if (type == CriteriaType.Counter)
            {
                if (parameters.Length != 2)
                    throw new ArgumentException($"Invalid parameter count {parameters.Length} for CriteriaType Counter!");

                UpdateCriteriaCounter((CriteriaCounterType)parameters[0], (uint)parameters[1]);
            }
            else
            {
                foreach (SaintCoinach.Xiv.Achievement entry in achievementEntries)
                {
                    if (HasAchievement((uint)entry.Key))
                        continue;

                    if (!CanUpdateCriteria(entry, parameters))
                        continue;

                    if (GetAchievementCriteriaCounter(entry) >= GetAchievementCriteriaCounterMax(entry))
                        CompleteAchievement(entry);
                }
            }
        }

        /// <summary>
        /// Update achievement criteria counter related to type.
        /// </summary>
        public void UpdateCriteriaCounter(CriteriaCounterType type, uint value)
        {
            if (!GameTableManager.AchievementXCriteriaCounterType.TryGetValue((byte)type, out ReadOnlyCollection<SaintCoinach.Xiv.Achievement> achievementEntries))
                throw new ArgumentException($"Invalid CriteriaCounterType: {type}!");

            if (!counterCriteria.ContainsKey(type))
                counterCriteria[type] = new Criteria(value);
            else
                counterCriteria[type].IncrementValue(value);

            foreach (SaintCoinach.Xiv.Achievement entry in achievementEntries)
            {
                if (HasAchievement((uint)entry.Key))
                    continue;

                if (GetAchievementCriteriaCounter(entry) >= GetAchievementCriteriaCounterMax(entry))
                    CompleteAchievement(entry);
            }
        }

        private bool HasAchievement(uint achievementId)
        {
            return completedAchievements.SingleOrDefault(a => a.AchievementId == achievementId) != null;
        }

        /// <summary>
        /// Update and reward all applicable achievement criteria.
        /// </summary>
        public void CheckAchievements()
        {
            foreach (KeyValuePair<byte, ReadOnlyCollection<SaintCoinach.Xiv.Achievement>> pair in GameTableManager.AchievementXCriteriaType)
            {
                CriteriaType type = (CriteriaType)pair.Key;
                if (type == CriteriaType.Counter)
                    continue;

                foreach (SaintCoinach.Xiv.Achievement entry in pair.Value)
                    if (GetAchievementCriteriaCounter(entry) >= GetAchievementCriteriaCounterMax(entry))
                        CompleteAchievement(entry);
            }
        }

        /// <summary>
        /// Get current amount of criteria progress for Achievement.
        /// </summary>
        private uint GetAchievementCriteriaCounter(SaintCoinach.Xiv.Achievement entry)
        {
            switch ((CriteriaType)entry.Type)
            {
                case CriteriaType.Counter:
                    CriteriaCounterType type = (CriteriaCounterType)entry.GetCriteriaParameter(CriteriaParameter.CriteriaCounterTypeId);
                    return counterCriteria.ContainsKey(type) ? counterCriteria[type].Value : 0u;
                case CriteriaType.Level:
                    return owner.Character.GetClassInfo((uint)entry.GetCriteriaParameter(CriteriaParameter.JobClassId)).Level;
                /* TODO: handle these types when related systems are implemented
                case CriteriaType.Achievement:
                case CriteriaType.MateriaMelding:
                case CriteriaType.HuntingLog:
                case CriteriaType.Exploration:
                case CriteriaType.QuestCompleteAny:
                case CriteriaType.ChocoboRank:
                case CriteriaType.PvPRank:
                case CriteriaType.PvPMatch:
                case CriteriaType.PvPMatchWin:
                case CriteriaType.ReputationRank:
                case CriteriaType.FrontlineCampaign:
                case CriteriaType.FrontlineCampaignWin:
                case CriteriaType.FrontlineCampaignWinAny:
                case CriteriaType.AetherCurrent:
                case CriteriaType.Minion:
                case CriteriaType.VerminionChallenge:
                case CriteriaType.AnimaWeapon:*/
                default:
                {
                    #if DEBUG
                        Console.WriteLine($"Unhandled CriteriaType: {entry.Type}, unable to return valid criteria counter!");
                    #endif
                    return 0;
                }
            }
        }
        
        /// <summary>
        /// Get amount of criteria required to complete Achievement.
        /// </summary>
        private static int GetAchievementCriteriaCounterMax(SaintCoinach.Xiv.Achievement entry)
        {
            switch ((CriteriaType)entry.Type)
            {   
                case CriteriaType.Achievement:
                case CriteriaType.QuestCompleteAny:
                    return entry.Data.Count(p => p != 0);
                case CriteriaType.MateriaMelding:
                case CriteriaType.ChocoboRank:
                case CriteriaType.PvPMatch:
                case CriteriaType.PvPMatchWin:
                case CriteriaType.FrontlineCampaign:
                case CriteriaType.FrontlineCampaignWinAny:
                case CriteriaType.VerminionChallenge:
                    return entry.Data[0];
                case CriteriaType.Counter:
                case CriteriaType.Level:
                case CriteriaType.PvPRank:
                case CriteriaType.ReputationRank:
                case CriteriaType.FrontlineCampaignWin:
                case CriteriaType.Minion:
                    return entry.Data[1];
                case CriteriaType.HuntingLog:
                case CriteriaType.Exploration:
                case CriteriaType.AetherCurrent:
                case CriteriaType.AnimaWeapon:
                    return 1;
                default:
                {
                    #if DEBUG
                        Console.WriteLine($"Unhandled CriteriaType: {entry.Type}, unable to return valid max criteria counter!");
                    #endif
                    return int.MaxValue;
                }       
            }
        }

        

        private bool CanUpdateCriteria(SaintCoinach.Xiv.Achievement entry, params int[] parameters)
        {
            Debug.Assert(entry != null);
            switch ((CriteriaType)entry.Type)
            {
                case CriteriaType.Achievement:
                    if (parameters.Length < 1)
                        return false;
                    return entry.Data.Contains(parameters[0]);
                case CriteriaType.Level:
                {
                    if (parameters.Length < 1)
                        return false;
                    if (entry.GetCriteriaParameter(CriteriaParameter.JobClassId) != parameters[0])
                        return false;
                    return entry.GetCriteriaParameter(CriteriaParameter.ClassLevel) < owner.Character.GetClassInfo((uint)parameters[0]).Level;
                }
                /* TODO: handle these types when related systems are implemented
                case CriteriaType.MateriaMelding:
                case CriteriaType.HuntingLog:
                case CriteriaType.Exploration:
                case CriteriaType.QuestCompleteAny:
                case CriteriaType.ChocoboRank:
                case CriteriaType.PvPRank:
                case CriteriaType.PvPMatch:
                case CriteriaType.PvPMatchWin:
                case CriteriaType.ReputationRank:
                case CriteriaType.FrontlineCampaign:
                case CriteriaType.FrontlineCampaignWin:
                case CriteriaType.FrontlineCampaignWinAny:
                case CriteriaType.AetherCurrent:
                case CriteriaType.Minion:
                case CriteriaType.VerminionChallenge:
                case CriteriaType.AnimaWeapon:*/
                default:
                {
                    #if DEBUG
                        Console.WriteLine($"Unhandled AchievementCriteriaType: {entry.Type} in CanUpdateCriteria!");
                    #endif
                    return false;
                }
            }
        }

        private void CompleteAchievement(SaintCoinach.Xiv.Achievement entry)
        {
            Debug.Assert(entry != null);

            completedAchievements.Add(new Achievement((uint)entry.Key));
            latestAchievements.Enqueue((ushort)entry.Key);

            // TODO: handle rewards properly
            if (entry.Item != null)
                owner.Inventory.NewItem((uint)entry.Item.Key);

            if (entry.Title != null)
            {
            }

            if (owner.InWorld)
            {
                owner.Session.Send(new ServerActorAction2
                {
                    Action     = ActorAction.AchievementComplete,
                    Parameter1 = (uint)entry.Key
                });
            
                owner.SendMessageToVisible(new ServerActorAction2
                {
                    Action     = ActorAction.AchievementCompleteChat,
                    Parameter1 = (uint)entry.Key
                });
            }
        }
        
        public void SendAchievementList()
        {
            var achievementMask = new BitArray(AchievmentMaskSize);
            completedAchievements.ForEach(a => achievementMask.Set((int)a.AchievementId, true));

            owner.Session.Send(new ServerAchievementList
            {
                AchievementMask    = achievementMask,
                LatestAchievements = latestAchievements
            });
        }

        public void SendAchievementCriteria(uint achievementId)
        {
            if (!GameTableManager.Achievements.TryGetValue(achievementId, out SaintCoinach.Xiv.Achievement entry))
                throw new ArgumentException($"Invalid achievement id: {achievementId}");

            uint criteriaCounterMax = (uint)GetAchievementCriteriaCounterMax(entry);
            owner.Session.Send(new ServerActorAction2
            {
                Action     = ActorAction.AchievementCriteriaResponse,
                Parameter1 = achievementId,
                Parameter2 = Math.Min(GetAchievementCriteriaCounter(entry), criteriaCounterMax),
                Parameter3 = criteriaCounterMax
            });
        }
    }
}
