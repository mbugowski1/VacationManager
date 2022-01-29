using Xunit;
using System.Net.Sockets;
using System.Net;
using VacationManagerServer;
using System.Collections.Generic;
using System.Text;
using System;
using System.Threading;

namespace VacationManagerServerTests
{
    public class NetworkTests
    {
        [Fact]
        public void Connecting_ToServer_Connected()
        {
            ushort port = 1000;
            Network service = new(port);
            service.Open();

            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(IPAddress.Loopback, port);

            Assert.True(socket.Connected);
            socket.Close();
        }
        [Fact]
        public void ReceivingData_Server_GetString()
        {
            // Arrange
            ushort port = 1001;
            Network service = new(port);
            DataArgs? data = null;
            bool wait = true;
            service.dataReceived += (sender, e) =>
            {
                data = e;
                wait = false;
            };
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string user = String.Empty;
            string message = "test";
            byte[] send = Encoding.UTF8.GetBytes(message);

            // Act
            service.Open();
            socket.Connect(IPAddress.Loopback, port);
            socket.Send(send);
            while (wait) ;

            // Assert
            #pragma warning disable CS8602 // Wy³uskanie odwo³ania, które mo¿e mieæ wartoœæ null.
            Assert.Equal(user, data.Username);
            #pragma warning restore CS8602 // Wy³uskanie odwo³ania, które mo¿e mieæ wartoœæ null.
            Assert.Equal(message, data.Data);

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        [Fact]
        public void Disconnecting_FromServer_Disconnected()
        {
            ushort port = 1002;
            Network service = new(port);
            service.Open();

            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(IPAddress.Loopback, port);

            socket.Close();
          
            Assert.True(true);
        }
        [Fact]
        public void SendingData_FromServer_SendString()
        {
            // Assign
            ushort port = 1006;
            Network service = new(port);
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string message = "test";
            string expected = "test echo";
            byte[] buffer = new byte[2048];

            // Act
            service.Open();
            socket.Connect(IPAddress.Loopback, port);
            socket.Send(Encoding.UTF8.GetBytes(message));
            socket.Receive(buffer);
            string received = Encoding.UTF8.GetString(buffer);
            received = received.Substring(0, received.IndexOf('\0'));

            // Assert
            Assert.Equal(expected, received);
            socket.Close();
        }
        [Fact]
        public void SendingData_FromServer_GetEvent()
        {
            // Assign
            ushort port = 1007;
            Network service = new(port);
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string message = "test";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            bool eventFired = false;
            service.dataSent += (source, e) => eventFired = true;

            // Act
            service.Open();
            socket.Connect(IPAddress.Loopback, port);
            socket.Send(messageBytes);
            socket.Receive(new byte[2048]);

            // Assert
            Assert.True(eventFired);
        }
    }
}