using System.Collections.Generic;
using System.Linq;
using Shared.Network;
using WorldServer.Game.Entity;
using WorldServer.Game.Map;
using WorldServer.Network.Message;

namespace WorldServer.Game.Social
{
    public class Party : SocialBase
    {
        public const byte FullPartySize = 8;

        public class Member
        {
            public ulong Id { get; }
            public string Name { get; }
            public byte Index { get; }

            public Member(Player player, byte index)
            {
                Id    = player.Character.Id;
                Name  = player.Character.Name;
                Index = index;
            }
        }

        public Member Leader => leaderIndex != byte.MaxValue ? Members[leaderIndex] : null;
        public List<Member> Members { get; } = new List<Member>();

        /// <summary>
        /// Return player entities for all online members.
        /// </summary>
        private List<Player> Players => Members.Select(m => MapManager.FindPlayer(m.Id)).Where(p => p != null).ToList();

        private byte leaderIndex = byte.MaxValue;
        
        public Party(uint id, Player leader)
            : base(id, SocialType.Party)
        {
            MemberAdd(leader, false);
        }

        /// <summary>
        /// Check if host can invite invitee to party.
        /// </summary>
        protected override bool CanInvite(Player host, Player invitee)
        {
            if (host.Character.Id != Leader.Id)
                return false;

            if (invitee.Party != null)
            {
                host.Session.Send(new ServerSocialMessage
                {
                    Name       = invitee.Character.Name,
                    LogMessage = LogMessage.PartyInviteInviteeParty,
                    SocialType = Type 
                });
                return false;
            }

            if (Members.Count > FullPartySize)
            {
                host.Session.Send(new ServerSocialMessage
                {
                    Name       = invitee.Character.Name,
                    LogMessage = LogMessage.PartyInvitePartyFull,
                    SocialType = Type 
                });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add a new player to the party.
        /// </summary>
        public sealed override void MemberAdd(Player invitee, bool broadcast)
        {
            if (Members.Any(m => m.Id == invitee.Character.Id))
                throw new PartyMemberException($"Can't invite {invitee.Character.Name}, {invitee.Character.Name} is already a member of the party!");

            if (Members.Count > FullPartySize)
                throw new PartyMemberException($"Can't invite {invitee.Character.Name}, party is full!");

            // no leader, assign new member as leader
            if (Leader == null)
                leaderIndex = (byte)Members.Count;

            Members.Add(new Member(invitee, (byte)Members.Count));

            if (broadcast)
            {
                invitee.Session.Send(new ServerSocialMessage
                {
                    Name       = invitee.Character.Name,
                    Byte1      = 1,
                    SocialType = Type
                });

                List<Player> onlineMembers = Players;
                Send(new ServerSocialAction
                {
                    ObjectParameter1 = Leader.Id,
                    ObjectParameter2 = invitee.Character.Id,
                    PlayerParameter1 = Leader.Name,
                    PlayerParameter2 = invitee.Character.Name,
                    Byte1            = 1,
                    Action           = 1,
                    Byte4            = 2
                }, onlineMembers);

                SendUpdate(onlineMembers);
            }
            
            invitee.SetParty(this);
        }

        private void MemberRemove(Member member)
        {
            Player leaver = MapManager.FindPlayer(member.Id);
            leaver?.SetParty(null);
            leaver?.Session.Send(new ServerParty());

            Members.Remove(member);
            if (Members.Count <= 1)
                Disband();
        }

        private void Disband()
        {
            InviteCancelAll();

            List<Player> onlineMembers = Players;
            onlineMembers.ForEach(m => m.SetParty(null));

            Send(new ServerSocialAction
            {
                ObjectParameter1 = Leader.Id,
                PlayerParameter1 = string.Empty,
                PlayerParameter2 = string.Empty,
                Action           = 3
            }, onlineMembers);

            Members.Clear();
            SendUpdate(onlineMembers);

            SocialManager.RemoveSocialEntity(this);
        }

        /// <summary>
        /// Remove a player from the party.
        /// </summary>
        public void MemberLeave(Player leaver)
        {
            Member member = Members.SingleOrDefault(m => m.Name == leaver.Character.Name);
            if (member == null)
                throw new PartyMemberException($"{leaver.Character.Name} can't leave a party they aren't a member of!");

            Send(new ServerSocialAction
            {
                ObjectParameter1 = Leader.Id,
                ObjectParameter2 = leaver.Character.Id,
                PlayerParameter1 = Leader.Name,
                PlayerParameter2 = leaver.Character.Name,
                Action           = 5
            });

            MemberRemove(member);
        }

        /// <summary>
        /// Disband the party.
        /// </summary>
        /// <param name="player"></param>
        public void MemberDisband(Player player)
        {
            if (player.Character.Id != Leader.Id)
                throw new PartyLeaderException($"{player.Character.Name} can't disband, they aren't the current party leader!");

            Disband();
        }

        /// <summary>
        /// Kick a player from the party.
        /// </summary>
        public void MemberKick(Player kicker, string victimName)
        {
            if (kicker.Character.Id != Leader.Id)
                throw new PartyLeaderException($"{kicker.Character.Name} can't kick {victimName}, they aren't the current party leader!");

            Member victim = Members.SingleOrDefault(m => m.Name == victimName);
            if (victim == null)
                throw new PartyMemberException($"{kicker.Character.Name} can't kick {victimName}, {victimName} isn't a party member!");

            Send(new ServerSocialAction
            {
                ObjectParameter1 = kicker.Character.Id,
                ObjectParameter2 = victim.Id,
                PlayerParameter1 = kicker.Character.Name,
                PlayerParameter2 = victim.Name,
                Action           = 4
            });

            MemberRemove(victim);
        }

        /// <summary>
        /// Promote a player to party leader.
        /// </summary>
        public void MemberPromote(Player promoter, string promoteeName)
        {
            if (promoter.Character.Id != Leader.Id)
                throw new PartyLeaderException($"{promoter.Character.Name} can't promote {promoteeName}, they aren't the current party leader!");

            Member member = Members.SingleOrDefault(m => m.Name == promoteeName);
            if (member == null)
                throw new PartyMemberException($"{promoter.Character.Name} can't promote {promoteeName}, {promoteeName} isn't a party member!");

            Player promotee = MapManager.FindPlayer(promoteeName);
            if (promotee == null)
            {
                promoter.Session.Send(new ServerSocialMessage
                {
                    Name       = promoteeName,
                    LogMessage = LogMessage.PartyPromoteOffline,
                    SocialType = Type
                });
                return;
            }

            leaderIndex = member.Index;

            List<Player> onlineMembers = Players;
            Send(new ServerSocialAction
            {
                ObjectParameter1 = promoter.Character.Id,
                ObjectParameter2 = promotee.Character.Id,
                PlayerParameter1 = promoter.Character.Name,
                PlayerParameter2 = promotee.Character.Name,
                Action           = 2,
                Byte4            = 2
            }, onlineMembers);

            SendUpdate(onlineMembers);
        }

        /// <summary>
        /// Send party social list, this shows the party in the social window.
        /// </summary>
        public override void SendSocialList(Player recipient, ushort sequence)
        {
            var socialList = new ServerSocialList
            {
                ListType = SocialListType.Party,
                Sequence = sequence
            };

            foreach (Member member in Members)
            {
                Player player = MapManager.FindPlayer(member.Name);

                var socialMember = player != null ? new ServerSocialList.Member(player) : new ServerSocialList.Member(member);
                if (member == Leader)
                    socialMember.OnlineStatusMask.Set(36, true);
                if (player != null)
                    socialMember.OnlineStatusMask.Set(37, true);

                socialList.SocialMembers.Add(socialMember);
            }

            recipient.Session.Send(socialList);
        }

        /// <summary>
        /// Send party list, this shows the party interface in the main UI.
        /// </summary>
        private void SendUpdate(List<Player> players = null)
        {
            // no recipients provided, send to all online party members
            if (players == null)
                players = Players;

            var serverParty = new ServerParty
            {
                LeaderIndex = leaderIndex 
            };

            foreach (Member member in Members)
            {
                Player player = players.SingleOrDefault(p => p.Character.Id == member.Id);
                serverParty.PartyMembers.Add(player != null ? new ServerParty.Member(player) : new ServerParty.Member(member));
            }

            Send(serverParty, players);
        }

        private void Send(SubPacket subPacket)
        {
            Members.ForEach(m => MapManager.FindPlayer(m.Id)?.Session.Send(subPacket));
        }

        private void Send(SubPacket subPacket, List<Player> players)
        {
            players.ForEach(p => p.Session.Send(subPacket));
        }
    }
}
