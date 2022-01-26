using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VacationManagerBackend
{
    public class Network
    {
        private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly IPAddress ip;
        private readonly ushort port;
        public Network(string ip, ushort port)
        {
            this.ip = IPAddress.Parse(ip);
            this.port = port;
        }
        public Network(IPAddress ip, ushort port)
        {
            this.ip = ip;
            this.port = port;
        }
        public bool Connect()
        {

            int attempts = 0;
            while (!_socket.Connected || attempts > 4)
            {
                try
                {
                    attempts++;
                    _socket.Connect(ip, port);
                }
                catch (SocketException){}
            }
            if (!_socket.Connected)
                return false;
            return true;
        }
        public void SendMessage(string message)
        {
            byte[] buffer = new byte[2048];
            byte[] messageByte = Encoding.UTF8.GetBytes(message);
            _socket.Send(messageByte);
        }
        public void Close() => _socket.Close();
        /*private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndReceive(AR);
        }*/
    }
}
