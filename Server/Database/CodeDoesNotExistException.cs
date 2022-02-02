using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationManagerLibrary;

namespace VacationManagerServer.Database
{
    internal class CodeDoesNotExistException : Exception
    {
        public VacationEvent.Code Code { get; set; }
        private static string defaultMessage = " doesn\'t exist";
        public CodeDoesNotExistException(VacationEvent.Code code) : base(code + defaultMessage) => Code = code;
    }
}
