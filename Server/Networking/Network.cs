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
            ArraySegment<byte> sending = new(_buffer, 0, received);
            DataArgs data = new(_clients[client], sending.ToArray());
            dataReceived?.Invoke(client, data);
            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
        }
        private void DataReceived(object source, DataArgs args)
        {
            Socket client = (Socket)source;
            byte[][] arguments = SeperateStream(args.Data, ' ');
            string command = Encoding.UTF8.GetString(arguments[0]);
            string username;
            Database.Person person;
            if (arguments.Length == 0)
                throw new WrongDataException();
            else
            {
                switch(command)
                {
                    case "test":
                        SendMessage(client, "test echo");
                        break;
                    case "add":
                        if (arguments.Length != 5 && arguments.Length != 6)
                            throw new WrongDataException("Expected 3 arguments - got " + arguments.Length);
                        username = Encoding.UTF8.GetString(arguments[1]);
                        person = new Database.Person(username, Encoding.UTF8.GetString(arguments[3]),
                                                                    Encoding.UTF8.GetString(arguments[4]));
                        if (arguments.Length == 5)
                            person.Position = "worker";
                        else
                            person.Position = Encoding.UTF8.GetString(arguments[5]);
                        Security.AddUser(person, arguments[2]);
                        _clients[client] = username;
                        break;
                    case "check":
                        if (arguments.Length != 3)
                            throw new WrongDataException("Expected 3 arguments - got " + arguments.Length);
                        if (_clients[client] != String.Empty)
                            throw new NetworkAccessForbiddenException(client, "User was already logged");
                        username = Encoding.UTF8.GetString(arguments[1]);
                        bool success = Database.DatabaseConnection.CheckPassword(username, arguments[2]);
                        if (success)
                            _clients[client] = username;
                        SendMessage(client, success ? "correct" : "incorrect");
                        break;
                    case "get":
                        if (arguments.Length != 1)
                            throw new WrongDataException("Expected 1 argument - got " + arguments.Length);
                        if (_clients[client] == String.Empty)
                            throw new NetworkAccessForbiddenException(client);
                        person = Database.DatabaseConnection.GetData(_clients[client]);
                        SendMessage(client, person.ToString());
                        break;
                    case "addevent":
                        if (arguments.Length != 5)
                            throw new WrongDataException("Expected 6 arguments - got " + arguments.Length);
                        if (_clients[client] == String.Empty)
                            throw new NetworkAccessForbiddenException(client);
                        Database.VacationEvent vacationEvent = new Database.VacationEvent
                            (
                                _clients[client],
                                Encoding.UTF8.GetString(arguments[1]),
                                DateTime.Parse(Encoding.UTF8.GetString(arguments[2])),
                                DateTime.Parse(Encoding.UTF8.GetString(arguments[3])),
                                Int32.Parse(Encoding.UTF8.GetString(arguments[4]))
                            );
                        Database.DatabaseConnection.SendEvent(vacationEvent);
                        break;
                    case "changeevent":
                        if (arguments.Length != 3)
                            throw new WrongDataException("Expected 3 arguments - got " + arguments.Length);
                        if (_clients[client] == String.Empty)
                            throw new NetworkAccessForbiddenException(client);
                        Database.DatabaseConnection.ChangeEventCode(Int32.Parse(Encoding.UTF8.GetString(arguments[1])), Int32.Parse(Encoding.UTF8.GetString(arguments[2])));
                        break;
                    case "addsuper": //usunac
                        if (arguments.Length != 3)
                            throw new WrongDataException("Expected 3 arguments - got " + arguments.Length);
                        if (_clients[client] == String.Empty)
                            throw new NetworkAccessForbiddenException(client);
                        Database.DatabaseConnection.AddSupervisor(Encoding.UTF8.GetString(arguments[1]), Encoding.UTF8.GetString(arguments[2]));
                        break;
                    case "remsuper": //usunac
                        if (arguments.Length != 3)
                            throw new WrongDataException("Expected 3 arguments - got " + arguments.Length);
                        if (_clients[client] == String.Empty)
                            throw new NetworkAccessForbiddenException(client);
                        Database.DatabaseConnection.RemoveSupervisor(Encoding.UTF8.GetString(arguments[1]), Encoding.UTF8.GetString(arguments[2]));
                        break;
                    case "toevents":
                        if (arguments.Length != 1)
                            throw new WrongDataException("Expected 1 argument - got " + arguments.Length);
                        if (_clients[client] == String.Empty)
                            throw new NetworkAccessForbiddenException(client);
                        Database.DatabaseConnection.GetEvents(_clients[client], true).ForEach(x => Console.WriteLine(x.ToString()));
                        break;
                    case "fromevents":
                        if (arguments.Length != 1)
                            throw new WrongDataException("Expected 1 argument - got " + arguments.Length);
                        if (_clients[client] == String.Empty)
                            throw new NetworkAccessForbiddenException(client);
                        Database.DatabaseConnection.GetEvents(_clients[client]).ForEach(x => Console.WriteLine(x.ToString()));
                        break;
                    case "exit":
                        if (arguments.Length != 1)
                            throw new WrongDataException("Expected 1 argument - got " + arguments.Length);
                        _clients[client] = String.Empty;
                        break;
                    default:
                        throw new WrongDataException();
                }
            }
        }

        public void SendMessage(Socket client, string message)
        {
            if (client == null) throw new SocketException();
            byte[] text = Encoding.UTF8.GetBytes(message);
            DataArgs data = new DataArgs(_clients[client], text);
            dataSent?.Invoke(client, data);
            client.Send(text);
        }
        private static byte[][] SeperateStream(byte[] source, char seperator)
        {
            List<byte[]> result = new();
            List<byte> array = new();
            foreach(var x in source)
            {
                if(x == seperator)
                {
                    result.Add(array.ToArray());
                    array.Clear();
                }
                else
                    array.Add(x);
            }
            result.Add(array.ToArray());
            return result.ToArray();
        }
    }
}
