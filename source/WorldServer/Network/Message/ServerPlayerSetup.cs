using System.Collections;
using System.IO;
using Shared.Database.Datacentre;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketServerOpcode.ServerPlayerSetup)]
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
            writer.Write((byte)0);      // 0060
            writer.Write((byte)0);      // 0061
            writer.Write((byte)0);      // 0062
            writer.Write((byte)0);      // 0063
            writer.Write((ushort)0);    // 0064
            writer.Write((byte)0);      // 0066
            writer.Write((byte)0);      // 0067
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
            writer.Write((ushort)0);    // 0078
            writer.Write((byte)0);      // 007A
            writer.Pad(1u);
            writer.Write(0u);           // 007C
            writer.Write(0u);           // 0080
            writer.Write((ushort)0);    // 0084
            writer.Write((ushort)0);    // 0086
            writer.Write((ushort)0);    // 0088
            writer.Write((ushort)0);    // 008A
            writer.Write(0u);           // 008C
            writer.Write(0u);           // 0090
            writer.Write(0u);           // 0094
            writer.Write(0u);           // 0098
            writer.Write(0u);           // 009C

            writer.Pad(11u);            // 00A0
            writer.Write((ushort)0);    // 00AB
            writer.Write((byte)0);      // 00AD
            writer.WriteStringLength(Character.Name, 0x20);
            writer.Pad(16u);            // 00CE
            writer.Write((byte)0);      // 00DE
            writer.Write((byte)0);      // 00DF
            writer.Write((ushort)0);    // 00E0

            for (int i = 0; i < CharacterClassInfo.MaxClassId; i++)
                writer.Write(Character.Classes[i].Level);

            for (int i = 0; i < CharacterClassInfo.MaxClassId; i++)
                writer.Write(Character.Classes[i].Experience);

            // enables various features (emotes) and UI elements
            var masterMask = new BitArray(64 * 8, true);
            writer.Write(masterMask.ToArray());

            var aetheryteMask = new BitArray(16 * 8, true);
            writer.Write(aetheryteMask.ToArray());

            writer.Write((byte)0);      // 01C8
            writer.Write((byte)0);      // 01C9
            writer.Write((byte)0);      // 01CA
            writer.Write((byte)0);      // 01CB

            // territory discovery mask
            var territoryDiscovery = new BitArray(320 * 8, true);
            writer.Write(territoryDiscovery.ToArray());

            var territoryDiscovery2 = new BitArray(96 * 8, true);
            writer.Write(territoryDiscovery2.ToArray());

            var activeHelpMask = new BitArray(33 * 8, true);
            writer.Write(activeHelpMask.ToArray());

            var minionMask = new BitArray(35 * 8, true);
            writer.Write(minionMask.ToArray());

            var chocoboPorterMask = new BitArray(8 * 8, true);
            writer.Write(chocoboPorterMask.ToArray());

            var someMask = new BitArray(105 * 8, true);
            writer.Write(someMask.ToArray());

            writer.Pad(1u);
            writer.Write((ushort)0);    // 0422
            writer.Write((ushort)0);    // 0424
            writer.Write((ushort)0);    // 0426
            writer.Write((ushort)0);    // 0428
            writer.Write((ushort)0);    // 042A
            writer.Write((ushort)0);    // 042C
            writer.Write((ushort)0);    // 042E
            writer.Write((ushort)0);    // 0430

            var companionBardingMask = new BitArray(8 * 8, true);
            writer.Write(companionBardingMask.ToArray());

            // companion related
            writer.Write((byte)0);      // companion gear head
            writer.Write((byte)0);      // companion gear body
            writer.Write((byte)0);      // companion gear feet
            writer.Write((byte)0);      // 043D
            writer.Write((byte)0);      // 043E
            writer.Write((byte)0);      // 043F
            writer.Write((byte)0);      // 0440

            var someMask8 = new BitArray(11 * 8, true);
            writer.Write(someMask8.ToArray());

            writer.WriteStringLength("Name", 0x15);
            writer.Write((byte)1);      // companion defender level
            writer.Write((byte)2);      // companion attacker level
            writer.Write((byte)3);      // companion healer level

            var mountMask = new BitArray(14 * 8, true);
            writer.Write(mountMask.ToArray());

            var fishingLogCatchMask = new BitArray(89 * 8, true);
            writer.Write(fishingLogCatchMask.ToArray());

            var fishingLogLocationMask = new BitArray(25 * 8, true);
            writer.Write(fishingLogLocationMask.ToArray());

            // fishing log record fish
            for (uint i = 0u; i < 26; i++)
                writer.Write((ushort)0);

            // fishing log record fish weight
            for (uint i = 0u; i < 26; i++)
                writer.Write((ushort)0);

            var someMask2 = new BitArray(15 * 8, true); // 54C
            writer.Write(someMask2.ToArray());

            // PvP related
            writer.Write((byte)0);      // 055B
            writer.Write((byte)0);      // 055C
            writer.Pad(3u);             // gap055D
            writer.Write(0u);           // 0560
            writer.Write(0u);           // 0564
            writer.Write(0u);           // 0568
            writer.Write((byte)0);      // 056C
            writer.Write((byte)0);      // 056D
            writer.Write((byte)0);      // 056E

            // beast tribe
            writer.Write((byte)0);      // 056F
            writer.Write((byte)0);      // 0570
            writer.Write((byte)0);      // 0571
            writer.Write((byte)0);      // 0572
            writer.Write((byte)0);      // 0573
            writer.Write((byte)0);      // 0574
            writer.Write((byte)0);      // 0575
            writer.Write((byte)0);      // 0576
            writer.Write((byte)0);      // 0577
            writer.Write((ushort)0);    // 0578
            writer.Write((ushort)0);    // 057A
            writer.Write((ushort)0);    // 057C
            writer.Write((ushort)0);    // 057E
            writer.Write((ushort)0);    // 0580
            writer.Write((ushort)0);    // 0582
            writer.Write((ushort)0);    // 0584
            writer.Write((ushort)0);    // 0586
            writer.Write((ushort)0);    // 0588

            writer.Write((byte)0);      // 058A
            writer.Write((byte)0);      // 058B
            writer.Write((byte)0);      // 058C
            writer.Write((byte)0);      // 058D
            writer.Write((byte)0);      // 058E
            writer.Write((byte)0);      // 058F
            writer.Write((byte)0);      // 0590
            writer.Write((byte)0);      // 0591
            writer.Write((byte)0);      // 0592
            writer.Write((byte)0);      // 0593
            writer.Write((ushort)0);    // 0594
            writer.Write((ushort)0);    // 0596
            writer.Write((ushort)0);    // 0598
            writer.Write((ushort)0);    // 059A
            writer.Write((ushort)0);    // 059C

            var someMask3 = new BitArray(5 * 8, true);
            writer.Write(someMask3.ToArray());

            writer.Write((byte)0);      // 05A3
            writer.Write((byte)0);      // 05A4

            writer.Write(0u);           // 05A5
            writer.Write((ushort)0);    // 05A9
            writer.Write((byte)0);      // 05AB

            writer.Write((byte)0);      // 05AC
            writer.Write((byte)0);      // 05AD
            writer.Write((byte)0);      // 05AE
            writer.Write((byte)0);      // 05AF
            writer.Write((byte)0);      // 05B0
            writer.Write((byte)0);      // 05B1
            writer.Write((byte)0);      // 05B2

            var someMask4 = new BitArray(28 * 8, true);
            writer.Write(someMask4.ToArray());

            writer.Pad(1u);             // gap05D9
            writer.Write(0u);           // 05D0

            // related, all read in same client function
            writer.Write((byte)0);      // 05D4
            writer.Write((byte)0);      // 05D5
            writer.Write((byte)0);      // 05D6
            writer.Write((byte)0);      // 05D7
            writer.Write((byte)0);      // 05D8
            writer.Write((byte)0);      // 05D9
            writer.Write((byte)0);      // 05DA
            writer.Write((byte)0);      // 05DB
            writer.Write((byte)0);      // 05DC
            writer.Write((byte)0);      // 05DD
            writer.Write((byte)0);      // 05DE
            writer.Write((byte)0);      // 05DF

            var someMask5 = new BitArray(26 * 8, true);
            writer.Write(someMask5.ToArray());

            writer.Write(0u);           // Frontline 1st place
            writer.Write(0u);           // Frontline 2nd place
            writer.Write(0u);           // Frontline 3rd place
            writer.Write((ushort)0);    // Frontline 1st place weekly
            writer.Write((ushort)0);    // Frontline 2nd place weekly
            writer.Write((ushort)0);    // Frontline 3rd place weekly
            writer.Pad(2u);

            // not a mask, 55 seperate bytes
            for (int i = 0; i < 55; i++)
                writer.Write((byte)0);

            var tripleTriadCardMask = new BitArray(27 * 8, true);
            writer.Write(tripleTriadCardMask.ToArray());

            writer.Write((byte)0);      // 0660
            writer.Write((byte)0);      // 0661
            writer.Write((byte)0);      // 0662
            writer.Write((byte)0);      // 0663
            writer.Write((byte)0);      // 0664
            writer.Write((byte)0);      // 0665
            writer.Write((byte)0);      // 0666
            writer.Write((byte)0);      // 0667
            writer.Write((byte)0);      // 0668
            writer.Write((byte)0);      // 0669
            writer.Write((byte)0);      // 066A

            var someMask6 = new BitArray(22 * 8, true);
            writer.Write(someMask6.ToArray());

            writer.Write((byte)0);      // 0681
            writer.Write((byte)0);      // 0682
            writer.Write((byte)0);      // 0683

            var orchestrionMask = new BitArray(40 * 8, true);
            writer.Write(orchestrionMask.ToArray());

            writer.Write((byte)0);      // 06AC
            writer.Write((byte)0);      // 06AD
            writer.Write((byte)0);      // 06AE

            // shows certain key items in bag
            var keyItemMask = new BitArray(11 * 8, false);
            writer.Write(keyItemMask.ToArray());

            var someMask7 = new BitArray(16 * 8, true);
            writer.Write(someMask7.ToArray());

            writer.Write((byte)0);      // 06CA
            writer.Write((byte)0);      // 06CB
            writer.Write((byte)0);      // 06CC
            writer.Write((byte)0);      // 06CD
            writer.Pad(8u);
            writer.Write((byte)0);      // 06D6

            // 4.1
            var someMask9 = new BitArray(28 * 8, true);
            writer.Write(someMask9.ToArray());

            var someMask10 = new BitArray(18 * 8, true);
            writer.Write(someMask10.ToArray());

            writer.Write((byte)0);      // 0705
            writer.Write((byte)0);      // 0706
            writer.Write((byte)0);      // 0707
            writer.Write((byte)0);      // 0708
            writer.Write((byte)0);      // 0709
            writer.Write((byte)0);      // 070A
            writer.Write((byte)0);      // 070B
            writer.Write((byte)0);      // 070C
            writer.Write((byte)0);      // 070D
            writer.Write((byte)0);      // 070E
            writer.Write((byte)0);      // 070F
            writer.Write((byte)0);      // 0710
            writer.Write((byte)0);      // 0711
            writer.Write((byte)0);      // 0712
            writer.Write((byte)0);      // 0713
            writer.Write((byte)0);      // 0714
            writer.Write((byte)0);      // 0715
            writer.Write((byte)0);      // 0716
            writer.Write((byte)0);      // 0717
            writer.Write((byte)0);      // 0718
            writer.Write((byte)0);      // 0719
            writer.Write((byte)0);      // 071A

            var someMask11 = new BitArray(28 * 8, true);
            writer.Write(someMask11.ToArray());

            var someMask12 = new BitArray(18 * 8, true);
            writer.Write(someMask12.ToArray());

            writer.Write((byte)0);      // 0749
            writer.Write((byte)0);      // 074A
            writer.Write((byte)0);      // 074B
            writer.Write((byte)0);      // 074C
            writer.Write((byte)0);      // 074D
            writer.Write((byte)0);      // 074E
            writer.Write((byte)0);      // 074F
            writer.Write((byte)0);      // 0750
            writer.Write((byte)0);      // 0751
            writer.Write((byte)0);      // 0752
            writer.Write((byte)0);      // 0753
            writer.Write((byte)0);      // 0754
            writer.Write((byte)0);      // 0755
            writer.Write((byte)0);      // 0756
            writer.Write((byte)0);      // 0757
            writer.Write((byte)0);      // 0758
            writer.Write((byte)0);      // 0759
            writer.Write((byte)0);      // 075A
            writer.Write((byte)0);      // 075B
            writer.Write((byte)0);      // 075C
            writer.Write((byte)0);      // 075D
            writer.Write((byte)0);      // 075E
        }
    }
}
