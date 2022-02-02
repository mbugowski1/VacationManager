using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationManagerLibrary;
using static VacationManagerLibrary.Message.Code;
using VacationManagerServer.Database;
using System.Net.Sockets;

namespace VacationManagerServer
{
    public partial class Network
    {
        public void NetworkReaction(object source, Message message)
        {
            Socket client = (Socket)source;
            byte[][] byteArgs;
            int[] intArgs;
            string user;
            bool result;
            switch (message.Operation)
            {
                case Test:
                    SendMessage(client, Test, "test echo");
                    break;
                case CheckPass:
                    byteArgs = Serializer.Deserialize<byte[][]>(message.Data);
                    result = RCheckPassword(client, byteArgs[0], byteArgs[1]);
                    SendMessage<bool>(client, CheckPass, result);
                    break;
                case AddUser:
                    byteArgs = Serializer.Deserialize<byte[][]>(message.Data);
                    result = RAddUser(client, byteArgs[0], byteArgs[1], byteArgs[2], byteArgs[3], byteArgs[4]);
                    SendMessage<bool>(client, AddUser, result);
                    break;
                case GetData:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    user = Encoding.UTF8.GetString(Serializer.Deserialize<byte[]>(message.Data));
                    SendMessage<Person>(client, GetData, DatabaseConnection.GetData(user));
                    break;
                case AddEvent:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    SendMessage<bool>(client, AddEvent, DatabaseConnection.SendEvent(Serializer.Deserialize<VacationEvent>(message.Data)));
                    break;
                case ChangeEventCode:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    intArgs = Serializer.Deserialize<int[]>(message.Data);
                    SendMessage<bool>(client, ChangeEventCode, DatabaseConnection.ChangeEventCode(intArgs[0], (Message.Code)intArgs[1]));
                    break;
                case NewSupervisor:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    byteArgs = Serializer.Deserialize<byte[][]>(message.Data);
                    SendMessage<bool>(client, NewSupervisor, DatabaseConnection.AddSupervisor(Encoding.UTF8.GetString(byteArgs[0]), Encoding.UTF8.GetString(byteArgs[1])));
                    break;
                case RemSupervisor:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    byteArgs = Serializer.Deserialize<byte[][]>(message.Data);
                    SendMessage<bool>(client, NewSupervisor, DatabaseConnection.RemoveSupervisor(Encoding.UTF8.GetString(byteArgs[0]), Encoding.UTF8.GetString(byteArgs[1])));
                    break;
                case GetEventsToMe:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    SendMessage<List<VacationEvent>>(client, GetEventsToMe, DatabaseConnection.GetEvents(_clients[client], true));
                    break;
                case GetEventsFromMe:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    SendMessage<List<VacationEvent>>(client, GetEventsFromMe, DatabaseConnection.GetEvents(_clients[client]));
                    break;
                case GetMySupervisors:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    SendMessage<List<string>>(client, GetMySupervisors, DatabaseConnection.GetWorkers(_clients[client], true));
                    break;
                case GetMyWorkers:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    SendMessage<List<string>>(client, GetMyWorkers, DatabaseConnection.GetWorkers(_clients[client]));
                    break;
                case Exit:
                    if (_clients[client] == String.Empty)
                        throw new UnauthorizedAccessException();
                    _clients[client] = String.Empty;
                    SendMessage<bool>(client, Exit, true);
                    break;
            }
        }
        private bool RCheckPassword(Socket client, byte[] username, byte[] password)
        {
            string user = Encoding.UTF8.GetString(username);
            bool success = DatabaseConnection.CheckPassword(user, password);
            if (success)
                _clients[client] = user;
            return success;
        }
        private bool RAddUser(Socket client, byte[] username, byte[] password, byte[] firstname, byte[] lastname, byte[] position)
        {
            string user = Encoding.UTF8.GetString(username);
            Person person = new Person(user, Encoding.UTF8.GetString(firstname), Encoding.UTF8.GetString(lastname), Encoding.UTF8.GetString(position));
            bool success = DatabaseConnection.CreateUser(person, password);
            if (success)
                _clients[client] = user;
            return success;
        }
    }
}
