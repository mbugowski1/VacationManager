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
            switch (message.Operation)
            {
                case Test:
                    SendMessage(client, Test, "test echo");
                    break;
                    /*case CheckPass:
                        SendMessage<bool>(client, Test, RCheckPassword(client, (byte[])args[0], (byte[])args[1]));
                        break;
                    case AddUser:
                       SendMessage<bool>(client, AddUser, RAddUser(client, (byte[])args[0], (byte[])args[1], (byte[])args[2], (byte[])args[3], (byte[])args[4]));
                        break;
                    case GetData:
                        if (_clients[client] == String.Empty)
                            throw new UnauthorizedAccessException();
                        SendMessage<Person>(client, GetData, DatabaseConnection.GetData(_clients[client]));
                        break;
                    case AddEvent:
                        if (_clients[client] == String.Empty)
                            throw new UnauthorizedAccessException();
                        SendMessage<bool>(client, AddEvent, RAddEvent(client, (byte[])args[0], (DateTime)args[1], (DateTime)args[2], (VacationEvent.Code)args[3], (VacationEvent.Type)args[4]));
                        break;
                    case ChangeEventCode:
                        if (_clients[client] == String.Empty)
                            throw new UnauthorizedAccessException();
                        SendMessage<bool>(client, ChangeEventCode, DatabaseConnection.ChangeEventCode((int)args[0], (int)args[1]));
                        break;
                    case NewSupervisor:
                        if (_clients[client] == String.Empty)
                            throw new UnauthorizedAccessException();
                        SendMessage<bool>(client, NewSupervisor, DatabaseConnection.AddSupervisor(Encoding.UTF8.GetString((byte[])args[0]), Encoding.UTF8.GetString((byte[])args[1])));
                        break;
                    case RemSupervisor:
                        if (_clients[client] == String.Empty)
                            throw new UnauthorizedAccessException();
                        SendMessage<bool>(client, NewSupervisor, DatabaseConnection.RemoveSupervisor(Encoding.UTF8.GetString((byte[])args[0]), Encoding.UTF8.GetString((byte[])args[1])));
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
                        break;*/
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
        private bool RAddEvent(Socket client, byte[] recipient, DateTime start, DateTime stop, VacationEvent.Code code, VacationEvent.Type type)
        {
            VacationEvent newEvent = new VacationEvent(_clients[client], Encoding.UTF8.GetString(recipient), start, stop, code, type);
            return DatabaseConnection.SendEvent(newEvent);
        }
    }
}
