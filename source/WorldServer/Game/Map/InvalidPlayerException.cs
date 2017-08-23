using System;

namespace WorldServer.Game.Map
{
    public class InvalidPlayerException : Exception
    {
        public InvalidPlayerException(string name)
            : base($"Characters {name} doesn't exist in the world!")
        {
        }

        public InvalidPlayerException(ulong characterId)
            : base($"Character {characterId} doesn't exist in the world!")
        {
        }
    }
}
