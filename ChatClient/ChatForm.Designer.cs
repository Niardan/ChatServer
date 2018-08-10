namespace ChatClient
{
    partial class ChatForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bConnect = new System.Windows.Forms.Button();
            this.tAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tName = new System.Windows.Forms.TextBox();
            this.bAuthorization = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tMessage = new System.Windows.Forms.TextBox();
            this.bSendMessage = new System.Windows.Forms.Button();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.chatBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // bConnect
            // 
            this.bConnect.Location = new System.Drawing.Point(461, 27);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(92, 23);
            this.bConnect.TabIndex = 0;
            this.bConnect.Text = "Подключится";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.BConnectOnClick);
            // 
            // tAddress
            // 
            this.tAddress.Location = new System.Drawing.Point(111, 29);
            this.tAddress.Name = "tAddress";
            this.tAddress.Size = new System.Drawing.Size(100, 20);
            this.tAddress.TabIndex = 1;
            this.tAddress.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(254, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Порт";
            // 
            // tPort
            // 
            this.tPort.Location = new System.Drawing.Point(320, 28);
            this.tPort.Name = "tPort";
            this.tPort.Size = new System.Drawing.Size(100, 20);
            this.tPort.TabIndex = 4;
            this.tPort.Text = "41200";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Имя";
            // 
            // tName
            // 
            this.tName.Enabled = false;
            this.tName.Location = new System.Drawing.Point(111, 73);
            this.tName.Name = "tName";
            this.tName.Size = new System.Drawing.Size(100, 20);
            this.tName.TabIndex = 6;
            this.tName.Text = "Vel";
            // 
            // bAuthorization
            // 
            this.bAuthorization.Enabled = false;
            this.bAuthorization.Location = new System.Drawing.Point(257, 70);
            this.bAuthorization.Name = "bAuthorization";
            this.bAuthorization.Size = new System.Drawing.Size(97, 23);
            this.bAuthorization.TabIndex = 7;
            this.bAuthorization.Text = "Авторизация";
            this.bAuthorization.UseVisualStyleBackColor = true;
            this.bAuthorization.Click += new System.EventHandler(this.bAuthorization_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Сообщение";
            // 
            // tMessage
            // 
            this.tMessage.Enabled = false;
            this.tMessage.Location = new System.Drawing.Point(98, 143);
            this.tMessage.Name = "tMessage";
            this.tMessage.Size = new System.Drawing.Size(349, 20);
            this.tMessage.TabIndex = 9;
            // 
            // bSendMessage
            // 
            this.bSendMessage.Enabled = false;
            this.bSendMessage.Location = new System.Drawing.Point(494, 141);
            this.bSendMessage.Name = "bSendMessage";
            this.bSendMessage.Size = new System.Drawing.Size(75, 23);
            this.bSendMessage.TabIndex = 10;
            this.bSendMessage.Text = "Отправить";
            this.bSendMessage.UseVisualStyleBackColor = true;
            this.bSendMessage.Click += new System.EventHandler(this.BSendMessageOnClick);
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Tick += new System.EventHandler(this.UpdateTimerOnTick);
            // 
            // chatBox
            // 
            this.chatBox.FormattingEnabled = true;
            this.chatBox.Location = new System.Drawing.Point(12, 181);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(541, 238);
            this.chatBox.TabIndex = 11;
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 450);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.bSendMessage);
            this.Controls.Add(this.tMessage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.bAuthorization);
            this.Controls.Add(this.tName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tAddress);
            this.Controls.Add(this.bConnect);
            this.Name = "ChatForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bConnect;
        private System.Windows.Forms.TextBox tAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tName;
        private System.Windows.Forms.Button bAuthorization;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tMessage;
        private System.Windows.Forms.Button bSendMessage;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.ListBox chatBox;
    }
}

