using System;
using System.Collections.Generic;
using LobbyServer.Manager;
using LobbyServer.Network.Message;
using Shared.Cryptography;
using Shared.Database;
using Shared.Database.Authentication;
using Shared.Network;

namespace LobbyServer.Network.Handler
{
    public static class AuthenticationHandler
    {
        [SubPacketHandler(SubPacketType.ClientHello)]
        public static void HandleClientHello(LobbySession session, ClientHello hello)
        {
            session.CalculateNetworkKey(hello.Time, hello.Seed);
            session.SessionHash = XxHash.CalculateHash(Guid.NewGuid().ToByteArray());
            session.Send(new ServerHello
            {
                SessionHash = session.SessionHash
            });
        }

        [SubPacketHandler(SubPacketClientOpcode.ClientLobbyRequest, SubPacketHandlerFlags.RequiresEncryption)]
        public static void HandleClientLobbyRequest(LobbySession session, ClientLobbyRequest sessionRequest)
        {
            session.Sequence = sessionRequest.Sequence;

            string[] versionExplode = sessionRequest.Version.Split('+');
            // module data, version...
            if (versionExplode.Length < 1)
                return;

            foreach (string moduleVersion in versionExplode[0].Split(','))
            {
                string[] moduleExplode = moduleVersion.Split('/');
                if (moduleExplode.Length != 3)
                    continue;

                #if DEBUG
                    Console.WriteLine($"Module - File: {moduleExplode[0]}, Version: {moduleExplode[1]}, Digest: {moduleExplode[2]}");
                #endif

                if (!AssetManager.IsValidVersion(moduleExplode[0], moduleExplode[1], moduleExplode[2]))
                {
                    session.SendError(1012, 13101);
                    return;
                }
            }

            session.AuthToken = new Token(sessionRequest.Token);

            #if DEBUG
                Console.WriteLine($"Token: {sessionRequest.Token}");
            #endif

            session.NewEvent(new DatabaseGenericEvent<uint>(DatabaseManager.Authentication.GetAccount(session.AuthToken.SessionId), accountId =>
            {
                if (accountId == 0u)
                {
                    session.SendError(1000, 13100);
                    return;
                }

                session.NewEvent(new DatabaseGenericEvent<List<ServiceAccountInfo>>(DatabaseManager.Authentication.GetServiceAccounts(accountId), serviceAccounts =>
                {
                    if (serviceAccounts.Count == 0)
                    {
                        // TODO: probably not the correct error to display when no service accounts are present
                        session.SendError(1000, 13209);
                        return;
                    }

                    session.ServiceAccounts = serviceAccounts;
                    session.Send(new ServerServiceAccountList
                    {
                        Sequence        = session.Sequence,
                        ServiceAccounts = session.ServiceAccounts
                    });
                }));
            }));
        }
    }
}
