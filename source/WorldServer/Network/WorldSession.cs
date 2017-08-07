using System;
using Shared.Network;
using WorldServer.Game.Entity;

namespace WorldServer.Network
{
    [Session(ConnectionChannel.World)]
    public class WorldSession : Session
    {
        public Player Player { get; set; }

        protected override bool CanProcessSubPacket(SubPacket subPacket)
        {
            SubPacketHandlerAttribute attribute = PacketManager.GetSubPacketHandlerInfo(subPacket);
            if (attribute == null)
                return true;

            if ((attribute.Flags & SubPacketHandlerFlags.RequiresPlayer) != 0 && Player == null)
            {
                #if DEBUG
                    Console.WriteLine($"Rejecting packet ({subPacket.SubHeader.Type}, {subPacket.SubMessageHeader.Opcode}), world session ({Remote}) doesn't have an assigned character!");
                #endif
                return false;
            }

            if ((attribute.Flags & SubPacketHandlerFlags.RequiresWorld) != 0 && (Player == null || !Player.InWorld))
            {
                #if DEBUG
                    Console.WriteLine($"Rejecting packet ({subPacket.SubHeader.Type}, {subPacket.SubMessageHeader.Opcode}), world session ({Remote}) character isn't in the world!");
                #endif
                return false;
            }

            return true;
        }

        public override void Send(SubPacket subPacket)
        {
            uint actorId = Player?.Character.ActorId ?? 0u;
            Send(actorId, actorId, subPacket);
        }

        public override void Disconnect()
        {
            base.Disconnect();
            Player?.RemoveFromMap();
        }
    }
}
