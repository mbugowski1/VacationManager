using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer.Database
{
    internal class EventDoesNotExist : Exception
    {
        public int EventID { get; set; }
        public EventDoesNotExist(int id) : base("Event " + id + "doesn\'t exist") => EventID = id;
    }
}
