using MoBot.Structure.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoBot.Structure
{
    partial class Viewer : Form, IObserver<SysAction>
    {
        internal Controller mainController;
        public Viewer()
        {
            InitializeComponent();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(SysAction value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<SysAction>(OnNext), value);
                return;
            }
            if (value is ActionConnect)
            {
                var connect = value as ActionConnect;
                if (connect.Connected)
                    consoleWindow.AppendText("Client connected!" + Environment.NewLine);
            }
            else if (value is ActionMessage)
            {
                var message = value as ActionMessage;
                consoleWindow.AppendText(message.message + Environment.NewLine);
            }
            else if (value is ActionChatMessage)
            {
                var message = value as ActionChatMessage;
                dynamic parsed = Newtonsoft.Json.Linq.JObject.Parse(message.JSONMessage);
                consoleWindow.AppendText(message.JSONMessage + Environment.NewLine);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            mainController.HandleConnect();
        }

        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            if(chatTextBox.Text != "")
            {
                mainController.HandleChatMessage(chatTextBox.Text);
                chatTextBox.Text = "";
            }
        }


        private static string strip_codes(string text)
        {
            // Strips the color codes from text.
            string smessage = text;
            if (smessage.Contains("§"))
            {

                smessage = smessage.Replace("§0", "");
                smessage = smessage.Replace("§1", "");
                smessage = smessage.Replace("§2", "");
                smessage = smessage.Replace("§3", "");
                smessage = smessage.Replace("§4", "");
                smessage = smessage.Replace("§5", "");
                smessage = smessage.Replace("§6", "");
                smessage = smessage.Replace("§7", "");
                smessage = smessage.Replace("§8", "");
                smessage = smessage.Replace("§9", "");
                smessage = smessage.Replace("§a", "");
                smessage = smessage.Replace("§b", "");
                smessage = smessage.Replace("§c", "");
                smessage = smessage.Replace("§d", "");
                smessage = smessage.Replace("§e", "");
                smessage = smessage.Replace("§f", "");
                smessage = smessage.Replace("§l", "");
                smessage = smessage.Replace("§r", "");
                smessage = smessage.Replace("§A", "");
                smessage = smessage.Replace("§B", "");
                smessage = smessage.Replace("§C", "");
                smessage = smessage.Replace("§D", "");
                smessage = smessage.Replace("§E", "");
                smessage = smessage.Replace("§F", "");

            }
            return smessage;
        }
    }
}
