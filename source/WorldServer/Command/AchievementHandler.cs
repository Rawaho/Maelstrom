using System.Linq;
using Shared.Command;
using Shared.Game;
using WorldServer.Game.Achievement;
using WorldServer.Network;

namespace WorldServer.Command
{
    public class AchievementHandler
    {
        // achievement_criteria criteriaTypeId parameters...
        [CommandHandler("achievement_criteria", SecurityLevel.Developer)]
        public static void HandleAchievementCriteria(WorldSession session, params string[] parameters)
        {
            if (parameters.Length < 1)
                return;
            
            if (!byte.TryParse(parameters[0], out byte criteriaTypeId))
                return;
            
            session.Player.Achievement.UpdateCriteria((CriteriaType)criteriaTypeId, parameters.Skip(1).Select(int.Parse).ToArray());
        }
        
        // achievement_criteria_counter criteriaCounterTypeId value
        [CommandHandler("achievement_criteria_counter", SecurityLevel.Developer, 2)]
        public static void HandleAchievementCriteriaCounter(WorldSession session, params string[] parameters)
        {
            if (!ushort.TryParse(parameters[0], out ushort criteriaCounterId))
                return;

            if (!int.TryParse(parameters[1], out int count))
                return;

            session.Player.Achievement.UpdateCriteria(CriteriaType.Counter, criteriaCounterId, count);
        }
    }
}
