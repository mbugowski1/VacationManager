using Xunit;
using System.Net.Sockets;
using System.Net;
using Server;
using System.Collections.Generic;

namespace VacationManagerServerTests
{
    public class NetworkTests
    {
        [Fact]
        public void Connecting_ToServer_Connected()
        {
            Network service = new(1000);
            service.Open();

            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(IPAddress.Loopback, 1000);

            Assert.True(socket.Connected);
            socket.Close();
            service.Close();
        }
        [Theory]
        [InlineData(0, 1001)]
        [InlineData(1, 1002)]
        [InlineData(2, 1003)]
        public void Disconnecting_FromServer_Disconnected(short userCount, ushort port)
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

            service.Close();
            users.ForEach(x => x.Close());
            Assert.True(true);
        }
    }
}