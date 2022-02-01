using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerLibrary
{
    public class Message : EventArgs
    {
        public enum Code
        {
            AddUser,
            CheckPass,
            GetData,
            AddEvent,
            ChangeEventCode,
            NewSupervisor,
            RemSupervisor,
            GetEventsToMe,
            GetEventsFromMe,
            GetMySupervisors,
            GetMyWorkers,
            Test
        }
        public Code Operation { get; set; }
        public object Data { set; get; }

        public Message(object data, Code code)
        {
            Operation = code;
            Data = data;
        }
    }
}
