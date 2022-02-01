using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer.Database
{
    public class VacationEvent
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public int Code { get; set; }
        public string? Desc { get; set; }
        public VacationEvent(string sender, string recipient, DateTime start, DateTime stop, int code)
        {
            Sender = sender;
            Recipient = recipient;
            Start = start;
            Stop = stop;
            Code = code;
        }
        public VacationEvent(string sender, string recipient, DateTime start, DateTime stop, int code, string desc)
        {
            Sender = sender;
            Recipient = recipient;
            Start = start;
            Stop = stop;
            Code = code;
            Desc = desc;
        }
        public override string ToString()
        {
            return $"{ Sender } -> { Recipient } ({ Start.ToShortDateString() } { Stop.ToShortDateString() }): ({ Code }) { Desc }";
        }
    }
}
