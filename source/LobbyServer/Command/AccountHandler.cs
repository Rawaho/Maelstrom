using System;
using System.Linq;
using Shared.Command;
using Shared.Cryptography;
using Shared.Database;
using Shared.Game;
using Shared.Network;

namespace LobbyServer.Command
{
    public static class AccountHandler
    {
        // account_create username password
        [CommandHandler("account_create", SecurityLevel.Console, 2)]
        public static void HandleAccountCreate(Session session, params string[] parameters)
        {
            string username = parameters[0];
            string salt     = HashProvider.GenerateSalt();
            string digest   = HashProvider.Sha256(parameters[1] + salt);

            if (DatabaseManager.Authentication.CreateAccount(username, digest, salt))
                Console.WriteLine($"Successfully created account {username}!");
        }

        // account_service_create accountId name
        [CommandHandler("account_service_create", SecurityLevel.Console, 2)]
        public static void HandleAccountServiceCreate(Session session, params string[] parameters)
        {
            if (!uint.TryParse(parameters[0], out uint accountId))
                return;

            string serviceName = string.Join(" ", parameters.Skip(1).ToArray());
            if (DatabaseManager.Authentication.CreateServiceAccount(accountId, serviceName))
                Console.WriteLine($"Successfully created service account {serviceName}!");
        }
    }
}
