using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using VacationManagerLibrary;

namespace VacationManagerBackend
{
    public partial class Network
    {
        public EventHandler<Message>? dataReceived;
        private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly IPAddress ip;
        private readonly ushort port;
        private byte[] _buffer = new byte[2048];
        public Network(string ip, ushort port)
        {
            this.ip = IPAddress.Parse(ip);
            this.port = port;
            dataReceived += SetUp;
        }
        public Network(IPAddress ip, ushort port)
        {
            this.ip = ip;
            this.port = port;
            dataReceived += SetUp;
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
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            return true;
        }
        private void ReceiveCallback(IAsyncResult AR)
        {
            int received;
            try
            {
                received = _socket.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Server disconnected");
                return;
            }
            ArraySegment<byte> recText = new(_buffer, 0, received);
            var args = Serializer.Deserialize<Message>(recText.ToArray());
            dataReceived?.Invoke(this, args);
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
        }
        public async Task SendMessage(byte[] message)
        {
            if (!_socket.Connected) throw new SocketException();
            await _socket.SendAsync(message, SocketFlags.None);
        }
        public async Task<Message> ReceiveMessage()
        {
            if (!_socket.Connected) throw new SocketException();
            byte[] buffer = new byte[2048];
            int length = await _socket.ReceiveAsync(buffer, SocketFlags.None);
            ArraySegment<byte> array = new(buffer, 0, length);
            return Serializer.Deserialize<Message>(array.ToArray());
        }
        public void Close() => _socket.Close();
        private void SetUp(object source, Message message)
        {
            
        }
    }
}
