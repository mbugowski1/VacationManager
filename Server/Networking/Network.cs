using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VacationManagerLibrary;

namespace VacationManagerServer
{
    public partial class Network
    {
        public EventHandler<Message>? dataReceived;
        public EventHandler<Message>? dataSent;

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
            ArraySegment<byte> sending = new(_buffer, 0, received);
            Message data = Serializer.Deserialize<Message>(sending.ToArray());
            dataReceived?.Invoke(client, data);
            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
        }
        public void SendMessage(Socket client, Message.Code code, string message)
        {
            byte[] text = Encoding.UTF8.GetBytes(message);
            SendMessage(client, code, text);
        }
        public void SendMessage<T>(Socket client, Message.Code code, T message)
        {
            Message data = new Message();
            data.Data = Serializer.Serialize(message);
            data.Operation = code;
            dataSent?.Invoke(client, data);
            client.Send(Serializer.Serialize(data));
        }
    }
}
