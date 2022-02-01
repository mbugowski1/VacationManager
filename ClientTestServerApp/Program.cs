using System;
using System.Collections.Generic;
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
            network.dataReceived += SetUp;
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
            string[] raw = text.Split(' ');
            string commandCase = raw[0];
            Message.Code code;
            Message message = new Message();
            byte[] messageToSend;
            byte[][] credentials = new byte[2][];
            switch (commandCase)
            {
                case "test":
                    message.Operation = Message.Code.Test;
                    messageToSend = Serializer.Serialize(message);
                    network.SendMessage(messageToSend);
                    break;
                case "check":
                    message.Operation = Message.Code.CheckPass;
                    credentials[0] = Encoding.UTF8.GetBytes(raw[1]);
                    credentials[1] = Encoding.UTF8.GetBytes(raw[2]);
                    message.Data = Serializer.Serialize(credentials);
                    messageToSend = Serializer.Serialize(message);
                    network.SendMessage(messageToSend);
                    break;
                case "myevents":
                    message.Operation = Message.Code.GetEventsToMe;
                    messageToSend = Serializer.Serialize(message);
                    network.SendMessage(messageToSend);
                    break;
            }
        }
        private static void SetUp(object source, Message message)
        {
            switch (message.Operation)
            {
                case Test:
                    Console.WriteLine(Encoding.UTF8.GetString(Serializer.Deserialize<byte[]>(message.Data)));
                    break;
                case CheckPass:
                    Console.WriteLine((Serializer.Deserialize<bool>(message.Data)) ? "correct" : "incorrect");
                    break;
                case GetEventsToMe:
                    Serializer.Deserialize<List<VacationEvent>>(message.Data).ForEach(x => Console.WriteLine(x.ToString()));
                    break;
            }
        }
    }
}