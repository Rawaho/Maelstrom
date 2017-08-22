using System.IO;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientEventSceneFinish)]
    public class ClientEventSceneFinish : SubPacket
    {
        public uint EventId { get; private set; }
        public ushort SceneId { get; private set; }

        public override void Read(BinaryReader reader)
        {
            EventId = reader.ReadUInt32();
            SceneId = reader.ReadUInt16();
        }
    }
}
