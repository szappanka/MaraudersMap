using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpMulticastServer
{
    public class MulticastServer : TcpServer
    {
        public MulticastServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new MulticastSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
        }
    }
}
