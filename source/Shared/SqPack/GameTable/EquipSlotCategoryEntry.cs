using System.Collections.Generic;

namespace Shared.SqPack.GameTable
{
    public class EquipSlotCategoryEntry : Exd.Entry
    {
        public IEnumerable<ushort> GetEquipSlots()
        {
            for (ushort i = 0; i < Data.Length; i++)
                if ((sbyte)Data[i] == 1)
                    yield return i;
        }
    }
}
