using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shared.Database.Datacentre;
using Shared.Game;
using Shared.Game.Enum;
using WorldServer.Game.Achievement;
using WorldServer.Game.Entity.Enums;
using WorldServer.Game.Event;
using WorldServer.Game.Map;
using WorldServer.Game.Social;
using WorldServer.Network;
using WorldServer.Network.Message;

namespace WorldServer.Game.Entity
{
    public class Player : Actor
    {
        public WorldSession Session { get; }
        public CharacterInfo Character { get; }
        public Inventory Inventory { get; }
        public EventManager Event { get; }
        public AchievementManager Achievement { get; }

        public Party Party { get; private set; }
        private readonly List<SocialInviteRequest> socialInviteLookup = new List<SocialInviteRequest>();

        public PlayerFlagsCu FlagsCu { get; private set; } = PlayerFlagsCu.FirstLogin;
        public bool IsLogin { get; set; } = true;
        public bool IsLoading { get; set; } = true;
        
        private readonly QueuedCounter<byte> spawnIndex = new QueuedCounter<byte>(0);
        private readonly Dictionary<uint, byte> spawnIndexLookup = new Dictionary<uint, byte>(byte.MaxValue);

        private WorldPosition pendingTeleportPosition;

        public Player(WorldSession session, CharacterInfo character)
            : base(character.ActorId, ActorType.Player)
        {
            Session     = session;
            Character   = character;
            Inventory   = new Inventory(this, character.Appearance.Race, character.Appearance.Sex, (ClassJob)character.ClassJobId);
            Event       = new EventManager(this);
            Achievement = new AchievementManager(this);
            Position    = character.SpawnPosition;
        }

        public override bool AddVisibleActor(Actor actor)
        {
            if (!base.AddVisibleActor(actor))
                return false;

            // inital territory spawns are sent in bulk from packet handler after territory loading
            if (!IsLoading)
                SendActor(actor);

            return true;
        }

        public override void RemoveVisibleActor(Actor actor)
        {
            Session.Send(actor.Id, actor.Id, new ServerActorAction1
            {
                Action = ActorAction.ActorRemove,
                Parameter1 = 1
            });

            if (actor.IsPlayer)
            {
                if (spawnIndexLookup.TryGetValue(actor.Id, out byte index))
                {
                    spawnIndexLookup.Remove(actor.Id);
                    spawnIndex.EnqueueValue(index);
                }
            }
            
            base.RemoveVisibleActor(actor);
        }

        private byte GetSpawnIndex(uint actorId)
        {
            // local actor always has an index of 0
            if (actorId == Id)
                return 0;
            
            if (!spawnIndexLookup.TryGetValue(actorId, out byte index))
            {
                index = spawnIndex.DequeueValue();
                spawnIndexLookup.Add(actorId, index);
            }

            return index;
        }

        private void SendActor(Actor actor)
        {
            if (actor.IsPlayer)
            {
                Player player = actor.ToPlayer;
                
                (ulong MainHand, ulong OffHand) weaponDisplayId = player.Inventory.GetWeaponDisplayIds();
                Session.Send(actor.Id, Id,
                    new ServerPlayerSpawn
                    {
                        SpawnIndex            = GetSpawnIndex(actor.Id),
                        Position              = Position,
                        Character             = player.Character,
                        MainHandDisplayId     = weaponDisplayId.MainHand,
                        OffHandDisplayId      = weaponDisplayId.OffHand,
                        VisibleItemDisplayIds = player.Inventory.GetVisibleItemDisplayIds()
                    });
            }
            else
            {
                // TODO: NPC spawns have a seperate packet
            }
        }

        public void OnLogin()
        {
            Debug.Assert(IsLogin);

            Achievement.CheckAchievements();

            Session.Send(new ServerContentFinderList {
                // Unlock all contents for now
                Contents = new BitArray(0x48 * 8, true)
            });
            
            Session.Send(new ServerPlayerSetup
            {
                Character = Character
            });

            Session.Send(new ServerClassSetup
            {
                ClassJobId = Character.ClassJobId,
                Level      = Character.Classes[Character.ClassId].Level
            });

            // MOTD
            Session.Send(new ServerMessage
            {
                Message = "Welcome to this Maelstrom server!\n"
                    + "Find out about this server project at https://www.github.com/Rawaho/Maelstrom"
            });

            Session.Send(new ServerSocialBlacklist());

            // TODO: client hangs without these sent
            Session.Send(new ServerUnknown01FB());
            Session.Send(new ServerUnknown01FD());
            
            Session.Send(new ServerQuestJournalActiveList());
            Session.Send(new ServerQuestJournalCompleteList());
        }

