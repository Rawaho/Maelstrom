using System.Collections.Generic;

namespace LobbyServer.Network
{
    public class Token
    {
        public string SessionId { get; }

        private readonly Dictionary<string, string> tokenParameters = new Dictionary<string, string>();

        public Token(string token)
        {
            foreach (string parameter in token.Split(' '))
            {
                if (parameter.Contains("="))
                {
                    string[] parameterExplode = parameter.Split('=');
                    if (parameterExplode.Length == 2)
                        tokenParameters.Add(parameterExplode[0], parameterExplode[1]);
                }
                else if (SessionId == null)
                    SessionId = parameter;
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            return tokenParameters.TryGetValue(key, out value);
        }
    }
}
