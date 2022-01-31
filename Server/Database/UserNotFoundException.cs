using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer.Database
{
    public class UserNotFoundException : Exception
    {
        public String Username { get; set; }
        public UserNotFoundException() => Username = String.Empty;
        public UserNotFoundException(string username) => Username = username;
        public UserNotFoundException(string username, string message) : base(message) => Username = username;
    }
}
