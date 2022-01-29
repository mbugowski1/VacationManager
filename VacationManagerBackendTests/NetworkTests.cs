using Xunit;
using VacationManagerBackend;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

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
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Socket server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(1);
            server.BeginAccept(new AsyncCallback(AR =>
            {
                Socket client = server.EndAccept(AR);
                byte[] buffer = new byte[1024];
                int length = client.Receive(buffer);
                receivedMessage = Encoding.UTF8.GetString(buffer, 0, length);
                tokenSource.Cancel();
            }), null);
            Network client = new Network(IPAddress.Loopback, port);
            client.Connect();

            // Act
            client.SendMessage(message).Wait();
            try
            {
                Task.Delay(2000, token).Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerException == null)
                    throw e;
                if (e.InnerException.GetType() != typeof(TaskCanceledException))
                    throw e;
            }
            finally
            {
                tokenSource.Dispose();
            }

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
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Socket server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(1);
            server.BeginAccept(new AsyncCallback(AR =>
            {
                Socket client = server.EndAccept(AR);
                client.Send(Encoding.UTF8.GetBytes(message));
            }), null);
            Network client = new Network(IPAddress.Loopback, port);
            client.dataReceived += (source, args) =>
            {
                receivedMessage = args.Message;
                tokenSource.Cancel();
            };

            // Act
            client.Connect();
            try
            {
                Task.Delay(2000, token).Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerException == null)
                    throw e;
                if (e.InnerException.GetType() != typeof(TaskCanceledException))
                    throw e;
            }
            finally
            {
                tokenSource.Dispose();
            }

            // Assert
            Assert.Equal(message, receivedMessage);
        }
    }
}