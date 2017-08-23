using System;

namespace WorldServer.Game.Social
{
    public class PartyMemberException : Exception
    {
        public PartyMemberException(string message)
            : base(message)
        {
        }
    }
}
