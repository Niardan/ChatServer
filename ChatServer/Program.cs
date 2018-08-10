using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MessagePack;
using Network.Network;
using Network.Protocol;
using Network.Transport;

namespace ChatServer
{
    class Program
    {
        private static Timer _timer;
        private static ChatServer _server;

        static void Main(string[] args)
        {
            var transport = new LiteNetLibTransport(100, "1");
            var protocol = new TransportUdpProtocol(transport, 1000);
            var network = new MyNetwork(protocol);
            _server = new ChatServer(network, 128);
            _timer = new Timer(100);
            _timer.Elapsed += OnTimerElapsed;
            Console.ReadKey();
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _server.Udpate();
        }
    }
}
