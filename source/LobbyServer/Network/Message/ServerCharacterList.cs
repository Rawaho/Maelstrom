using System.Collections.Generic;
using System.IO;
using Shared.Database.Datacentre;
using Shared.Game.Enum;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerCharacterList)]
    public class ServerCharacterList : SubPacket
    {
        public const byte MaxCharactersPerPacket = 2;

        public byte Offset;
        public byte VeteranRank;
        public uint DaysSubscribed;
        public uint SubscriptionDaysRemaining;
        public uint DaysTillNextVeteranRank;
        public ushort RealmCharacterLimit;
        public ushort AccountCharacterLimit;
        public Expansion Expansion;

        public List<(byte index, string RealmName, CharacterInfo character)> Characters = new List<(byte index, string RealmName, CharacterInfo character)>(2);

        public override void Write(BinaryWriter writer)
        {
            writer.Write(0ul);
            writer.Write(Offset);
            writer.Write((byte)(Characters.Count != 0 ? Characters.Count : MaxCharactersPerPacket));
            writer.Pad(34u);
            writer.Write((byte)0x80);   // flags of some sort (0x80 = hides subscription days remaining)
            writer.Write(VeteranRank);
            writer.Write((byte)0);
            writer.Pad(1u);
            writer.Write(DaysSubscribed);
            writer.Write(SubscriptionDaysRemaining);
            writer.Write(DaysTillNextVeteranRank);
            writer.Write(RealmCharacterLimit);
            writer.Write(AccountCharacterLimit);
            writer.Write((uint)Expansion);
            writer.Pad(4u);

            for (int i = 0; i < MaxCharactersPerPacket; i++)
            {
                if (i < Characters.Count)
                {
                    CharacterInfo characterInfo = Characters[i].character;
                    writer.Write(0u);                   // some character id, persistent even after removing character
                    writer.Write(0u);
                    writer.Write(characterInfo.Id);
                    writer.Write(Characters[i].index);
                    writer.Write((byte)0x00);           // flags (0x01 = invalid account, 0x02 = character rename, 0x08 = legacy)
                    writer.Pad(2u);
                    writer.Write(0u);
                    writer.Write(characterInfo.RealmId);
                    writer.WriteStringLength(characterInfo.Name, 0x20);
                    writer.WriteStringLength(Characters[i].RealmName, 0x20);
                    writer.WriteStringLength(characterInfo.BuildJsonData(), 1006);
                    writer.Write(0ul);
                    writer.Write(0ul);
                    writer.Write(0ul);
                }
                else
                    writer.Pad(1120);
            }
        }
    }
}
