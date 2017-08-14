using System.Collections;
using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerPlayerStateFlags, SubPacketDirection.Server)]
    public class ServerPlayerStateFlags : SubPacket
    {
        public BitArray StateMask { get; } = new BitArray(8 * 8);

        public override void Write(BinaryWriter writer)
        {
            writer.Pad(2u);
            writer.Write(StateMask.ToArray());
            writer.Pad(6u);
        }
    }
}
