using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Shared.SqPack;
using Shared.SqPack.GameTable;
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
            if (!GameTableManager.AchievementXCriteriaType.TryGetValue((byte)type, out ReadOnlyCollection<AchievementEntry> achievementEntries))
                throw new ArgumentException($"Invalid CriteriaType: {type}!");

            if (type == CriteriaType.Counter)
            {
                if (parameters.Length != 2)
                    throw new ArgumentException($"Invalid parameter count {parameters.Length} for CriteriaType Counter!");

                UpdateCriteriaCounter((CriteriaCounterType)parameters[0], (uint)parameters[1]);
            }
            else
            {
                foreach (AchievementEntry entry in achievementEntries)
                {
                    if (HasAchievement(entry.Index))
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
            if (!GameTableManager.AchievementXCriteriaCounterType.TryGetValue((byte)type, out ReadOnlyCollection<AchievementEntry> achievementEntries))
                throw new ArgumentException($"Invalid CriteriaCounterType: {type}!");

            if (!counterCriteria.ContainsKey(type))
                counterCriteria[type] = new Criteria(value);
            else
                counterCriteria[type].IncrementValue(value);

            foreach (AchievementEntry entry in achievementEntries)
            {
                if (HasAchievement(entry.Index))
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
            foreach (KeyValuePair<byte, ReadOnlyCollection<AchievementEntry>> pair in GameTableManager.AchievementXCriteriaType)
            {
                CriteriaType type = (CriteriaType)pair.Key;
                if (type == CriteriaType.Counter)
                    continue;

                foreach (AchievementEntry entry in pair.Value)
                    if (GetAchievementCriteriaCounter(entry) >= GetAchievementCriteriaCounterMax(entry))
                        CompleteAchievement(entry);
            }
        }

        /// <summary>
        /// Get current amount of criteria progress for Achievement.
        /// </summary>
        private uint GetAchievementCriteriaCounter(AchievementEntry entry)
        {
            switch ((CriteriaType)entry.CriteriaType)
            {
                case CriteriaType.Counter:
                    CriteriaCounterType type = (CriteriaCounterType)entry.CriteriaData.CriteriaCounterTypeId;
                    return counterCriteria.ContainsKey(type) ? counterCriteria[type].Value : 0u;
                case CriteriaType.Level:
                    return owner.Character.GetClassInfo(entry.CriteriaData.JobClassId).Level;
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
                        Console.WriteLine($"Unhandled CriteriaType: {entry.CriteriaType}, unable to return valid criteria counter!");
                    #endif
                    return 0;
                }
            }
        }
        
        /// <summary>
        /// Get amount of criteria required to complete Achievement.
        /// </summary>
        private static int GetAchievementCriteriaCounterMax(AchievementEntry entry)
        {
            switch ((CriteriaType)entry.CriteriaType)
            {   
                case CriteriaType.Achievement:
                case CriteriaType.QuestCompleteAny:
                    return entry.CriteriaParameters.Count(p => p != 0);
                case CriteriaType.MateriaMelding:
                case CriteriaType.ChocoboRank:
                case CriteriaType.PvPMatch:
                case CriteriaType.PvPMatchWin:
                case CriteriaType.FrontlineCampaign:
                case CriteriaType.FrontlineCampaignWinAny:
                case CriteriaType.VerminionChallenge:
                    return entry.CriteriaParameters[0];
                case CriteriaType.Counter:
                case CriteriaType.Level:
                case CriteriaType.PvPRank:
                case CriteriaType.ReputationRank:
                case CriteriaType.FrontlineCampaignWin:
                case CriteriaType.Minion:
                    return entry.CriteriaParameters[1];
                case CriteriaType.HuntingLog:
                case CriteriaType.Exploration:
                case CriteriaType.AetherCurrent:
                case CriteriaType.AnimaWeapon:
                    return 1;
                default:
                {
                    #if DEBUG
                        Console.WriteLine($"Unhandled CriteriaType: {entry.CriteriaType}, unable to return valid max criteria counter!");
                    #endif
                    return int.MaxValue;
                }       
            }
        }
        
        private bool CanUpdateCriteria(AchievementEntry entry, params int[] parameters)
        {
            Debug.Assert(entry != null);
            switch ((CriteriaType)entry.CriteriaType)
            {
                case CriteriaType.Achievement:
                    if (parameters.Length < 1)
                        return false;
                    return entry.CriteriaParameters.Contains(parameters[0]);
                case CriteriaType.Level:
                {
                    if (parameters.Length < 1)
                        return false;
                    if (entry.CriteriaData.JobClassId != parameters[0])
                        return false;
                    return entry.CriteriaData.ClassLevel < owner.Character.GetClassInfo((uint)parameters[0]).Level;
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
                        Console.WriteLine($"Unhandled AchievementCriteriaType: {entry.CriteriaType} in CanUpdateCriteria!");
                    #endif
                    return false;
                }
            }
        }

        private void CompleteAchievement(AchievementEntry entry)
        {
            Debug.Assert(entry != null);

            completedAchievements.Add(new Achievement(entry.Index));
            latestAchievements.Enqueue((ushort)entry.Index);

            // TODO: handle rewards properly
            if (entry.RewardItemId != 0u)
                owner.Inventory.NewItem(entry.RewardItemId);

            if (entry.RewardTitleId != 0u)
            {
            }

            if (owner.InWorld)
            {
                owner.Session.Send(new ServerActorAction2
                {
                    Action     = ActorAction.AchievementComplete,
                    Parameter1 = entry.Index
                });
            
                owner.SendMessageToVisible(new ServerActorAction2
                {
                    Action     = ActorAction.AchievementCompleteChat,
                    Parameter1 = entry.Index
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
            if (!GameTableManager.Achievements.TryGetValue(achievementId, ExdLanguage.En, out AchievementEntry entry))
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
