using System;
using System.Collections.Generic;
using Network.Callbacks;
using Network.Network;
using Network.Owner;
using Network.Values;

namespace ChatServer
{
    public class ChatServer
    {
        private readonly ProtocolUdpNetwork _network;
        private readonly IDictionary<IOwner, string> _clients = new Dictionary<IOwner, string>();
        private readonly Queue<Tuple<IOwner, IValue, ICallbacks>> _request = new Queue<Tuple<IOwner, IValue, ICallbacks>>();

        private readonly string _dublicateUsername = "dublicateUsername";
        private readonly string _incorrectUsername = "incorrectUsername";
        private readonly string _unknownFormat = "unknownFormat";
        private readonly string _toBigChatMessage = "toBigChatMessage";
        private readonly string _toEmptyMessage = "toEmptyMessage";
        private readonly string _ok = "ok";

        private readonly int _maxLengthMessage;

        public ChatServer(ProtocolUdpNetwork network, int maxLengthMessage)
        {
            _network = network;
            _maxLengthMessage = maxLengthMessage;
            _network.AuthorizeReceived += OnAuthorizeReceived;
            _network.Connected+=NetworkOnConnected;
            _network.Disconnected += OnDisconnected;
            _network.RequestReceived += OnRequestReceived;
            Console.WriteLine("ChatServer Run");
        }

        public void Udpate()
        {
            _network.Update();
            while (_request.Count > 0)
            {
                var request = _request.Dequeue();
                var value = request.Item2 as ChatValue;
                var callback = request.Item3;
                var owner = request.Item1;
                if (value == null)
                {
                    callback.Fail(_unknownFormat);
                    return;
                }

                if (string.IsNullOrEmpty(value.Message))
                {
                    callback.Fail(_toEmptyMessage);
                    return;
                }

                if (value.Message.Length > _maxLengthMessage)
                {
                    callback.Fail(_toBigChatMessage);
                    return;
                }

                string name = _clients[owner];
                foreach (var client in _clients)
                {
                    IOwner clientOwner = client.Key;
                    if (clientOwner != owner)
                    {
                        _network.Request(clientOwner, value, new ChatMessageCallbacks(name, client.Value, value.Message));
                    }
                }
                callback.Ack(_ok);
            }
        }

        private void NetworkOnConnected(IUdpNetwork network, IOwner owner)
        {
            Console.WriteLine("Connected, address: {0}", owner.Id);
        }

        private void OnRequestReceived(IUdpNetwork network, IOwner owner, IValue request, ICallbacks callbacks)
        {
            var chatValue = (ChatValue)request;
            Console.WriteLine("Server receive chat message, address: {0}, name: {1}, message: {2}", owner.Id, chatValue.Name, chatValue.Message);

            _request.Enqueue(new Tuple<IOwner, IValue, ICallbacks>(owner, request, callbacks));
        }

        private void OnDisconnected(IUdpNetwork network, IOwner owner)
        {
            Console.WriteLine("Disconneted, address: {0}", owner.Id);
            _clients.Remove(owner);
        }

        private void OnAuthorizeReceived(IUdpNetwork network, IOwner owner, string name, ICallbacks callbacks)
        {
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Authorized failed");
                callbacks.Fail(_incorrectUsername);
                _network.Authorize(owner, false);
                return;
            }

            foreach (var item in _clients.Values)
            {
                if (item == name)
                {
                    Console.WriteLine("Authorized failed");
                    callbacks.Fail(_dublicateUsername);
                    _network.Authorize(owner, false);
                    return;
                }
            }
            Console.WriteLine("Authorized true, address: {0}", owner.Id);
           
            callbacks.Ack(_ok);
            _network.Authorize(owner, true);
            _clients.Add(owner, name);
        }
    }
}