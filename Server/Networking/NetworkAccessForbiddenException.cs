using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer
{
    public class NetworkAccessForbiddenException : Exception
    {
        public Socket? Client { get; set; } = null;
        public NetworkAccessForbiddenException() { }
        public NetworkAccessForbiddenException(string message)
            : base(message) { }
        public NetworkAccessForbiddenException(Socket client)
        {
            Client = client;
        }
        public NetworkAccessForbiddenException(Socket client, string message)
            : base(message)
        {
            Client = client;
        }
    }
}
