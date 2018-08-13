using System;
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

        static void Main()
        {
            var parametrs = new Parametrs("config.ini");
            parametrs.LoadParametrs();
            var transport = new LiteNetLibTransport(parametrs.MaxConnection, parametrs.KeyConnection);
            var protocol = new TransportUdpProtocol(transport, parametrs.MaxMessageSize, new BinarySerializer());
            var network = new ProtocolUdpNetwork(protocol, new RealNow(), parametrs.Timeout);
            _server = new ChatServer(network, parametrs.MaxMessageLength);
            _timer = new Timer(100);
            _timer.Elapsed += OnTimerElapsed;
            network.Start(parametrs.Port);
            _timer.Start();
            Console.WriteLine("Serve run, port: {0} ", parametrs.Port);
            Console.ReadKey();
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _server.Udpate();
        }
    }
}
