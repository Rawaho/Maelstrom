using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Network
{
    public static class NetworkManager
    {
        public static volatile bool Shutdown;

        private static readonly HashSet<Session> sessions = new HashSet<Session>();
        private static readonly ReaderWriterLockSlim mutex = new ReaderWriterLockSlim();

        private static readonly ConcurrentQueue<(Connection connection, ConnectionChannel channel)> pendingAdd = new ConcurrentQueue<(Connection connection, ConnectionChannel channel)>();
        private static readonly ConcurrentQueue<Session> pendingRemove = new ConcurrentQueue<Session>();

        private static ReadOnlyDictionary<ConnectionChannel, Type> sessionChannels;

        public static void Initialise(int port)
        {
            InitialiseSessions();

            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"Listening for connections on port {port}...");

            new Thread(() =>
            {
                while (!Shutdown)
                {
                    Thread.Sleep(1);
                    if (!listener.Pending())
                        continue;

                    Task.Run(() => AcceptConnection(listener.AcceptSocket()));
                }
            }
            ).Start();
        }
        
        private static void InitialiseSessions()
        {
            var channelTypes = new Dictionary<ConnectionChannel, Type>();
            foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
                foreach (SessionAttribute attribute in type.GetCustomAttributes<SessionAttribute>())
                    channelTypes.Add(attribute.Channel, type);

            sessionChannels = new ReadOnlyDictionary<ConnectionChannel, Type>(channelTypes);
        }

        private static void AcceptConnection(Socket socket)
        {
            var connection = new Connection(socket);
            if (!FloodManager.CanAccept(connection.Remote))
            {
                connection.Close();
                return;
            }
            
            FloodManager.Accept(connection.Remote);
            
            // client has 10 seconds to send ClientHello otherwise connection will be terminated
            connection.ReceiveTimeout = 10_000; 
            connection.Receive();
            
            if (connection.ReceiveLength < Packet.Header.Length)
            {
                connection.Close();
                return;
            }

            connection.ReceiveTimeout = 0;

            // the rest of the packet is parsed in the session once created, for now we just need to know channel
            Packet.Header header = connection.Buffer.Copy(0, Packet.Header.Length).UnMarshal<Packet.Header>();
            if (!sessionChannels.ContainsKey(header.Channel))
            {
                connection.Close();
                return;
            }

            pendingAdd.Enqueue((connection, header.Channel));
        }

        // not thread safe, must handle mutex
        private static void _AddConnection(Connection connection, ConnectionChannel channel)
        {
            if (!sessionChannels.TryGetValue(channel, out Type sessionType))
                return;

            Session session = (Session)Activator.CreateInstance(sessionType);
            session.Accept(connection, channel);
            sessions.Add(session);
        }
        
        public static void RemoveSession(Session session)
        {
            pendingRemove.Enqueue(session);
        }

        // not thread safe, must handle mutex
        private static void _RemoveSession(Session session)
        {
            sessions.Remove(session);
        }

        public static void Update(double lastTick)
        {
            if (pendingAdd.Count > 0 || pendingRemove.Count > 0)
            {
                try
                {
                    mutex.EnterWriteLock();

                    while (pendingAdd.Count > 0)
                        if (pendingAdd.TryDequeue(out (Connection connection, ConnectionChannel channel) pending))
                            _AddConnection(pending.connection, pending.channel);
                    
                    while (pendingRemove.Count > 0)
                        if (pendingRemove.TryDequeue(out Session session))
                            _RemoveSession(session);
                }
                finally
                {
                    mutex.ExitWriteLock();
                }
            }

            try
            {
                mutex.EnterReadLock();
                foreach (Session session in sessions)
                    session.Update(lastTick);
            }
            finally
            {
                mutex.ExitReadLock();
            }
        }
    }
}
