using System.IO;
using System.Numerics;
using Shared.Network;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketClientOpcode.ClientPlayerMove, false)]
    public class ClientPlayerMove : SubPacket
    {
        public float Orientation { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Position2 { get; private set; }  // always seems to mirror the first position

        public override void Read(BinaryReader reader)
        {
            Orientation = reader.ReadSingle();
            reader.ReadUInt32();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();

            Position  = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.ReadUInt32();
            Position2 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }
}
