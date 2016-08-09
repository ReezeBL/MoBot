using System.ComponentModel;
using System.Windows.Forms;

namespace MoBot.Structure
{
    partial class Viewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.consoleWindow = new System.Windows.Forms.RichTextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.chatTextBox = new System.Windows.Forms.TextBox();
            this.buttonSendMessage = new System.Windows.Forms.Button();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.serverTextbox = new System.Windows.Forms.TextBox();
            this.reconnectCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // consoleWindow
            // 
            this.consoleWindow.BackColor = System.Drawing.SystemColors.GrayText;
            this.consoleWindow.Location = new System.Drawing.Point(12, 45);
            this.consoleWindow.Name = "consoleWindow";
            this.consoleWindow.ReadOnly = true;
            this.consoleWindow.Size = new System.Drawing.Size(428, 240);
            this.consoleWindow.TabIndex = 0;
            this.consoleWindow.Text = "";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(457, 184);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(135, 36);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // chatTextBox
            // 
            this.chatTextBox.Location = new System.Drawing.Point(12, 293);
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.Size = new System.Drawing.Size(427, 20);
            this.chatTextBox.TabIndex = 2;
            // 
            // buttonSendMessage
            // 
            this.buttonSendMessage.Location = new System.Drawing.Point(457, 293);
            this.buttonSendMessage.Name = "buttonSendMessage";
            this.buttonSendMessage.Size = new System.Drawing.Size(135, 20);
            this.buttonSendMessage.TabIndex = 3;
            this.buttonSendMessage.Text = "Send Message";
            this.buttonSendMessage.UseVisualStyleBackColor = true;
            this.buttonSendMessage.Click += new System.EventHandler(this.buttonSendMessage_Click);
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(462, 67);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(95, 20);
            this.usernameTextBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(464, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(464, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Server";
            // 
            // serverTextbox
            // 
            this.serverTextbox.Location = new System.Drawing.Point(462, 127);
            this.serverTextbox.Name = "serverTextbox";
            this.serverTextbox.Size = new System.Drawing.Size(130, 20);
            this.serverTextbox.TabIndex = 4;
            // 
            // reconnectCheckBox
            // 
            this.reconnectCheckBox.AutoSize = true;
            this.reconnectCheckBox.Location = new System.Drawing.Point(457, 227);
            this.reconnectCheckBox.Name = "reconnectCheckBox";
            this.reconnectCheckBox.Size = new System.Drawing.Size(99, 17);
            this.reconnectCheckBox.TabIndex = 6;
            this.reconnectCheckBox.Text = "Auto reconnect";
            this.reconnectCheckBox.UseVisualStyleBackColor = true;
            // 
            // Viewer
            // 
            this.AcceptButton = this.buttonSendMessage;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 336);
            this.Controls.Add(this.reconnectCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serverTextbox);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.buttonSendMessage);
            this.Controls.Add(this.chatTextBox);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.consoleWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Viewer";
            this.Text = "MoBot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Viewer_FormClosing);
            this.Load += new System.EventHandler(this.Viewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RichTextBox consoleWindow;
        private Button buttonConnect;
        private TextBox chatTextBox;
        private Button buttonSendMessage;
        private TextBox usernameTextBox;
        private Label label1;
        private Label label2;
        private TextBox serverTextbox;
        private CheckBox reconnectCheckBox;
    }
}