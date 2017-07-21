using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Shared.Cryptography;
using System.Net;
using System.Net.Sockets;
using Shared.Network.Message;

namespace Shared.Network
{
    public abstract class Session
    {
        private const uint MaxIncomingPacketsPerUpdate   = 5u;
        private const uint MaxOutgoingSubPacketsPerFlush = 20u;
        
        public ConnectionChannel Channel { get; private set; }
        public IPAddress Local => connection.Local;
        public IPAddress Remote => connection.Remote;
        public ConnectionHeartbeat Heartbeat { get; } = new ConnectionHeartbeat();
        
        protected Blowfish blowfish;
        protected Connection connection;

        private bool pendingDisconnect;
        private readonly ConcurrentQueue<Packet> incomingPackets = new ConcurrentQueue<Packet>();
        private readonly ConcurrentQueue<PendingSubPacket> outgoingPackets = new ConcurrentQueue<PendingSubPacket>();

        public void Accept(Connection newConnection, ConnectionChannel channel)
        {
            connection = newConnection;
            Channel    = channel;
            
            // call this directly to finish processing the first packet
            ReceiveCallback();
        }

        private void ReceiveCallback()
        {
            if (pendingDisconnect)
                return;
            
            if (connection.ReceiveLength <= 0)
            {
                Disconnect();
                return;
            }

            byte[] payload = connection.Buffer.Copy(0, connection.ReceiveLength);
            
            // TODO: fragmented packets
            var packet = new Packet();
            if (packet.Process(payload, blowfish) != PacketResult.Ok)
            {
                Disconnect();
                return;
            }

            incomingPackets.Enqueue(packet);

            try
            {
                connection.BeginReceive(ReceiveCallback);
            }
            catch (SocketException exception)
            {
                #if DEBUG
                    Console.WriteLine(exception.Message);
                #endif
                Disconnect();
            }
        }

        public abstract void Send(SubPacket subPacket);

        public void Send(uint source, uint target, SubPacket subPacket)
        {
            outgoingPackets.Enqueue(new PendingSubPacket(subPacket, source, target));
        }

        public virtual void Disconnect()
        {
            pendingDisconnect = true;
            
            #if DEBUG
                Console.WriteLine($"Disconnected: {Remote}");
            #endif
            
            connection.Close();
            outgoingPackets.Clear();
            incomingPackets.Clear();
            
            NetworkManager.RemoveSession(this);
        }
        
        public void FlushPacketQueue()
        {
            if (outgoingPackets.Count == 0 || pendingDisconnect)
                return;

            try
            {
                IEnumerable<PendingSubPacket> outgoingSubPackets = outgoingPackets.DequeueMultiple(MaxOutgoingSubPacketsPerFlush);
                connection.Send(Packet.Build(blowfish, outgoingSubPackets));
            }
            catch (SocketException exception)
            {
                #if DEBUG
                    Console.WriteLine(exception.Message);
                #endif
                Disconnect();
            }
        }

        public virtual void Update(double lastTick)
        {
            foreach (Packet packet in incomingPackets.DequeueMultiple(MaxIncomingPacketsPerUpdate))
                foreach (SubPacket subPacket in packet.SubPackets)
                    PacketManager.InvokeHandler(this, subPacket);

            (ConnectionHeartbeatResult result, uint pulseTime) = Heartbeat.Update(lastTick);
            switch (result)
            {
                case ConnectionHeartbeatResult.Pulse:
                {
                    Send(0u, 0u, new KeepAliveRequest
                    {
                        Check     = (uint)Guid.NewGuid().GetHashCode(),
                        Timestamp = pulseTime
                    });
                    break;
                }
                case ConnectionHeartbeatResult.Flatline:
                    Disconnect();
                    break;
            }
            
            FlushPacketQueue();
        }
    }
}