        public void OnLogout()
        {
            DeclineSocialInvites();
        }

        public override void OnAddToMap()
        {
            // opening maps are unique phased
            if (Map.Entry.Type == 6)
                SetPhase(Character.ActorId);

            base.OnAddToMap();
            pendingTeleportPosition = null;

            Session.Send(new ServerNewWorld
            {
                ActorId = Character.ActorId
            });
            
            Session.Send(new ServerTerritorySetup
            {
                WeatherId     = 1,
                WorldPosition = Position
            });

            Inventory.Send();
        }

        public override void OnRemoveFromMap()
        {
            base.OnRemoveFromMap();

            if (pendingTeleportPosition != null)
            {
                // remove phasing when leaving an opening map
                if (Map.Entry.Type == 6)
                    SetPhase(0u);

                Position  = pendingTeleportPosition;
                IsLoading = true;
                AddToMap();
            }
        }

        public void SendVisible()
        {
            foreach (Actor actor in visibleActors)
                SendActor(actor);
        }

        /// <summary>
        /// Teleport player to a new world position.
        /// </summary>
        public void TeleportTo(WorldPosition newPosition)
        {
            if (pendingTeleportPosition != null)
                return;

            #if DEBUG
                Console.WriteLine($"Teleporting player '{Character.Name}' to Territory: {newPosition.TerritoryId}, "
                    + $"X: {newPosition.Offset.X}, Y: {newPosition.Offset.Y}, Z: {newPosition.Offset.Z}");
            #endif

            // TODO: validate position
            pendingTeleportPosition = newPosition;

            if (Position.TerritoryId != newPosition.TerritoryId)
            {
                // shows black loading screen with territory name
                Session.Send(new ServerTerritoryPending
                {
                    TerritoryId  = newPosition.TerritoryId,
                    LogMessageId = 0u
                });
            }

            Map.RemoveActor(this);
        }

        /// <summary>
        /// Invite player to your party, if no party exists one will be created.
        /// </summary>
        public void PartyInvite(string inviteeName)
        {
            Player invitee = MapManager.FindPlayer(inviteeName);
            if (invitee == null)
                throw new InvalidPlayerException(inviteeName);

            if (Party == null)
                Party = SocialManager.NewParty(this);

            Party.Invite(this, invitee);
        }

        public void SetParty(Party party)
        {
            if (party != null && Party != null)
                throw new PartyStateException($"Can't assign {Character.Name} to a new party, they haven't left their current one!");

            Party = party;
        }

        public SocialInviteRequest FindSocialInvite(ulong hostId, SocialType type)
        {
            return socialInviteLookup.SingleOrDefault(s => s.HostId == hostId && s.Type == type);
        }

        public void AddSocialInvite(SocialInviteRequest inviteRequest)
        {
            socialInviteLookup.Add(inviteRequest);
        }

        public void RemoveSocialInvite(ulong hostId, SocialType type)
        {
            SocialInviteRequest inviteRequest = FindSocialInvite(hostId, type);
            if (inviteRequest != null)
                socialInviteLookup.Remove(inviteRequest);
        }

        /// <summary>
        /// Decline all pending social invites of a type, if no type is specified all invites will be declined.
        /// </summary>
        public void DeclineSocialInvites(SocialType type = SocialType.None)
        {
            IEnumerable<SocialInviteRequest> invites = type != SocialType.None ? socialInviteLookup.Where(s => s.Type == type) : socialInviteLookup;
            foreach (SocialInviteRequest inviteRequest in invites)
            {
                SocialBase socialEntity = SocialManager.FindSocialEntity<SocialBase>(inviteRequest.Type, inviteRequest.EntityId);
                socialEntity?.InviteResponse(this, 0);
            }
        }
    }
}
