using System.Text;
using System.Text.Json;
using System.Linq;

namespace VacationManagerLibrary
{
    public class Serializer
    {
        public static byte[] Serialize(string[] message)
        {
            byte[][] text = new byte[message.Length][];
            for(int i = 0; i < message.Length; i++)
                text[i] = Encoding.UTF8.GetBytes(message[i]);
            using var memStream = new MemoryStream();
            JsonSerializer.Serialize(memStream, text);
            return memStream.ToArray();
        }
        public static byte[] Serialize(string message)
        {
            byte[] text = Encoding.UTF8.GetBytes(message);
            using var memStream = new MemoryStream();
            JsonSerializer.Serialize(memStream, text);
            return memStream.ToArray();
        }
        public static byte[] Serialize<T>(T message)
        {
            using var memStream = new MemoryStream();
            JsonSerializer.Serialize(memStream, message);
            return memStream.ToArray();
        }
        public static T Deserialize<T>(byte[] message)
        {
            using (var memStream = new MemoryStream(message))
                return JsonSerializer.Deserialize<T>(memStream);
        }
    }
}