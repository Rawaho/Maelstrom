using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using LobbyServer.Manager;
using LobbyServer.Network.Message;
using Shared.Database;
using Shared.Database.Datacentre;
using Shared.Game;
using Shared.Network;
using Shared.SqPack;
using Shared.SqPack.GameTable;

namespace LobbyServer.Network.Handler
{
    public class CharacterHandler
    {
        [SubPacketHandler(SubPacketOpcode.ClientCharacterList, SubPacketHandlerFlags.RequiresEncryption)]
        public static void HandleClientSessionRequest(LobbySession session, ClientCharacterList characterList)
        {
            session.Sequence = characterList.Sequence;

            if (characterList.ServiceAccount >= session.ServiceAccounts.Count)
                return;

            // must be sent before characrer list otherwise crash
            SendRealmList(session);

            session.ServiceAccount = session.ServiceAccounts[characterList.ServiceAccount];
            SendCharacterList(session);
            
            // SendRetainerList();
        }

        private static void SendRealmList(LobbySession session)
        {
            ReadOnlyCollection<RealmInfo> realmInfo = AssetManager.RealmInfoStore;

            ServerRealmList realmList = new ServerRealmList { Sequence = session.Sequence };
            for (ushort i = 0; i < realmInfo.Count; i++)
            {
                // client expects no more than 6 realms per chunk
                if (i % ServerRealmList.MaxRealmsPerPacket == 0)
                {
                    // flush previous chunk
                    if (i != 0)
                    {
                        session.Send(realmList);
                        realmList.Realms.Clear();
                    }

                    realmList.Offset = i;
                    realmList.Final  = (ushort)(realmInfo.Count - i < ServerRealmList.MaxRealmsPerPacket ? 1 : 0);
                }

                RealmInfo realm = realmInfo[i];
                realmList.Realms.Add(new ServerRealmList.RealmInfo
                {
                    Id       = realm.Id,
                    Position = i,
                    Name     = realm.Name,
                    Flags    = realm.Flags
                });

                // flush final chunk
                if (i == realmInfo.Count - 1)
                    session.Send(realmList);
            }
        }

        private static async void SendCharacterList(LobbySession session)
        {
            session.Characters = await DatabaseManager.DataCentre.GetCharacters(session.ServiceAccount.Id);
            ServerCharacterList characterList = new ServerCharacterList
            {
                VeteranRank               = 0,
                DaysTillNextVeteranRank   = 0u,
                DaysSubscribed            = 0u,
                SubscriptionDaysRemaining = 0u,
                RealmCharacterLimit       = session.ServiceAccount.RealmCharacterLimit,
                AccountCharacterLimit     = session.ServiceAccount.AccountCharacterLimit,
                Expansion                 = session.ServiceAccount.Expansion,
                Offset                    = 1
            };

            if (session.Characters.Count == 0)
            {
                session.Send(characterList);
                return;
            }

            for (int i = 0; i < session.Characters.Count; i++)
            {
                // client expects no more than 2 characters per chunk
                if (i % ServerCharacterList.MaxCharactersPerPacket == 0)
                {
                    // flush previous chunk
                    if (i != 0)
                    {
                        session.Send(characterList);
                        session.FlushPacketQueue();
                        characterList.Characters.Clear();
                    }

                    // weird...
                    characterList.Offset = (byte)(session.Characters.Count - i <= ServerCharacterList.MaxCharactersPerPacket ? i * 2 + 1 : i * 2);
                }

                RealmInfo realmInfo = AssetManager.GetRealmInfo(session.Characters[i].RealmId);
                if (realmInfo == null)
                    continue;

                characterList.Characters.Add(((byte)i, realmInfo.Name, session.Characters[i]));
                
                // flush final chunk
                if (i == session.Characters.Count - 1)
                {
                    session.Send(characterList);
                    session.FlushPacketQueue();
                }
            }
        }

