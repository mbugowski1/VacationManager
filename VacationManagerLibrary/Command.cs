using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerLibrary
{
    public class Command
    {
        public object[] Args { get; set; }
        public Command(object[] args) => Args = args;
        private static byte[][] SeperateStream(byte[] source, char seperator)
        {
            List<byte[]> result = new();
            List<byte> array = new();
            foreach (var x in source)
            {
                if (x == seperator)
                {
                    result.Add(array.ToArray());
                    array.Clear();
                }
                else
                    array.Add(x);
            }
            result.Add(array.ToArray());
            return result.ToArray();
        }
    }
}
