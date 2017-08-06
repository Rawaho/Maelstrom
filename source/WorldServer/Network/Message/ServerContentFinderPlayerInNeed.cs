using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Shared.Game.Enum;
using Shared.Network;

namespace WorldServer.Network.Message {
    [SubPacket(SubPacketOpcode.ServerContentFinderPlayerInNeed, SubPacketDirection.Server)]
    public class ServerContentFinderPlayerInNeed : SubPacket {
        public ClassJobRole[] InNeed = new ClassJobRole[0x10];
        
        public override void Write(BinaryWriter writer)
        {
            // Ordered by roulette id, max 16
            var data = new byte[0x10];
            Array.Copy(InNeed, 0, data, 0, Math.Min(InNeed.Length, 0x10));
            
            writer.Write(data);
        }
    }
}