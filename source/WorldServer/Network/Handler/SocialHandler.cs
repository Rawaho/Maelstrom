using System;
using Shared.Network;
using WorldServer.Game.Social;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class SocialHandler
    {
        [SubPacketHandler(SubPacketClientOpcode.ClientPartyLeave, SubPacketHandlerFlags.RequiresWorld | SubPacketHandlerFlags.RequiresParty)]
        public static void HandleSocialLeave(WorldSession session, ClientPartyLeave partyLeave)
        {
            session.Player.Party.MemberLeave(session.Player);
        }

        [SubPacketHandler(SubPacketClientOpcode.ClientPartyDisband, SubPacketHandlerFlags.RequiresWorld | SubPacketHandlerFlags.RequiresParty)]
        public static void HandleSocialDisband(WorldSession session, ClientPartyDisband partyDisband)
        {
            session.Player.Party.MemberDisband(session.Player);
        }

        [SubPacketHandler(SubPacketClientOpcode.ClientPartyKick, SubPacketHandlerFlags.RequiresWorld | SubPacketHandlerFlags.RequiresParty)]
        public static void HandlePartyClientPartyKick(WorldSession session, ClientPartyKick partyKick)
        {
            session.Player.Party.MemberKick(session.Player, partyKick.Name);
        }

        [SubPacketHandler(SubPacketClientOpcode.ClientPartyPromote, SubPacketHandlerFlags.RequiresWorld | SubPacketHandlerFlags.RequiresParty)]
        public static void HandlePartyClientPartyPromote(WorldSession session, ClientPartyPromote partyPromote)
        {
            session.Player.Party.MemberPromote(session.Player, partyPromote.Name);
        }

        [SubPacketHandler(SubPacketClientOpcode.ClientSocialList, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleSocialList(WorldSession session, ClientSocialList socialList)
        {
            switch (socialList.ListType)
            {
                case SocialListType.Party:
                {
                    // party
                    if (session.Player.Party != null)
                        session.Player.Party.SendSocialList(session.Player, socialList.Sequence);
                    // solo
                    else
                    {
                        ServerSocialList serverSocialList = new ServerSocialList
                        {
                            ListType = SocialListType.Party,
                            Sequence = socialList.Sequence
                        };

                        serverSocialList.SocialMembers.Add(new ServerSocialList.Member(session.Player));
                        session.Send(serverSocialList);
                    }
                    break;
                }
                default:
                {
                    // throw new NotImplementedException($"Unhandled SocialListType {socialList.ListType}!");
                    #if DEBUG
                        Console.WriteLine($"Unhandled SocialListType {socialList.ListType}");
                    #endif
                    break;
                }
            }
        }

        [SubPacketHandler(SubPacketClientOpcode.ClientSocialInvite, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleSocialInvite(WorldSession session, ClientSocialInvite socialInvite)
        {
            switch (socialInvite.SocialType)
            {
                case SocialType.Party:
                    session.Player.PartyInvite(socialInvite.Invitee);
                    break;
                default:
                    throw new NotImplementedException($"Unhandled SocialType {socialInvite.SocialType}!");
            }
        }

        [SubPacketHandler(SubPacketClientOpcode.ClientSocialInviteResponse, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleSocialInviteResponse(WorldSession session, ClientSocialInviteResponse socialInviteResponse)
        {
            SocialInviteRequest inviteRequest = session.Player.FindSocialInvite(socialInviteResponse.CharacterId, socialInviteResponse.SocialType);
            if (inviteRequest == null)
                throw new SocialInviteStateException($"Character {session.Player.Character.Id} doesnt't have a pending {socialInviteResponse.SocialType} invite!");

            SocialBase socialEntity = SocialManager.FindSocialEntity<SocialBase>(socialInviteResponse.SocialType, inviteRequest.EntityId);
            socialEntity?.InviteResponse(session.Player, socialInviteResponse.Result);
        }
    }
}
