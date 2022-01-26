using Xunit;
using VacationManagerBackend;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

namespace VacationManagerBackendTests
{
    public class NetworkTests
    {

        [Fact]
        public void Connecting_ToServer_Connected()
        {
            // Assign
            ushort port = 1100;
            Socket server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(1);
            server.BeginAccept(new AsyncCallback(AR =>
            {
                
            }), null);
            Network client = new Network(IPAddress.Loopback, port);
            
            // Act
            bool result = client.Connect();

            // Assert
            Assert.True(result);
        }
        [Fact]
        public void SendingData_ToServer_String()
        {
            // Assign
            ushort port = 1101;
            string message = "test";
            string receivedMessage = String.Empty;
            Socket server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(1);
            server.BeginAccept(new AsyncCallback(AR =>
            {
                Socket client = server.EndAccept(AR);
                byte[] buffer = new byte[1024];
                int length = client.Receive(buffer);
                receivedMessage = Encoding.UTF8.GetString(buffer, 0, length);
            }), null);
            Network client = new Network(IPAddress.Loopback, port);
            client.Connect();

            // Act
            client.SendMessage(message);
            while (receivedMessage == String.Empty) ;

            // Assert
            Assert.Equal(message, receivedMessage);
        }
        [Fact]
        public void ReceivingData_FromServer_GetString()
        {
            // Assign
            ushort port = 1102;
            string message = "test";
            string receivedMessage = String.Empty;
            Socket server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(1);
            server.BeginAccept(new AsyncCallback(AR =>
            {
                Socket client = server.EndAccept(AR);
                client.Send(Encoding.UTF8.GetBytes(message));
            }), null);
            Network client = new Network(IPAddress.Loopback, port);
            client.Connect();

            // Act
            receivedMessage = client.ReceiveMessage().Result;

            // Assert
            Assert.Equal(message, receivedMessage);
        }
    }
}