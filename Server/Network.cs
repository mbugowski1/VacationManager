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
        public EventHandler<DataArgs>? dataReceived;
        public EventHandler<DataArgs>? dataSent;

        private readonly Dictionary<Socket, string> _clients = new();
        private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly byte[] _buffer = new byte[2048];
        private readonly int port;
        public Network(int port) => this.port = port;
        public void Open()
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(5);
            _socket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        private void AcceptCallback(IAsyncResult AR)
        {
            Socket client = _socket.EndAccept(AR);
            _clients.Add(client, String.Empty);
            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
            Console.WriteLine("Client connected");
            _socket.BeginAccept(AcceptCallback, null);
        }
        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket? client = (Socket?)AR.AsyncState;
            if (client == null) throw new SocketException();
            int received;
            try
            {
                received = client.EndReceive(AR);
            }
            catch(SocketException)
            {
                Console.WriteLine("Client disconnected");
                _clients.Remove(client);
                return;
            }
            string recText = Encoding.UTF8.GetString(_buffer, 0, received);
            DataArgs data = new("Unknown", recText);
            dataReceived?.Invoke(client, data);
            NetworkActions(AR, recText);
            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
            client.Close();
            _clients.Remove(client);
        }
        public void SendMessage(IAsyncResult AR, string message)
        {
            Socket? client = (Socket?)AR.AsyncState;
            if (client == null) throw new SocketException();
            byte[] text = Encoding.UTF8.GetBytes(message);
            DataArgs data = new DataArgs("Unknown", message);
            dataSent?.Invoke(client, data);
            client.Send(text);
        }
        private void NetworkActions(IAsyncResult AR, string message)
        {
            switch(message)
            {
                case "test":
                    SendMessage(AR, "test echo");
                    break;
            }
        }
    }
}
