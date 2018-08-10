using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatClient.Callbacks;
using Network.Callbacks;
using Network.Network;
using Network.Owner;
using Network.Protocol;
using Network.Transport;
using Network.Utils;

namespace ChatClient
{
    public partial class ChatForm : Form
    {
        private readonly IUdpNetwork _network;
        private readonly IUdpTransport _transport;
        private IOwner _owner;
        public ChatForm()
        {
            InitializeComponent();
            _transport = new LiteNetLibClientTransport(1, "1");
            var protocol = new TransportUdpProtocol(_transport, 1000);
            _network = new MyNetwork(protocol, new RealNow());
            _network.RequestReceived += NetworkOnRequestReceived;
            _network.AuthorizeReceived += NetworkOnAuthorizeReceived;
            _network.Disconnected += NetworkOnDisconnected;
            _network.Connected += NetworkOnConnected;
            _network.Start(0);
        }

        private void NetworkOnConnected(IUdpNetwork network, IOwner owner)
        {
            _owner = owner;
            MessageBox.Show("OnConnected");
        }

        private void NetworkOnDisconnected(IUdpNetwork network, IOwner owner)
        {
            MessageBox.Show("OnDisconnected");
        }

        private void NetworkOnAuthorizeReceived(IUdpNetwork network, IOwner owner, string name, ICallbacks callbacks)
        {
            throw new NotImplementedException();
        }

        private void NetworkOnRequestReceived(IUdpNetwork network, Network.Owner.IOwner owner, Network.Values.IValue request, Network.Callbacks.ICallbacks callbacks)
        {
            throw new NotImplementedException();
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            _transport.Connect(tAddress.Text, Convert.ToInt32(tPort.Text));
        }

        private void UpdateTimerOnTick(object sender, EventArgs e)
        {
            _network.Update();
        }

        private void bAuthorization_Click(object sender, EventArgs e)
        {
            _network.Authorize(_owner, tName.Text, new AuthorizedCallbacks());
        }
    }
}
