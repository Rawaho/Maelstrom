using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WorldServer.Game.Entity;
using WorldServer.Game.Map;
using WorldServer.Network.Message;

namespace WorldServer.Game.Social
{
    public abstract class SocialBase
    {
        private const double TimeToRespond = 300d;

        public class PendingInvite
        {
            public ulong InviteeId { get; }
            public ulong HostId { get; }
            public string HostName { get; }
            public double ResponseTime { get; set; }

            public PendingInvite(ulong inviteeId, ulong hostId, string hostName)
            {
                InviteeId    = inviteeId;
                HostId       = hostId;
                HostName     = hostName;
                ResponseTime = TimeToRespond;
            }
        }

        public uint Id { get; }
        public SocialType Type { get; }

        /// <summary>
        /// Key used to store entity in the social manager.
        /// </summary>
        public ulong Key => ((uint)Type << 56) | Id;

        private readonly List<PendingInvite> pendingInvites = new List<PendingInvite>();

        protected SocialBase(uint id, SocialType type)
        {
            Id   = id;
            Type = type;
        }

        private PendingInvite FindInvite(Player invitee)
        {
            return pendingInvites.SingleOrDefault(i => i.InviteeId == invitee.Character.Id);
        }

        private void RemoveInvite(Player invitee, PendingInvite pendingInvite)
        {
            invitee.RemoveSocialInvite(pendingInvite.HostId, Type);
            pendingInvites.Remove(pendingInvite);
        }

        /// <summary>
        /// Create a pending social invite.
        /// </summary>
        public virtual void Invite(Player host, Player invitee)
        {
            Debug.Assert(host != null);
            Debug.Assert(invitee != null);

            if (FindInvite(invitee) != null)
            {
                host.Session.Send(new ServerSocialMessage
                {
                    Name       = invitee.Character.Name,
                    LogMessage = LogMessage.SocialPartyPending,
                    SocialType = Type
                });
                return;
            }

            if (!CanInvite(host, invitee))
                return;

            invitee.AddSocialInvite(new SocialInviteRequest(host.Character.Id, Type, Id));
            pendingInvites.Add(new PendingInvite(invitee.Character.Id, host.Character.Id, host.Character.Name));

            host.Session.Send(new ServerSocialInviteResponse
            {
                Invitee    = invitee.Character.Name,
                SocialType = Type
            });

            invitee.Session.Send(new ServerSocialInviteUpdate
            {
                CharacterId = host.Character.Id,
                UnixTime    = (uint)DateTimeOffset.UtcNow.AddSeconds(TimeToRespond).ToUnixTimeSeconds(),
                Name        = host.Character.Name,
                SocialType  = Type,
                UpdateType  = SocialInviteUpdateType.Invite
            });
        }

        protected abstract bool CanInvite(Player host, Player invitee);

        protected void InviteCancelAll()
        {
            pendingInvites.ForEach(InviteCancel);
        }

        protected virtual void InviteCancel(PendingInvite pendingInvite)
        {
            Debug.Assert(pendingInvite != null);

            Player invitee = MapManager.FindPlayer(pendingInvite.InviteeId);
            if (invitee == null)
                throw new InvalidPlayerException(pendingInvite.InviteeId);

            invitee.Session.Send(new ServerSocialInviteUpdate
            {
                CharacterId = pendingInvite.HostId,
                Name        = pendingInvite.HostName,
                SocialType  = Type,
                UpdateType  = SocialInviteUpdateType.Canceled
            });

            RemoveInvite(invitee, pendingInvite);
        }

        /// <summary>
        /// Update result for a pending social invite.
        /// </summary>
        public void InviteResponse(Player invitee, byte result)
        {
            Debug.Assert(invitee != null);

            PendingInvite pendingInvite = FindInvite(invitee);
            if (pendingInvite == null)
                throw new SocialInviteStateException($"Character {invitee.Character.Id} doesnt't have a pending {Type} invite!");

            RemoveInvite(invitee, pendingInvite);

            // declined
            if (result == 0)
            {
                invitee.Session.Send(new ServerSocialMessage
                {
                    Name       = pendingInvite.HostName,
                    SocialType = Type
                });

                Player host = MapManager.FindPlayer(pendingInvite.HostId);
                host?.Session.Send(new ServerSocialInviteUpdate
                {
                    CharacterId = invitee.Character.Id,
                    Name        = invitee.Character.Name,
                    SocialType  = Type,
                    UpdateType  = SocialInviteUpdateType.Declined
                });
            }
            // accepted
            else
            {
                invitee.DeclineSocialInvites(Type);
                MemberAdd(invitee, true);
            }
        }

        public abstract void MemberAdd(Player invitee, bool broadcast);

        public virtual void SendSocialList(Player recipient, ushort sequence) { }

        public virtual void Update(double lastTick)
        {
            if (pendingInvites.Count == 0)
                return;

            foreach (PendingInvite invite in pendingInvites.ToArray())
            {
                if (invite.ResponseTime < lastTick)
                    InviteCancel(invite);
                else
                    invite.ResponseTime -= lastTick;
            }
        }
    }
}
