using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer.Database
{
    internal class UserDoesNotExistException : Exception
    {
        public string Username { get; set; }
        private static string defaultMessage = " doesn\'t exist";
        public UserDoesNotExistException(string username) : base(username + defaultMessage) => Username = username;
    }
}
