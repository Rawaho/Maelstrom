using System;
using Shared.Game;

namespace Shared.Command
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandHandlerAttribute : Attribute
    {
        public string Command { get; }
        public SecurityLevel Security { get; }
        public int ParameterCount { get; }

        public CommandHandlerAttribute(string command, SecurityLevel security, int parameterCount = -1)
        {
            Command        = command;
            Security       = security;
            ParameterCount = parameterCount;
        }
    }
}
