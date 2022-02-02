using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationManagerLibrary;

namespace Navigation_Drawer_App
{
    internal static class Quick
    {
        public static void RegisterUser(string username, string password, string firstname, string lastname, string position, string supervisor)
        {
            var message = new Message();
            message.Operation = Message.Code.AddUser;
            string[] data = new string[] { username, password, firstname, lastname, position };
            message.Data = Serializer.Serialize(data);
            Globals.Connection.SendMessage(Serializer.Serialize(message));
            data = new string[] { username, supervisor };
            message.Operation = Message.Code.NewSupervisor;
            message.Data = Serializer.Serialize(data);
            Globals.Connection.SendMessage(Serializer.Serialize(message));
        }
    }
}
