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
        private readonly ChatClient _chatClient;
        private IOwner _owner;
        private bool _isConnect;

        private ClientStage _currentStage;

        public ChatForm()
        {
            InitializeComponent();

            var transport = new LiteNetLibClientTransport(1, "1");
            var protocol = new TransportUdpProtocol(transport, 1000, new BinarySerializer());
            var network = new MyNetwork(protocol, new RealNow());
            _chatClient = new ChatClient(transport, network);
            _chatClient.Message += ChatClientOnMessage;
            _chatClient.ChangeStage += ChatClientOnChangeStage;
            _currentStage = ClientStage.Disconnected;
        }

        private void ChatClientOnChangeStage(ClientStage stage)
        {
            _currentStage = stage;
            switch (stage)
            {
                case ClientStage.Disconnecting:
                    tMessage.Enabled = false;
                    tName.Enabled = false;
                    bAuthorization.Enabled = false;
                    bSendMessage.Enabled = false;
                    break;

                case ClientStage.Disconnected:
                    tAddress.Enabled = true;
                    tPort.Enabled = true;
                    bConnect.Text = "Connect";
                    bConnect.Enabled = true;
                    break;

                case ClientStage.Connecting:
                    tAddress.Enabled = false;
                    tPort.Enabled = false;
                    bConnect.Enabled = false;
                    break;

                case ClientStage.Connected:
                    tName.Enabled = true;
                    bAuthorization.Enabled = true;
                    bConnect.Text = "Diconnect";
                    bConnect.Enabled = true;
                    break;

                case ClientStage.Authorizing:
                    tName.Enabled = false;
                    bAuthorization.Enabled = false;
                    break;

                case ClientStage.Autorized:
                    tMessage.Enabled = true;
                    bSendMessage.Enabled = true;
                    break;
            }
        }

        private void ChatClientOnMessage(string text, bool system)
        {
            chatBox.Items.Add(text);
        }


        private void BConnectOnClick(object sender, EventArgs e)
        {
            if (_currentStage == ClientStage.Disconnected)
            {
                _chatClient.Connect(tAddress.Text, Convert.ToInt32(tPort.Text));
            }
            else
            {
                _chatClient.Disconnect();
            }
        }

        private void UpdateTimerOnTick(object sender, EventArgs e)
        {
            _chatClient.Update();
        }

        private void bAuthorization_Click(object sender, EventArgs e)
        {
           _chatClient.Authorize(tName.Text);
        }

        private void BSendMessageOnClick(object sender, EventArgs e)
        {
           _chatClient.SendMessage(tMessage.Text);
            tMessage.Text = "";
        }
    }
}
