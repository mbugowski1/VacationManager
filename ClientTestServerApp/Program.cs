using System;
using VacationManagerBackend;
namespace ClientTestServerApp
{
    public class Program
    {
        public static Network network = new Network("127.0.0.1", 1337);
        public static void Main()
        {
            Console.WriteLine("0. Wyjdz");
            if (network.Connect())
                Console.WriteLine("Polaczono");
            network.dataReceived += (source, args) =>
            {
                string responce = ((NetworkArgs)args).Message;
                Console.WriteLine(responce);
            };
            string message;
            while ((message = Console.ReadLine()) != "0")
            {
                while (message == String.Empty)
                {
                    Console.WriteLine("Napisz wiadomosc");
                    message = Console.ReadLine();
                }
                network.SendMessage(message).Wait();
                Console.WriteLine("Wyslano");
            }
        }
    }
}