using System.Diagnostics;
using Shared.SqPack.GameTable;
using WorldServer.Game.Entity.Enums;
using WorldServer.Network.Message;

namespace WorldServer.Game.Entity
{
    public class Item
    {
        public ItemEntry Template { get; }

        public ContainerType ContainerType { get; private set; } = ContainerType.None;
        public ushort Slot { get; private set; }
        public ulong Guid { get; }
        public uint StackSize { get; private set; }

        private readonly Player owner;

        public Item(Player player, ItemEntry template, ulong guid, uint stackSize = 1u)
        {
            Debug.Assert(player != null);
            Debug.Assert(template != null);
            
            owner         = player;
            Template      = template;
            Guid          = guid;
            StackSize     = stackSize;
        }

        public void UpdatePosition(ContainerType containerType, ushort slot, bool update = false)
        {
            ContainerType = containerType;
            Slot          = slot;
            
            if (update)
                SendItemUpdate();
        }

        public void UpdateStackSize(uint stackChange, bool add = true)
        {
            Debug.Assert(stackChange != 0);
            
            checked
            {
                if (add)
                    StackSize += stackChange;
                else
                    StackSize -= stackChange;
            }
            
            SendItemUpdate();
        }

        public void SendSetup(uint index)
        {
            owner.Session.Send(new ServerItemSetup
            {
                Index         = index,
                ContainerType = ContainerType,
                Slot          = Slot,
                ItemId        = Template.Index,
                StackSize     = StackSize
            });
        }
        
        private void SendItemUpdate()
        {
            if (!owner.InWorld)
                return;
            
            owner.Session.Send(new ServerItemUpdate
            {
                ContainerType = ContainerType,
                Slot          = Slot,
                ItemId        = Template.Index,
                StackSize     = StackSize
            });
        }
    }
}
