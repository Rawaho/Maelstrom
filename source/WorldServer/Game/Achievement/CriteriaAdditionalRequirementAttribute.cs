using System;

namespace WorldServer.Game.Achievement
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class CriteriaAdditionalRequirementAttribute : Attribute
    {
        public CriteriaAdditionalRequirement Requirement { get; }

        public CriteriaAdditionalRequirementAttribute(CriteriaAdditionalRequirement requirement)
        {
            Requirement = requirement;
        }
    }
}
