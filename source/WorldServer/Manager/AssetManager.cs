using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Shared.Game;
using WorldServer.Game.Entity;
using WorldServer.Game.Entity.Enums;

namespace WorldServer.Manager
{
    public static class AssetManager
    {
        public static QueuedCounter<ulong> ItemId { get; private set; }
        
        /// <summary>
        /// Contains player inventory container max capacities.
        /// </summary>
        public static ReadOnlyDictionary<ContainerType, ushort> ContainerCapacities { get; private set; }

        /// <summary>
        /// Contains Armoury container types for items that can be equipped into Armoury containers.
        /// </summary>
        public static ReadOnlyDictionary<ItemUiCategory, ContainerType> EquipArmouryContainerTypes { get; private set; }

        public static void Initialise()
        {
            ItemId = new QueuedCounter<ulong>(0, true);
            InitialiseContainerTypeAttributes();
        }

        private static void InitialiseContainerTypeAttributes()
        {
            var containerCapacities        = new Dictionary<ContainerType, ushort>();
            var equipArmouryContainerTypes = new Dictionary<ItemUiCategory, ContainerType>();
            
            foreach (ContainerType containerType in Enum.GetValues(typeof(ContainerType)))
            {
                if (containerType == ContainerType.None)
                    continue;
                
                MemberInfo member = containerType.GetType().GetMember(containerType.ToString()).FirstOrDefault();
                Debug.Assert(member != null);

                ContainerTypeAttribute attribute = member.GetCustomAttribute<ContainerTypeAttribute>();
                Debug.Assert(attribute != null, "Container type is missing attribute!");

                // cache item ui category to armoury container types 
                if (containerType >= ContainerType.ArmouryOffHand && containerType <= ContainerType.ArmouryMainHand)
                    for (int i = 0; i < attribute.ItemCategories.Count; i++)
                        if (attribute.ItemCategories[i])
                            equipArmouryContainerTypes.Add((ItemUiCategory)i, containerType);

                containerCapacities.Add(containerType, attribute.Capacity);
            }

            ContainerCapacities        = new ReadOnlyDictionary<ContainerType, ushort>(containerCapacities);
            EquipArmouryContainerTypes = new ReadOnlyDictionary<ItemUiCategory, ContainerType>(equipArmouryContainerTypes);
        }
    }
}
