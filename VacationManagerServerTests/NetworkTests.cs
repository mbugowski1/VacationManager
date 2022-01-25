using Xunit;
using System.Net.Sockets;
using System.Net;
using Server;
using System.Collections.Generic;
using System.Text;
using System;

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
            service.Close();
        }
        [Fact]
        public void ReceivingData_Server_GetString()
        {
            // Arrange
            ushort port = 1001;
            Network service = new(port);
            DataArgs data = null;
            bool wait = true;
            service.dataReceived += (sender, e) =>
            {
                data = e;
                wait = false;
            };
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string user = "Unknown";
            string message = "test";
            byte[] send = Encoding.UTF8.GetBytes(message);

            // Act
            service.Open();
            socket.Connect(IPAddress.Loopback, port);
            socket.Send(send);
            while (wait) ;

            // Assert
            Assert.Equal(user, data.Username);
            Assert.Equal(message, data.Data);
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
            service.Close();
        }
        [Theory]
        [InlineData(0, 1003)]
        [InlineData(1, 1004)]
        [InlineData(2, 1005)]
        public void ShuttingDown_Server(short userCount, ushort port)
        {
            Network service = new(port);
            service.Open();

            List<Socket> users = new();
            for(short i = 0; i < userCount; i++)
            {
                var user = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                user.Connect(IPAddress.Loopback, port);
                users.Add(user);
            }

            var exception = Record.Exception(() => service.Close());
            users.ForEach(x => x.Close());
            Assert.Null(exception);
        }
    }
}