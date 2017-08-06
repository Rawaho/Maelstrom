using System;
using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ClientContentFinderRequestInfo, SubPacketDirection.Client)]
    public class ClientContentFinderRequestInfo : SubPacket
    {
        public override void Read(BinaryReader reader)
        {
            // TODO, 8bytes
        }
    }
}
