using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerLibrary
{
    public class VacationEvent
    {
        public enum Code
        {
            Pending,
            Seen,
            Accepted,
            Refused
        }
        public enum Type
        {
            Normal,
            Demanded
        }

        public int? ID { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public Code CodeId { get; set; }
        public string? CodeDesc { get; set; }
        public Type TypeId{ get; set; }
        public string? TypeDesc { get; set; }
        public override string ToString()
        {
            return $"{ Sender } -> { Recipient } ({ Start.ToShortDateString() } { Stop.ToShortDateString() }): ({ CodeId.ToString() }){ CodeDesc } || ({TypeId.ToString()}){TypeDesc}";
        }
    }
}
