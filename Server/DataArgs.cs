using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DataArgs : EventArgs
    {
        public string Username { get; set; }
        public string Data { get; set; }
        public DataArgs(string username, string data)
        {
            Username = username;
            Data = data;
        }
    }
}
