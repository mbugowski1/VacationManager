using System.Text;
using System.Text.Json;

namespace VacationManagerLibrary
{
    public class Serializer
    {
        public static byte[] Serialize<T>(T message)
        {
            using var memStream = new MemoryStream();
            JsonSerializer.Serialize(memStream, message);
            return memStream.ToArray();
        }
        public static Message Deserialize(byte[] message)
        {
            using (var memStream = new MemoryStream(message))
                return JsonSerializer.Deserialize<Message>(memStream);
        }
    }
}