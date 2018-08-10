﻿using Network.Values;

namespace Network.Protocol
{
    public delegate void ProtocolConnectedHandler(IUdpProtocol protocol, string address);
    public delegate void ProtocolDisconnectedHandler(IUdpProtocol protocol, string address);
    public delegate void ProtocolAuthorizeReceived(IUdpProtocol protocol, string address, long id, string name);
    public delegate void ProtocolRequestReceived(IUdpProtocol protocol, string address, long id, IValue message);
    public delegate void ProtocolResponseReceived(IUdpProtocol protocol, string address, long id, IValue message);

    public interface IUdpProtocol
    {
        event ProtocolConnectedHandler Connected;
        event ProtocolDisconnectedHandler Disconnectd;
        event ProtocolAuthorizeReceived AuthorizeReceived;
        event ProtocolRequestReceived RequestReceived;
        event ProtocolResponseReceived ResponseReceived;

        bool Start(int port);
        void Stop();

        void Update();

        void Connect(string host, int port);
        void Disconnect(string address);
        
        void Request(string address, long id, IValue value);
        void Ack(string address, long id, string text);
        void Fail(string address, long id, string text);
        void Error(string address, string message);
    }
}