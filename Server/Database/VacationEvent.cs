using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerServer.Database
{
    public class VacationEvent
    {
        public int? ID { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public int Code { get; set; }
        public string? CodeDesc { get; set; }
        public int Type{ get; set; }
        public string? TypeDesc { get; set; }
        public VacationEvent(string sender, string recipient, DateTime start, DateTime stop, int code, int type, int? id = null, string? codedesc = null, string? typedesc = null)
        {
            ID = id;
            Sender = sender;
            Recipient = recipient;
            Start = start;
            Stop = stop;
            Code = code;
            Type = type;
            CodeDesc = codedesc;
            TypeDesc = typedesc;
        }
        public override string ToString()
        {
            return $"{ Sender } -> { Recipient } ({ Start.ToShortDateString() } { Stop.ToShortDateString() }): ({ Code }){ CodeDesc } || ({Type}){TypeDesc}";
        }
    }
}
