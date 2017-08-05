using System;
using System.Collections;
using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerContentFinderList, SubPacketDirection.Server)]
    public class ServerContentFinderList : SubPacket
    {
        public BitArray Contents = new BitArray(0x48 * 8, false);
        
        public override void Write(BinaryWriter writer)
        {
            var contentsLength = (Contents.Length - 1) / 8 + 1;
            
            var data = new byte[Math.Max(contentsLength, 0x48)];
            Contents.CopyTo(data, 0);
            
            writer.Write(data);
        }
    }
}
 