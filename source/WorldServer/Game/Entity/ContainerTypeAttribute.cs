using System;
using System.Collections;
using Shared.SqPack;
using WorldServer.Game.Entity.Enums;

namespace WorldServer.Game.Entity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ContainerTypeAttribute : Attribute
    {
        public ushort Capacity { get; }
        public BitArray ItemCategories { get; } = new BitArray(GameTableManager.ItemUiCategories.Count);

        public ContainerTypeAttribute(ushort capacity, params ItemUiCategory[] categories)
        {
            Capacity = capacity;

            if (categories.Length > 0)
            {
                foreach (ItemUiCategory category in categories)
                    ItemCategories.Set((int)category, true);
            }
            else
                ItemCategories.SetAll(true);
        }
    }
}
