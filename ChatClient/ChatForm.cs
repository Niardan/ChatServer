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
using Network.Values;
using ChatMessageCallbacks = ChatClient.Callbacks.ChatMessageCallbacks;

namespace ChatClient
{
    public partial class ChatForm : Form
    {
        private readonly MyNetwork _network;
        private readonly IUdpTransport _transport;
        private IOwner _owner;
        private bool _isConnect;

        public ChatForm()
        {
            InitializeComponent();
            _transport = new LiteNetLibClientTransport(1, "1");
            var protocol = new TransportUdpProtocol(_transport, 1000, new BinarySerializer());
            _network = new MyNetwork(protocol, new RealNow());
            _network.RequestReceived += NetworkOnRequestReceived;
            _network.Disconnected += NetworkOnDisconnected;
            _network.Connected += NetworkOnConnected;
            _network.Start(0);
        }

        private void NetworkOnConnected(IUdpNetwork network, IOwner owner)
        {
            _owner = owner;
            bAuthorization.Enabled = true;
            tName.Enabled = true;
            chatBox.Items.Add("Connected");
        }

        private void NetworkOnDisconnected(IUdpNetwork network, IOwner owner)
        {
            bAuthorization.Enabled = false;
            bSendMessage.Enabled = false;
            tAddress.Enabled = true;
            tPort.Enabled = true;
            chatBox.Items.Add("Disconnected");
        }

        private void NetworkOnRequestReceived(IUdpNetwork network, Network.Owner.IOwner owner, Network.Values.IValue request, Network.Callbacks.ICallbacks callbacks)
        {
            var value = (ChatValue) request;
            string message = value.Name + ": " + value.Message;
            chatBox.Items.Add(message);
        }

        private void BConnectOnClick(object sender, EventArgs e)
        {
            if (!_isConnect)
            {
                _transport.Connect(tAddress.Text, Convert.ToInt32(tPort.Text));
                tAddress.Enabled = false;
                tPort.Enabled = false;
                bConnect.Text = "Disconnect";
            }
            else
            {
                _network.Disconnect(_owner);
                bConnect.Text = "Connect";
            }

            _isConnect = !_isConnect;
        }

        private void UpdateTimerOnTick(object sender, EventArgs e)
        {
            _network.Update();
        }

        private void bAuthorization_Click(object sender, EventArgs e)
        {
            bAuthorization.Enabled = false;
            tName.Enabled = false;
            _network.Authorize(_owner, tName.Text, new AuthorizedCallbacks(_network, _owner, AuthorizedSuccess, AuthorizedFail));
        }

        private void BSendMessageOnClick(object sender, EventArgs e)
        {
            var value = new ChatValue(tName.Text, tMessage.Text);
            string message = tName.Text + ": " + tMessage.Text;
            chatBox.Items.Add(message);

            _network.Request(_owner, value, new ChatMessageCallbacks(ChatSendFail));
        }

        private void ChatSendFail(string reason)
        {
            chatBox.Items.Add("Chat Send Fail, Reason: " + reason);
        }

        private void AuthorizedSuccess()
        {
            tMessage.Enabled = true;
            bSendMessage.Enabled = true;
            chatBox.Items.Add("Authorized");
        }

        private void AuthorizedFail(string reason)
        {
            tName.Enabled = true;
            bAuthorization.Enabled = true;
            chatBox.Items.Add("Authorized Fail, Reason: " + reason);
        }
    }
}
