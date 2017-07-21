using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Shared.Cryptography;

namespace Shared.Network
{
    public class SubPacket
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Header
        {
            public const uint Length = 0x10;

            public uint Size;
            public uint Source;
            public uint Target;
            public SubPacketType Type;
            public ushort Unknown2;

            public static Header UnMarshal(BinaryReader reader)
            {
                return reader.ReadBytes((int)Length).UnMarshal<Header>();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MessageHeader
        {
            public const uint Length = 0x10;

            public ushort Unknown1; // always 0x14
            public SubPacketOpcode Opcode;
            public uint Unknown2;
            public uint Timestamp;
            public uint Unknown4;

            public static MessageHeader UnMarshal(BinaryReader reader)
            {
                return reader.ReadBytes((int)Length).UnMarshal<MessageHeader>();
            }
        }

        public Header SubHeader { get; private set; } = new Header();
        public MessageHeader SubMessageHeader { get; private set; }

        public virtual void Read(BinaryReader reader) { }
        public virtual void Write(BinaryWriter writer) { }

        public void Initialise(Header header, MessageHeader messageHeader)
        {
            SubHeader        = header;
            SubMessageHeader = messageHeader;
        }

        public byte[] Build(Blowfish blowfish, uint source, uint target)
        {
            SubPacketAttribute attribute = (SubPacketAttribute)Attribute.GetCustomAttribute(GetType(), typeof(SubPacketAttribute));

            byte[] payload;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                    Write(writer);

                payload = stream.ToArray();
            }

            if (attribute.Opcode != SubPacketOpcode.None)
            {
                SubMessageHeader = new MessageHeader
                {
                    Unknown1  = 0x14,
                    Opcode    = attribute.Opcode,
                    Timestamp = (uint)DateTimeOffset.Now.ToUnixTimeSeconds()
                };

                payload = SubMessageHeader.Marshal().Concat(payload).ToArray();
            }

            blowfish?.Encipher(payload, 0, payload.Length);

            SubHeader = new Header
            {
                Size   = (ushort)(Header.Length + payload.Length),
                Type   = attribute.Opcode != SubPacketOpcode.None ? SubPacketType.Message : attribute.Type,
                Source = source,
                Target = target
            };

            return SubHeader.Marshal().Concat(payload).ToArray();
        }

        public static PacketResult Process(BinaryReader reader, Blowfish blowfish, out SubPacket subPacket)
        {
            subPacket = null;
            Header header = Header.UnMarshal(reader);

            // not enough data to cover length specified in header
            if (reader.BaseStream.Length - reader.BaseStream.Position < header.Size - Header.Length)
                return PacketResult.Malformed;

            byte[] payload = reader.ReadBytes((int)(header.Size - Header.Length));
            
            if (header.Type != SubPacketType.KeepAliveRequest && header.Type != SubPacketType.KeepAliveResponse)
                blowfish?.Decipher(payload, 0, payload.Length);

            using (var stream = new MemoryStream(payload))
            {
                using (var subReader = new BinaryReader(stream))
                {
                    MessageHeader messageHeader = new MessageHeader();
                    if (header.Type == SubPacketType.Message)
                    {
                        messageHeader = MessageHeader.UnMarshal(subReader);

                        Array.Copy(payload, MessageHeader.Length, payload, 0, payload.Length - MessageHeader.Length);
                        Array.Resize(ref payload, payload.Length - (int)MessageHeader.Length);
                        stream.Position = 0L;
                    }

                    subPacket = PacketManager.GetSubPacket(header.Type, messageHeader.Opcode, SubPacketDirection.Client) ?? new SubPacket();
                    subPacket.Initialise(header, messageHeader);
                    subPacket.Read(subReader);
                }
            }

            return PacketResult.Ok;
        }
    }
}
