using System;
using System.Linq;
using System.Runtime.InteropServices;
using Shared.Network;

namespace Shared.SqPack.GameTable
{
    public class AchievementEntry : Exd.Entry
    {
        public byte AchievementCategoryId => (byte)Data[0];
        public string Title => (string)Data[1];
        public string Description => (string)Data[2];
        public byte Points => (byte)Data[3];
        public ushort RewardTitleId => (ushort)Data[4];
        public uint RewardItemId => (uint)Data[5];
        public ushort IconId => (ushort)Data[6];
        // 7
        public byte CriteriaType => (byte)Data[8];      // AchievementCriteriaType
        // 9-17 - these vary depending on CriteriaType
        public int[] CriteriaParameters => Data.RangeSubset(9, 9).Cast<int>().ToArray();
        // 18
        
        public CriteriaDataUnion CriteriaData;
        
        [StructLayout(LayoutKind.Explicit)]
        public struct CriteriaDataUnion
        {
            // Counter
            [FieldOffset(0)]
            public uint CriteriaCounterTypeId;

            // Level
            [FieldOffset(0)]
            public uint JobClassId;
            [FieldOffset(4)]
            public uint ClassLevel;
            
            // MateriaMelding
            [FieldOffset(0)]
            public uint MateriaCount;

            // ReputationRank
            [FieldOffset(0)]
            public uint BeastTribeId;
            [FieldOffset(4)]
            public uint BeastReputationRankId;

            // Minion
            [FieldOffset(4)]
            public uint MinionCount;
        }

        public override void Initialise()
        {
            byte[] buffer = new byte[CriteriaParameters.Length * sizeof(int)];
            Buffer.BlockCopy(CriteriaParameters, 0, buffer, 0, buffer.Length);

            CriteriaData = buffer.UnMarshal<CriteriaDataUnion>();
        }
    }
}
