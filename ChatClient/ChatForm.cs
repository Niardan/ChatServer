using System;
using System.Windows.Forms;
using Network.Network;
using Network.Protocol;
using Network.Transport;
using Network.Utils;

namespace ChatClient
{
    public partial class ChatForm : Form
    {
        private readonly ChatClient _chatClient;

        private ClientStage _currentStage;

        public ChatForm()
        {
            InitializeComponent();
            var parametrs = new Parametrs("config.ini");
            parametrs.LoadParametrs();

            var transport = new LiteNetLibTransport(parametrs.MaxConnection, parametrs.KeyConnection);
            var protocol = new TransportUdpProtocol(transport, parametrs.MaxMessageSize, new BinarySerializer());
            var network = new ProtocolUdpNetwork(protocol, new RealNow(), parametrs.Timeout);

            _chatClient = new ChatClient(transport, network, parametrs.MaxMessageLength);
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
                    AcceptButton = bConnect;
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
                    bConnect.Text = "Disconnect";
                    bConnect.Enabled = true;
                    AcceptButton = bAuthorization;
                    break;

                case ClientStage.Authorizing:
                    tName.Enabled = false;
                    bAuthorization.Enabled = false;
                    break;

                case ClientStage.Autorized:
                    tMessage.Enabled = true;
                    bSendMessage.Enabled = true;
                    AcceptButton = bSendMessage;
                    break;
            }
        }

        private void ChatClientOnMessage(string text, bool system)
        {
            if (system)
            {
                text = "System: " + text;
            }
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

        private void BAuthorization_Click(object sender, EventArgs e)
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
