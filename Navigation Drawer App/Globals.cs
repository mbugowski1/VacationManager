using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Drawer_App
{
    public static class Globals
    {
        public static string IPaddress { get; set; } = "127.0.0.1";
        public static ushort Port { get; set; } = 1337;
        public static Network Connection { get; set; }
        public static string Username { get; set; }
        public static string Firstname { get; set; }
        public static string Lastname { get; set; }
        public static string Position { get; set; }
        public static int FreeDays { get; set; }
    }
}
