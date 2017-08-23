using System.Collections;
using System.Collections.Generic;
using System.IO;
using Shared.Network;
using WorldServer.Game.Entity;
using WorldServer.Game.Social;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerSocialList)]
    public class ServerSocialList : SubPacket
    {
        public class Member
        {
            public ulong Id { get; }
            public string Name { get; }
            public ushort TerritoryId { get; }
            public byte Language { get; } = 1;
            public uint LanguageMask { get; } = 0x02u;
            public BitArray OnlineStatusMask { get; } = new BitArray(6 * 8);
            public byte GrandCompany { get; }
            public byte ClassJobId { get; }
            public ushort Level { get; }

            public Member(Player player)
            {
                Id           = player.Character.Id;
                Name         = player.Character.Name;
                TerritoryId  = (ushort)player.Map.Entry.Index;
                ClassJobId   = player.Character.ClassJobId;
                Level        = player.Character.GetClassInfo(player.Character.ClassJobId).Level;

                OnlineStatusMask.Set(47, true);
            }

            public Member(Party.Member member)
            {
                Id   = member.Id;
                Name = member.Name;
            }
        }

        public SocialListType ListType;
        public ushort Sequence;
        public List<Member> SocialMembers = new List<Member>();

        public override void Write(BinaryWriter writer)
        {
            writer.Pad(12u);
            writer.Write((byte)ListType);
            writer.Write(Sequence);
            writer.Pad(1u);

            for (int i = 0; i < Party.FullPartySize; i++)
            {
                if (i < SocialMembers.Count)
                {
                    Member member = SocialMembers[i];
                    member.OnlineStatusMask.Set(5, true);

                    writer.Write(member.Id);
                    writer.Write((ushort)0);
                    writer.Write(member.TerritoryId);
                    writer.Write((ushort)45);
                    writer.Write((ushort)57);
                    writer.Write((byte)0);            // enables disband button (flags?)
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write((byte)16);
                    writer.Write(member.TerritoryId);
                    writer.Write(member.GrandCompany);
                    writer.Write(member.Language);
                    writer.Write(member.LanguageMask);
                    writer.Write(0u);
                    writer.Write(member.OnlineStatusMask.ToArray());
                    writer.Pad(2u);
                    writer.Write(member.ClassJobId);
                    writer.Write((byte)0);
                    writer.Write(member.Level);
                    writer.Pad(3u);
                    writer.WriteStringLength(member.Name, 0x20);
                    writer.WriteStringLength("", 6);
                    writer.Pad(3u);
                }
                else
                    writer.Pad(88u);
            }

            writer.Pad(176);
        }
    }
}
