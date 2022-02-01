using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerLibrary
{
    public class Person
    {
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Position { get; set; }
        public Person(string username, string firstname, string lastname, string position)
        {
            Username = username;
            Firstname = firstname;
            Lastname = lastname;
            Position = position;
        }
        public override string ToString()
        {
            return $"{ Username }: { Firstname } { Lastname } - { Position }";
        }
    }
}
