using System.IO;
using Shared.Network;
using WorldServer.Game.Event;

namespace WorldServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerEventSceneStart, SubPacketDirection.Server)]
    public class ServerEventSceneStart : SubPacket
    {
        public Event Event;
        public SceneFlags Flags;
        public uint Unk1;
        public byte Unk2;
        public uint Unk3;
        public uint Unk4;
        public uint Unk5;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Event.ActorId);
            writer.Write(Event.Id);
            writer.Write(Event.ActiveScene.Id);
            writer.Pad(2u);
            writer.Write((uint)Flags);
            writer.Write(Unk1);
            writer.Write(Unk2);
            writer.Pad(3u);
            writer.Write(Unk3);
            writer.Write(Unk4);
            writer.Write(Unk5);
        }
    }
}
