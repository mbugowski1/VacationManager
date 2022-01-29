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
        public async Task SendMessage(string message)
        {
            if (!_socket.Connected) throw new SocketException();
            byte[] messageByte = Encoding.UTF8.GetBytes(message);
            await _socket.SendAsync(messageByte, SocketFlags.None);
        }
        public async Task<string> ReceiveMessage()
        {
            if (!_socket.Connected) throw new SocketException();
            byte[] buffer = new byte[2048];
            int length = await _socket.ReceiveAsync(buffer, SocketFlags.None);
            return Encoding.UTF8.GetString(buffer, 0, length);
        }
        public void Close() => _socket.Close();
    }
}
