using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer
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
            dataReceived += DataReceived;
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
            DataArgs data = new(_clients[client], recText);
            dataReceived?.Invoke(client, data);
            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
            //client.Close();
            //_clients.Remove(client);
        }
        private void DataReceived(object source, DataArgs args)
        {
            Socket client = (Socket)source;
            string[] arguments = args.Data.Split(' ');
            if (arguments.Length == 0)
                throw new WrongDataException();
            else
            {
                switch(arguments[0])
                {
                    case "test":
                        SendMessage(client, "test echo");
                        break;
                    case "add":
                        if (arguments.Length != 3)
                            throw new WrongDataException("Expected 3 arguments - got " + arguments.Length);
                        Database.DatabaseConnection.CreateUser(arguments[1], Encoding.UTF8.GetBytes(arguments[2]));
                        break;
                    case "check":
                        if (arguments.Length != 3)
                            throw new WrongDataException("Expected 3 arguments - got " + arguments.Length);
                        bool success = Database.DatabaseConnection.CheckPassword(arguments[1], Encoding.UTF8.GetBytes(arguments[2])).Result;
                        SendMessage(client, success ? "correct" : "incorrect");
                        break;
                }
            }
        }

        public void SendMessage(Socket client, string message)
        {
            if (client == null) throw new SocketException();
            byte[] text = Encoding.UTF8.GetBytes(message);
            DataArgs data = new DataArgs(_clients[client], message);
            dataSent?.Invoke(client, data);
            client.Send(text);
        }
    }
}
