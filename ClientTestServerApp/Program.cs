using System;
using System.Text;
using VacationManagerBackend;
using VacationManagerLibrary;
using static VacationManagerLibrary.Message.Code;
namespace ClientTestServerApp
{
    public class Program
    {
        public static Network network;
        public static void Main()
        {
            network = new Network("127.0.0.1", 1337);
            Console.WriteLine("0. Wyjdz");
            if (network.Connect())
                Console.WriteLine("Polaczono");
            string message;
            while ((message = Console.ReadLine()) != "0")
            {
                while (message == String.Empty)
                {
                    Console.WriteLine("Napisz wiadomosc");
                    message = Console.ReadLine();
                }
                Control(message);
                Console.WriteLine("Wyslano");
            }

        }
        private static void Control(string text)
        {
            string[] args = text.Split(' ');
            string commandCase = args[0];
            Message.Code code;
            Message message = new Message();
            byte[] messageToSend;
            switch (commandCase)
            {
                case "test":
                    message.Operation = Message.Code.Test;
                    messageToSend = Serializer.Serialize(message);
                    network.SendMessage(messageToSend);
                    break;
            }
        }
        public class TestClass
        {
            public int Prop { get; set; }
        }
    }
}