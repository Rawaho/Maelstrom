using System.Collections.Generic;
using System.IO;
using Shared.Database.Authentication;
using Shared.Network;

namespace LobbyServer.Network.Message
{
    [SubPacket(SubPacketOpcode.ServerServiceAccountList, SubPacketDirection.Server)]
    public class ServerServiceAccountList : SubPacket
    {
        public const byte MaxServiceAccounts = 8;

        public ulong Sequence;
        public List<ServiceAccountInfo> ServiceAccounts;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Sequence);
            writer.Pad(1u);
            writer.Write((byte)ServiceAccounts.Count);
            writer.Write((byte)3);
            writer.Write((byte)0x99);
            writer.Pad(4u);

            for (int i = 0; i < MaxServiceAccounts; i++)
            {
                if (i < ServiceAccounts.Count)
                {
                    ServiceAccountInfo serviceAccount = ServiceAccounts[i];
                    writer.Write(serviceAccount.Id);
                    writer.Write(0u);
                    writer.Write(i);
                    writer.WriteStringLength(serviceAccount.Name, 0x44u);
                }
                else
                    writer.Pad(0x50u);
            }
        }
    }
}
