using System.Collections;
using System.IO;
using Shared.Database.Datacentre;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerPlayerSetup, SubPacketDirection.Server)]
    public class ServerPlayerSetup : SubPacket
    {
        public CharacterInfo Character;

        // TODO: finish mapping this, it needs a lot of work
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Character.Id);
            writer.Pad(8u);
            writer.Write(Character.ActorId);
            writer.Write(0u);           // rested XP
            writer.Write((ushort)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)Character.Appearance.Race);
            writer.Write(Character.Appearance.Clan);
            writer.Write((byte)Character.Appearance.Sex);
            writer.Write(Character.ClassJobId);
            writer.Write(Character.ClassId);
            writer.Write(Character.Guardian);
            writer.Write(Character.BirthMonth);
            writer.Write(Character.BirthDay);
            writer.Write((byte)0);      // city state
            writer.Write((byte)0);      // return Aetheryte
            writer.Write((byte)0);

            // companion related
            writer.Write((byte)0);      // shows companion action bar
            writer.Write((byte)0);      // companion rank
            writer.Write((byte)0);      // companion stars
            writer.Write((byte)0);      // companion available skill points
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Pad(2u);
            writer.Write(0f);
            writer.Write(0u);           // companion XP

            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);           // fish caught
            writer.Write(0u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((ushort)0);    // player commendations
            writer.Write((ushort)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((ushort)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((ushort)0);
            writer.Write(0u);           // Frontline campaigns
            writer.Write((ushort)0);    // Frontline campaigns weekly 
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((ushort)0);    // sub_A24E10(&unk_14FB360, *(payload + 120));
            writer.Write((byte)0);      // *(payload + 122) != 0;
            writer.Pad(1u);
            writer.Write(0u);
            writer.Write(0u);
            writer.Write((ushort)0);    //
            writer.Write((ushort)0);    //
            writer.Write((ushort)0);    //
            writer.Write((ushort)0);    //
            writer.Write(0u);           //
            writer.Write(0u);           //
            writer.Write(0u);           //
            writer.Write(0u);           //
            writer.Write(0u);           //
            writer.Write(0u);           //
            writer.Write((byte)0);      //
            writer.WriteStringLength(Character.Name, 0x20);
            writer.Pad(16u);            // ??
            writer.Write((byte)0);      //

            for (int i = 0; i < CharacterClassInfo.MaxClassId; i++)
                writer.Write((ushort)70);
            /*writer.Write(Character.Classes[i].Level);*/

            for (int i = 0; i < CharacterClassInfo.MaxClassId; i++)
                writer.Write(Character.Classes[i].Experience);

            // enables various features (emotes) and UI elements
            var masterMask = new BitArray(64 * 8, true);
            writer.Write(masterMask.ToArray());

            var aetheryteMask = new BitArray(16 * 8, true);
            writer.Write(aetheryteMask.ToArray());

            writer.Write((byte)0);      // 01BC
            writer.Write((byte)0);      // 01BD
            writer.Write((byte)0);      // 01BE
            writer.Write((byte)0);      // 01BF

            // territory discovery mask
            var territoryDiscovery = new BitArray(320 * 8, true);
            writer.Write(territoryDiscovery.ToArray());

            var territoryDiscovery2 = new BitArray(96 * 8, true);
            writer.Write(territoryDiscovery2.ToArray());

            var activeHelpMask = new BitArray(33 * 8, true);
            writer.Write(activeHelpMask.ToArray());

            var minionMask = new BitArray(33 * 8, true);
            writer.Write(minionMask.ToArray());

            var chocoboPorterMask = new BitArray(8 * 8, true);
            writer.Write(chocoboPorterMask.ToArray());

            var someMask = new BitArray(104 * 8, true);
            writer.Write(someMask.ToArray());

            var companionBardingMask = new BitArray(8 * 8, true);
            writer.Write(companionBardingMask.ToArray());

            // companion related
            writer.Write((byte)0);      // companion gear head
            writer.Write((byte)0);      // companion gear body
            writer.Write((byte)0);      // companion gear feet
            writer.Write((byte)0);      // 041D
            writer.Write((byte)0);      // 041E
            writer.Write((byte)0);      // 041F
            writer.Write((byte)0);      // 0420
            writer.Pad(11u);            // gap0421
            writer.WriteStringLength("Name", 0x15);
            writer.Write((byte)0);      // companion defender level
            writer.Write((byte)0);      // companion attacker level
            writer.Write((byte)0);      // companion healer level

            var mountMask = new BitArray(13 * 8, true);
            writer.Write(mountMask.ToArray());

            var fishingLogCatchMask = new BitArray(77 * 8, true);
            writer.Write(fishingLogCatchMask.ToArray());

            var fishingLogLocationMask = new BitArray(26 * 8, true);
            writer.Write(fishingLogLocationMask.ToArray());

            // fishing log record fish
            for (uint i = 0u; i < 26; i++)
                writer.Write((ushort)0);

            // fishing log record fish weight
            for (uint i = 0u; i < 26; i++)
                writer.Write((ushort)0);

            var someMask2 = new BitArray(15 * 8, true);
            writer.Write(someMask2.ToArray());

            // PvP related
            writer.Write((byte)0);      // 052F
            writer.Write((byte)0);      // 0530
            writer.Pad(3u);             // gap0531
            writer.Write(0u);           // 0534
            writer.Write(0u);           // 0538
            writer.Write(0u);           // 053C
            writer.Write((byte)0);      // 0540
            writer.Write((byte)0);      // 0541
            writer.Write((byte)0);      // 0542

            writer.Write((byte)0);      // 0543
            writer.Write((byte)0);      // 0544
            writer.Write((byte)0);      // 0545
            writer.Write((byte)0);      // 0546
            writer.Write((byte)0);      // 0547
            writer.Write((byte)0);      // 0548
            writer.Write((byte)0);      // 0549
            writer.Write((byte)0);      // 054A
            writer.Write((byte)0);      // gap054B
            writer.Write((ushort)0);    // 054C
            writer.Write((ushort)0);    // 054E
            writer.Write((ushort)0);    // 0550
            writer.Write((ushort)0);    // 0552
            writer.Write((ushort)0);    // 0554
            writer.Write((ushort)0);    // 0556
            writer.Write((ushort)0);    // 0558
            writer.Write((ushort)0);    // 055A

            writer.Write((byte)0);      // 055C
            writer.Write((byte)0);      // 055D
            writer.Write((byte)0);      // 055E
            writer.Write((byte)0);      // 055F
            writer.Write((byte)0);      // 0560
            writer.Write((byte)0);      // 0561
            writer.Write((byte)0);      // 0562
            writer.Write((byte)0);      // 0563
            writer.Write((byte)0);      // 0564
            writer.Write((byte)0);      // 0565
            writer.Write((ushort)0);    // 0556
            writer.Write((ushort)0);    // 0568
            writer.Write((ushort)0);    // 056A
            writer.Write((ushort)0);    // 056C
            writer.Write((ushort)0);    // 056E

            var someMask3 = new BitArray(5 * 8, true);
            writer.Write(someMask3.ToArray());

            writer.Write((byte)0);      // 0575
            writer.Write((byte)0);      // 0576
            writer.Pad(7u);             // gap0577
            writer.Write((byte)0);      // 057E
            writer.Write((byte)0);      // 057F
            writer.Write((byte)0);      // 0580
            writer.Write((byte)0);      // 0581
            writer.Write((byte)0);      // 0582
            writer.Write((byte)0);      // 0583
            writer.Write((byte)0);      // 0584

            var someMask4 = new BitArray(28 * 8, true);
            writer.Write(someMask4.ToArray());

            writer.Pad(3u);             // gap05A1
            writer.Write(0u);           // 05A4

            // related, all read in same client function
            writer.Write((byte)0);      // 05A8
            writer.Write((byte)0);      // 05A9
            writer.Write((byte)0);      // 05AA
            writer.Write((byte)0);      // 05AB
            writer.Write((byte)0);      // 05AC
            writer.Write((byte)0);      // 05AD
            writer.Write((byte)0);      // 05AE
            writer.Write((byte)0);      // 05AF
            writer.Write((byte)0);      // 05B0
            writer.Write((byte)0);      // 05B1
            writer.Write((byte)0);      // 05B2
            writer.Write((byte)0);      // 05B3

            var someMask5 = new BitArray(24 * 8, true);
            writer.Write(someMask5.ToArray());

            writer.Write(0u);           // Frontline 1st place
            writer.Write(0u);           // Frontline 2nd place
            writer.Write(0u);           // Frontline 3rd place
            writer.Write((ushort)0);    // Frontline 1st place weekly
            writer.Write((ushort)0);    // Frontline 2nd place weekly
            writer.Write((ushort)0);    // Frontline 3rd place weekly

            // not a mask, 55 seperate bytes
            for (int i = 0; i < 55; i++)
                writer.Write((byte)0);

            var tripleTriadCardMask = new BitArray(26 * 8, true);
            writer.Write(tripleTriadCardMask.ToArray());

            writer.Write((byte)0);      // 062F
            writer.Write((byte)0);      // 0630
            writer.Write((byte)0);      // 0631
            writer.Write((byte)0);      // 0632
            writer.Write((byte)0);      // 0633
            writer.Write((byte)0);      // 0634
            writer.Write((byte)0);      // 0635
            writer.Write((byte)0);      // 0636
            writer.Write((byte)0);      // 0637
            writer.Write((byte)0);      // 0638

            var someMask6 = new BitArray(22 * 8, true);
            writer.Write(someMask6.ToArray());

            writer.Write((byte)0);      // 064F
            writer.Write((byte)0);      // 0650
            writer.Write((byte)0);      // 0651

            var orchestrionMask = new BitArray(21 * 8, true);
            writer.Write(orchestrionMask.ToArray());

            writer.Write((byte)0);      // 0667
            writer.Write((byte)0);      // 0668
            writer.Write((byte)0);      // 0669

            // shows certain key items in bag
            var keyItemMask = new BitArray(11 * 8, false);
            writer.Write(keyItemMask.ToArray());

            var someMask7 = new BitArray(16 * 8, true);
            writer.Write(someMask7.ToArray());

            writer.Write((byte)0);      // 0688
            writer.Write((byte)0);      // 0686
            writer.Write((byte)0);      // 0687
            writer.Write((byte)0);      // 0688
            writer.Pad(8u);
        }
    }
}
