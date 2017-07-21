using System;
using System.Net;
using System.Net.Sockets;

namespace Shared.Network
{
    public class Connection
    {
        public IPAddress Local => ((IPEndPoint)socket.LocalEndPoint).Address;
        public IPAddress Remote => ((IPEndPoint)socket.RemoteEndPoint).Address;
        public byte[] Buffer { get; } = new byte[0x1000];
        public int ReceiveLength { get; private set; }

        public int ReceiveTimeout
        {
            get => socket.ReceiveTimeout;
            set => socket.ReceiveTimeout = value;
        }
        
        private readonly Socket socket;
        private Action receiveCallback;

        public Connection(Socket socket)
        {
            this.socket = socket;
        }

        public void Close()
        {
            socket.Close();
        }

        public void Receive()
        {
            try
            {
                ReceiveLength = socket.Receive(Buffer, 0, Buffer.Length, SocketFlags.None);
            }
            catch (SocketException exception)
            {
                #if DEBUG
                    Console.WriteLine(exception.Message);
                #endif
                throw;
            }
        }

        public void BeginReceive(Action callback)
        {
            receiveCallback = callback;
            socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, BeginReceiveCallback, null);
        }

        private void BeginReceiveCallback(IAsyncResult result)
        {
            ReceiveLength = socket.EndReceive(result);
            receiveCallback();
        }

        public void Send(byte[] buffer)
        {
            socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
    }
}
