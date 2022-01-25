using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Network
    {
        public EventHandler dataReceived;
        public EventHandler dataSent;

        private readonly List<Socket> _clients = new();
        private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly byte[] _buffer = new byte[2048];
        private readonly int port;
        public Network(int port) => this.port = port;
        public void Open()
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(5);
            _socket.BeginAccept(AcceptCallback, null);
        }
        private void AcceptCallback(IAsyncResult AR)
        {
            Socket client = _socket.EndAccept(AR);
            _clients.Add(client);
            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, client);
            Debug.WriteLine("Client connected");
            _socket.BeginAccept(AcceptCallback, null);
        }
        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket? client = (Socket?)AR.AsyncState;
            if (client == null) return;
            int received;
            try
            {
                received = client.EndReceive(AR);
            }
            catch(SocketException)
            {
                Debug.WriteLine("Client disconnected");
                _clients.Remove(client);
                return;
            }
            string recText = Encoding.UTF8.GetString(_buffer, 0, received);
            DataArgs data = new("Unknown", recText);
            dataReceived.Invoke(client, data);
            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, client);
        }
        public void Close()
        {
            foreach (var client in _clients)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            _socket.Close();
        }
    }
}
