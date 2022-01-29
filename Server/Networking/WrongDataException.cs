using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer
{
    public class WrongDataException : Exception
    {
        public Socket? Client { get; set; } = null;
        public WrongDataException() { }
        public WrongDataException(string message) : base(message) { }
        public WrongDataException(Socket client) => Client = client;
        public WrongDataException(Socket client, string message) : base(message) => Client = client;
    }
}
