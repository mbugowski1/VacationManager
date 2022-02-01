using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer.Database
{
    internal class CodeDoesNotExistException : Exception
    {
        public int Code { get; set; }
        private static string defaultMessage = " doesn\'t exist";
        public CodeDoesNotExistException(int code) : base(code + defaultMessage) => Code = code;
    }
}
