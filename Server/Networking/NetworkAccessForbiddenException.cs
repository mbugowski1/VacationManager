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
        public Socket Client { get; set; } = null;
        private static string defaultMessage = "User wasn\'t logged in";
        public NetworkAccessForbiddenException(Socket client) : base(defaultMessage)
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
