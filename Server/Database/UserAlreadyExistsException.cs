using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer.Database
{
    public class UserAlreadyExistsException : Exception
    {
        public string Username { get; set; }
        private static string message = "User already exists";
        public UserAlreadyExistsException() : base(message) => Username = String.Empty;
        public UserAlreadyExistsException(string username) : base(message) => Username = username;
    }
}
