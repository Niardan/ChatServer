using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
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
            var parametrs = new Parametrs("config.ini");
            parametrs.LoadParametrs();
            var transport = new LiteNetLibServerTransport(parametrs.MaxConnection, parametrs.KeyConnection);
            var protocol = new TransportUdpProtocol(transport, parametrs.MaxMessageSize, new BinarySerializer());
            var network = new ProtocolUdpNetwork(protocol, new RealNow(), parametrs.Timeout);
            _server = new ChatServer(network, parametrs.MaxMessageLength);
            _timer = new Timer(100);
            _timer.Elapsed += OnTimerElapsed;
            network.Start(parametrs.Port);
            _timer.Start();

            Console.ReadKey();
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _server.Udpate();
        }
    }
}
