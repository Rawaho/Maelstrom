using System;
using System.Collections.Generic;
using LobbyServer.Network.Message;
using Shared.Cryptography;
using Shared.Database.Authentication;
using Shared.Database.Datacentre;
using Shared.Network;

namespace LobbyServer.Network
{
    [Session(ConnectionChannel.Lobby)]
    public class LobbySession : Session
    {
        public uint SessionHash { get; set; }
        
        public ulong Sequence { get; set; }
        public Token AuthToken { get; set; }
        public ServiceAccountInfo ServiceAccount { get; set; }
        public List<ServiceAccountInfo> ServiceAccounts { get; set; }

        public (ushort RealmId, string Name) CharacterCreate { get; set; }
        public List<CharacterInfo> Characters { get; set; }

        public void CalculateNetworkKey(uint time, byte[] seed)
        {
            byte[] key = Blowfish.CalculateNetworkKey(time, seed);
            blowfish = new Blowfish(key);

            #if DEBUG
                Console.WriteLine($"Calculated Blowfish Key: {BitConverter.ToString(key).Replace("-", "")}");
            #endif
        }

        protected override bool CanProcessSubPacket(SubPacket subPacket)
        {
            SubPacketHandlerAttribute attribute = PacketManager.GetSubPacketHandlerInfo(subPacket);
            if (attribute == null)
                return true;

            if ((attribute.Flags & SubPacketHandlerFlags.RequiresEncryption) != 0 && blowfish == null)
            {
                #if DEBUG
                    Console.WriteLine($"Rejecting packet ({subPacket.SubHeader.Type}, {subPacket.SubMessageHeader.Opcode}), lobby session ({Remote}) doesn't have Blowfish enabled!");
                #endif
                return false;
            }

            if ((attribute.Flags & SubPacketHandlerFlags.RequiresAccount) != 0 && ServiceAccount.Id == 0)
            {
                #if DEBUG
                    Console.WriteLine($"Rejecting packet ({subPacket.SubHeader.Type}, {subPacket.SubMessageHeader.Opcode}), lobby session ({Remote}) doesn't have assigned service account!");
                #endif
                return false;
            }

            return true;
        }

        public override void Send(SubPacket subPacket)
        {
            uint sessionHash = SessionHash != 0u ? SessionHash : 0u;
            Send(sessionHash, sessionHash, subPacket);
        }

        public void SendError(uint errorId, ushort errorExdId, uint value = 0u)
        {
            Send(new ServerError
            {
                ErrorId    = errorId,
                ExdErrorId = errorExdId,
                Value      = value,
                Sequence   = Sequence 
            });
        }
    }
}
