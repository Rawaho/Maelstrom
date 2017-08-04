using System;

namespace WorldServer.Game.Achievement
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CriteriaAttribute : Attribute
    {
        public CriteriaAction Action { get; }

        public CriteriaAttribute(CriteriaAction action)
        {
            Action = action;
        }
    }
}
