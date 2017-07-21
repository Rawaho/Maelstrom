using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shared.Database;
using Shared.Database.Datacentre;
using Shared.Network;
using WorldServer.Game;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public class AuthenticationHandler
    {
        [SubPacketHandler(SubPacketType.ClientHelloWorld)]
        public static async void HandleClientHelloWorld(Session session, ClientHelloWorld helloWorld)
        {
            (uint ServiceAccountId, ulong CharacterId) characterSession = await DatabaseManager.DataCentre.GetCharacterSession(helloWorld.ActorId, session.Remote.ToString());
            if (characterSession.ServiceAccountId == 0u)
                return;

            switch (session.Channel)
            {
                case ConnectionChannel.World:
                {
                    var worldSession = (WorldSession)session;
                    if (worldSession.Player != null)
                        return;

                    List<CharacterInfo> characters = await DatabaseManager.DataCentre.GetCharacters(characterSession.ServiceAccountId);
                    Debug.Assert(characters.Count > 0);
                    CharacterInfo characterInfo = characters.SingleOrDefault(c => c.Id == characterSession.CharacterId);
                    Debug.Assert(characterInfo != null);

                    session.Send(new ServerHelloWorld
                    {
                        ActorId  = helloWorld.ActorId
                    });

                    worldSession.Player = new Player(worldSession, characterInfo);
                    break;
                }
                case ConnectionChannel.Chat:
                {
                    session.Send(new ServerHelloWorld
                    {
                        ActorId  = helloWorld.ActorId
                    });
                    break;
                }
            }
        }
    }
}
