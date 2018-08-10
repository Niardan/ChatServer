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
using Network.Utils;

namespace ChatServer
{
    class Program
    {
        private static Timer _timer;
        private static ChatServer _server;

        static void Main(string[] args)
        {
            var transport = new LiteNetLibServerTransport(100, "1");
            var protocol = new TransportUdpProtocol(transport, 1000, new BinarySerializer());
            var network = new MyNetwork(protocol, new RealNow());
            _server = new ChatServer(network, 10);
            _timer = new Timer(100);
            _timer.Elapsed += OnTimerElapsed;
            network.Start(41200);
            _timer.Start();

            Console.ReadKey();
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _server.Udpate();
        }
    }
}
