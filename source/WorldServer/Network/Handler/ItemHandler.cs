using System;
using Shared.Network;
using WorldServer.Game.Entity.Enums;
using WorldServer.Network.Message;

namespace WorldServer.Network.Handler
{
    public static class ItemHandler
    {
        [SubPacketHandler(SubPacketClientOpcode.ClientInventoryAction, SubPacketHandlerFlags.RequiresWorld)]
        public static void HandleClientInventoryAction(WorldSession session, ClientInventoryAction inventoryAction)
        {
            try
            {
                switch (inventoryAction.Action)
                {
                    case InventoryAction.Discard:
                        session.Player.Inventory.DiscardItem(inventoryAction.Source, inventoryAction.SourceSlot);
                        break;
                    case InventoryAction.Move:
                        session.Player.Inventory.MoveItem(inventoryAction.Source, inventoryAction.SourceSlot, inventoryAction.Destination, inventoryAction.DestinationSlot);
                        break;
                    case InventoryAction.Swap:
                        session.Player.Inventory.SwapItems(inventoryAction.Source, inventoryAction.SourceSlot, inventoryAction.Destination, inventoryAction.DestinationSlot);
                        break;
                    #if DEBUG
                    default:
                        Console.WriteLine($"Unhandled inventory action {inventoryAction.Action}!");
                        break;
                    #endif
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Player '{session.Player.Character.Name}' triggered inventory exception '{exception.Message}'");
            }
        }
    }
}
