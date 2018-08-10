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
        private readonly MyNetwork _network;
        private readonly IDictionary<IOwner, string> _clients = new Dictionary<IOwner, string>();
        private readonly Queue<Tuple<IOwner, IValue, ICallbacks>> _request = new Queue<Tuple<IOwner, IValue, ICallbacks>>();

        private readonly string _alreadyAutorized = "alreadyAutorized";
        private readonly string _unknownFormat = "unknownFormat";
        private readonly string _toBigChatMessage = "toBigChatMessage";
        private readonly string _ok = "ok";

        private readonly int _maxLengthMessage;
        public ChatServer(MyNetwork network, int maxLengthMessage)
        {
            _network = network;
            _maxLengthMessage = maxLengthMessage;
            _network.AuthorizeReceived += OnAuthorizeReceived;
            _network.Disconnected += OnDisconnected;
            _network.RequestReceived += OnRequestReceived;
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

        private void OnRequestReceived(IUdpNetwork network, IOwner owner, IValue request, ICallbacks callbacks)
        {
            _request.Enqueue(new Tuple<IOwner, IValue, ICallbacks>(owner, request, callbacks));
        }

        private void OnDisconnected(IUdpNetwork network, IOwner owner)
        {
            _clients.Remove(owner);
        }

        private void OnAuthorizeReceived(IUdpNetwork network, IOwner owner, string name, ICallbacks callbacks)
        {
            foreach (var item in _clients.Values)
            {
                if (item == name)
                {
                    callbacks.Fail(_alreadyAutorized);
                    _network.Authorize(owner, false);
                    break;
                }
            }
            callbacks.Ack(_ok);
            _network.Authorize(owner, true);
            _clients.Add(owner, name);
        }
    }
}