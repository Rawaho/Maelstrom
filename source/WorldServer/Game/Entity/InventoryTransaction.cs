using System;
using System.Collections.Generic;
using Shared.Game;
using WorldServer.Game.Entity.Enums;
using WorldServer.Network.Message;

namespace WorldServer.Game.Entity
{
    public partial class Inventory
    {
        /// <summary>
        /// Used to force update inventory at client.
        /// </summary>
        public class InventoryTransaction
        {
            private struct TransactionAction
            {
                public InventoryAction InventoryAction { get; }
                public ContainerType Source { get; }
                public ushort SrcSlot { get; }
                public ContainerType Destination { get; }
                public ushort DstSlot { get; }

                public TransactionAction(InventoryAction inventoryAction, ContainerType source, ushort srcSlot, ContainerType destination, ushort dstSlot)
                {
                    InventoryAction = inventoryAction;
                    Source          = source;
                    SrcSlot         = srcSlot;
                    Destination     = destination;
                    DstSlot         = dstSlot;
                }
            }
        
            public static QueuedCounter<uint> Generator { get; } = new QueuedCounter<uint>(0u, true);

            private readonly uint id;
            private readonly Player owner;
            private readonly Queue<TransactionAction> actions = new Queue<TransactionAction>();

            public InventoryTransaction(Player player)
            {
                id    = Generator.DequeueValue();
                owner = player;
            }
            
            /// <summary>
            /// Queue a forced inventory action that will be performed on commit.
            /// </summary>
            public void Add(InventoryAction inventoryAction, ContainerType source, ushort srcSlot, ContainerType destination = ContainerType.None, ushort dstSlot = ushort.MaxValue)
            {
                actions.Enqueue(new TransactionAction(inventoryAction, source, srcSlot, destination, dstSlot));
            }
            
            /// <summary>
            /// Commit all pending inventory actions in this transaction.
            /// </summary>
            public void Commit()
            {
                while (actions.Count > 0)
                {
                    TransactionAction action = actions.Dequeue();
                    Container srcContainer = owner.Inventory.GetContainer(action.Source);
                    Container dstContainer = owner.Inventory.GetContainer(action.Destination);

                    Item srcItem = srcContainer.GetItem(action.SrcSlot);
                    if (srcItem == null && action.Source != ContainerType.None)
                        return;
                    
                    Item dstItem = dstContainer?.GetItem(action.DstSlot);
                    if (dstItem == null && action.Destination != ContainerType.None)
                        return;

                    switch (action.InventoryAction)
                    {
                        case InventoryAction.Discard:
                            owner.Inventory.RemoveItem(srcItem, srcContainer);
                            break;
                        #if DEBUG
                        default:
                            Console.WriteLine($"Unhandled inventory transaction action {action.InventoryAction}!");
                            break;
                        #endif
                    }

                    owner.Session.Send(new ServerInventoryUpdate
                    {
                        Id               = id,
                        Action           = action.InventoryAction,
                        SrcActorId       = owner.Character.ActorId,
                        SrcContainerType = action.Source,
                        SrcSlot          = action.SrcSlot,
                        SrcStackSize     = srcItem?.StackSize ?? 0u,
                        DstActorId       = owner.Character.ActorId,
                        DstContainerType = action.Destination,
                        DstSlot          = action.DstSlot,
                        DstStackSize     = dstItem?.StackSize ?? 0u
                    });
                }

                owner.Session.Send(new ServerInventoryUpdateFinish
                {
                    Id = id
                });
            }
        }
    }
}
