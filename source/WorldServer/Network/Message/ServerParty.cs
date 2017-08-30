using System.Collections.Generic;
using System.IO;
using Shared.Network;
using WorldServer.Game.Entity;
using WorldServer.Game.Social;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerParty)]
    public class ServerParty : SubPacket
    {
        public class Member
        {
            public ulong Id { get; }
            public string Name { get; }
            public uint ActorId { get; }
            public byte ClassJobId { get; }
            public ushort Level { get; }
            public uint Hp { get; }
            public uint MaxHp { get; }
            public ushort Mp { get; }
            public ushort MaxMp { get; }
            public ushort Tp { get; }
            public ushort TerritoryId { get; }

            public Member(Player player)
            {
                Id          = player.Character.Id;
                Name        = player.Character.Name;
                ActorId     = player.Character.ActorId;
                ClassJobId  = player.Character.ClassJobId;
                Level       = player.Character.GetClassInfo(player.Character.ClassJobId).Level;
                TerritoryId = (ushort)player.Map.Entry.Key;
                Hp          = 100;
                MaxHp       = 100;
                Mp          = 100;
                MaxMp       = 100;
                Tp          = 1000;
            }

            public Member(Party.Member member)
            {
                Id   = member.Id;
                Name = member.Name;
            }
        }

        public byte LeaderIndex;
        public List<Member> PartyMembers = new List<Member>();

        public override void Write(BinaryWriter writer)
        {
            for (int i = 0; i < Party.FullPartySize; i++)
            {
                if (i < PartyMembers.Count)
                {
                    Member member = PartyMembers[i];
                    writer.WriteStringLength(member.Name, 0x20);
                    writer.Write(member.Id);
                    writer.Write(member.ActorId);
                    writer.Write((byte)0);
                    writer.Write(member.ClassJobId);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write(member.Level);
                    writer.Write((byte)0);
                    writer.Pad(1u);
                    writer.Write(member.Hp);
                    writer.Write(member.MaxHp);
                    writer.Write(member.Mp);
                    writer.Write(member.MaxMp);
                    writer.Write(member.Tp);
                    writer.Write(member.TerritoryId);
                    writer.Write(0u);
                    writer.Write(0u);
                    writer.Pad(364u);
                }
                else
                    writer.Pad(440u);
            }

            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(LeaderIndex);
            writer.Write((byte)PartyMembers.Count);
            writer.Write((byte)0);
            writer.Pad(5u);
        }
    }
}
