using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer
{
    public class DataArgs : EventArgs
    {
        public string Username { get; set; }
        public byte[] Data { get; set; }
        public DataArgs(string username, byte[] data)
        {
            Username = username;
            Data = data;
        }
    }
}
