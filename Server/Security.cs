using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer
{
    public static class Security
    {
        public static bool Auth(string username, byte[] password)
        {
            return true;
        }
    }
}
