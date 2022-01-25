using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Service
    {
        private volatile bool _working;
        private List<Socket> _clients = new();
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private byte[] _buffer = new byte[2048];
        public Service()
        {
            
        }
        public void Start()
        {
            _working = true;
            new Thread(new ThreadStart(Run)).Start();
            Debug.WriteLine("Starting Server");
        }
        public void Stop()
        {
            _working = false;
        }
        public void Run()
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, 1337));
            _socket.Listen(5);
            _socket.BeginAccept(AcceptCallback, null);
        }
        private void AcceptCallback(IAsyncResult AR)
        {
            Socket client = _socket.EndAccept(AR);
            _clients.Add(client);
            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, client);
        }
        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket? client = (Socket?)AR.AsyncState;
            if (client == null) return;
            int received = client.EndReceive(AR);
            string recText = Encoding.UTF8.GetString(_buffer, 0, received);
            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, client);
            //client.Close();
        }
    }
}
