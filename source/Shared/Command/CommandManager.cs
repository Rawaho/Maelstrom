using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Shared.Game;
using Shared.Network;

namespace Shared.Command
{
    public static class CommandManager
    {
        public delegate void CommandHandler(Session session, params string[] parameters);

        private static Dictionary<string, (CommandHandlerAttribute Attribute, CommandHandler Handler)> commandHandlers; 

        public static void Initialise()
        {
            InitialiseCommandHandlers();

            new Thread(() =>
            {
                while (true)
                {
                    Console.Write("Maelstrom >> ");

                    string commandLine = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(commandLine))
                        continue;

                    ParseCommand(commandLine, out string command, out string[] parameters);
                    if (GetCommand(null, command, parameters, out CommandHandler handler))
                        handler.Invoke(null, parameters);
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        private static void InitialiseCommandHandlers()
        {
            commandHandlers = new Dictionary<string, (CommandHandlerAttribute Attribute, CommandHandler Handler)>(StringComparer.OrdinalIgnoreCase);
            foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    foreach (CommandHandlerAttribute attribute in method.GetCustomAttributes<CommandHandlerAttribute>())
                    {
                        ParameterInfo[] handlerParameters = method.GetParameters();
                        Debug.Assert(handlerParameters.Length == 2);
                        Debug.Assert(handlerParameters[0].ParameterType == typeof(Session) || handlerParameters[0].ParameterType.IsSubclassOf(typeof(Session)));

                        ParameterExpression sessionParam = Expression.Parameter(typeof(Session));
                        ParameterExpression commandParam = Expression.Parameter(handlerParameters[1].ParameterType);

                        MethodCallExpression expressionBody = Expression.Call(method,
                            Expression.Convert(sessionParam, handlerParameters[0].ParameterType), commandParam);

                        Expression<CommandHandler> expression = Expression.Lambda<CommandHandler>(expressionBody, sessionParam, commandParam);
                        commandHandlers.Add(attribute.Command, (attribute, expression.Compile()));
                    }
                }
            }
        }

        public static void ParseCommand(string commandLine, out string command, out string[] parameters)
        {
            string[] commandExplode = commandLine.Split(' ');
            command    = commandExplode[0];
            parameters = commandExplode.Skip(1).ToArray();
        }

        public static bool GetCommand(Session session, string command, string[] parameters, out CommandHandler handler)
        {
            handler = null;
            if (!commandHandlers.TryGetValue(command, out (CommandHandlerAttribute Attribute, CommandHandler Handler) commandInfo))
                return false;

            if (commandInfo.Attribute.ParameterCount != -1 && parameters.Length < commandInfo.Attribute.ParameterCount)
                return false;

            // console commands can't be invoked from game session
            if (commandInfo.Attribute.Security == SecurityLevel.Console && session != null)
                return false;

            // session commands can't be invoked from console
            if (commandInfo.Attribute.Security < SecurityLevel.Console && session == null)
                return false;

            handler = commandInfo.Handler;
            return true;
        }
    }
}
