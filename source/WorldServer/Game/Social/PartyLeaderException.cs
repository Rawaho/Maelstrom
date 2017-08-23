using System;

namespace WorldServer.Game.Social
{
    public class PartyLeaderException : Exception
    {
        public PartyLeaderException(string message)
            : base(message)
        {
        }
    }
}