        [SubPacketHandler(SubPacketOpcode.ClientCharacterCreate, SubPacketHandlerFlags.RequiresEncryption | SubPacketHandlerFlags.RequiresAccount)]
        public static async void HandleCharacterCreate(LobbySession session, ClientCharacterCreate characterCreate)
        {
            session.Sequence = characterCreate.Sequence;
            switch (characterCreate.Type)
            {
                // verify
                case 1:
                {
                    RealmInfo realmInfo = AssetManager.GetRealmInfo(characterCreate.RealmId);
                    if (realmInfo == null)
                        return;

                    if (!CharacterInfo.VerifyName(characterCreate.Name) || !await DatabaseManager.DataCentre.IsCharacterNameAvailable(characterCreate.Name))
                    {
                        session.SendError(3035, 13004);
                        return;
                    }

                    if (session.Characters?.Count >= session.ServiceAccount.AccountCharacterLimit)
                    {
                        session.SendError(3035, 13203);
                        return;
                    }

                    if (session.Characters?.Count(c => c.RealmId == realmInfo.Id) >= session.ServiceAccount.RealmCharacterLimit)
                    {
                        session.SendError(3035, 13204);
                        return;
                    }

                    session.CharacterCreate = (realmInfo.Id, characterCreate.Name);
                    session.Send(new ServerCharacterCreate
                    {
                        Sequence = session.Sequence,
                        Type     = 1,
                        Name     = characterCreate.Name,
                        Realm    = realmInfo.Name
                    });
                    break;
                }
                // create
                case 2:
                {
                    if (session.CharacterCreate.Name == string.Empty)
                        return;

                    CharacterInfo characterInfo;
                    try
                    {
                        characterInfo = new CharacterInfo(session.ServiceAccount.Id, session.CharacterCreate.RealmId, session.CharacterCreate.Name, characterCreate.Json);
                    }
                    catch
                    {
                        // should only occur if JSON data is tampered with
                        return;
                    }

                    if (!characterInfo.Verify())
                        return;

                    ClassJobEntry entry = GameTableManager.ClassJobs.GetValue(characterInfo.ClassJobId, ExdLanguage.En);
                    Debug.Assert(entry != null);

                    characterInfo.AddClassInfo((byte)entry.ClassId);

                    if (!AssetManager.GetCharacterSpawn(entry.CityState, out WorldPosition spawnPosition))
                        return;

                    characterInfo.Finalise(AssetManager.GetNewCharacterId(), spawnPosition);

                    try
                    {
                        await characterInfo.SaveToDatabase();
                    }
                    catch
                    {
                        // should only occur if name was claimed in the time between verification and creation
                        session.SendError(3035, 13208);
                        return;
                    }

                    RealmInfo realmInfo = AssetManager.GetRealmInfo(session.CharacterCreate.Item1);
                    Debug.Assert(realmInfo != null);

                    session.Send(new ServerCharacterCreate
                    {
                        Sequence    = session.Sequence,
                        Type        = 2,
                        Name        = characterCreate.Name,
                        Realm       = realmInfo.Name,
                        CharacterId = characterInfo.Id
                    });

                    session.Characters.Add(characterInfo);
                    session.CharacterCreate = (0, string.Empty);
                    break;
                }
                    
            }
        }

        [SubPacketHandler(SubPacketOpcode.ClientCharacterDelete, SubPacketHandlerFlags.RequiresEncryption | SubPacketHandlerFlags.RequiresAccount)]
        public static void HandleCharacterDelete(LobbySession session, SubPacket subPacket)
        {
            // TODO
        }

        [SubPacketHandler(SubPacketOpcode.ClientEnterWorld, SubPacketHandlerFlags.RequiresEncryption | SubPacketHandlerFlags.RequiresAccount)]
        public static async void HandleClientEnterWorld(LobbySession session, ClientEnterWorld enterWorld)
        {
            session.Sequence = enterWorld.Sequence;

            CharacterInfo character = session.Characters.SingleOrDefault(c => c.Id == enterWorld.CharacterId);
            if (character == null)
                return;

            RealmInfo realmInfo = AssetManager.GetRealmInfo(character.RealmId);
            if (realmInfo == null)
                return;

            await DatabaseManager.DataCentre.CreateCharacterSession(character.Id, session.Remote.ToString());
            
            session.Send(new ServerEnterWorld
            {
                Sequence    = session.Sequence,
                ActorId     = character.ActorId,
                CharacterId = enterWorld.CharacterId,
                Token       = "",
                Host        = realmInfo.Host,
                Port        = realmInfo.Port
            });
        }
    }
}
