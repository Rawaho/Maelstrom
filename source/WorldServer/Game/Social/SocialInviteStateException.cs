using System;

namespace WorldServer.Game.Social
{
    public class SocialInviteStateException : Exception
    {
        public SocialInviteStateException(string message)
            : base(message)
        {
        }
    }
}
