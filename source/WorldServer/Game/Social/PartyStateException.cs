using System;

namespace WorldServer.Game.Social
{
    public class PartyStateException : Exception
    {
        public PartyStateException(string message)
            : base(message)
        {
        }
    }
}
