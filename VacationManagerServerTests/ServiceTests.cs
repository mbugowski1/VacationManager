using Xunit;
using Server;
using System.Net.Sockets;
using System.Net;

namespace VacationManagerServerTests
{
    public class ServiceTests
    {
        [Fact]
        public void Connecting_ToServer_Connected()
        {
            Service service = new();
            service.Run();

            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveTimeout = 2000;
            socket.Connect(IPAddress.Loopback, 1337);

            Assert.True(socket.Connected);
            socket.Close();
        }
    }
}