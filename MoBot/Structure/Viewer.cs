using MoBot.Structure.Actions;
using Newtonsoft.Json.Linq;
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
            if (value is ActionConnect)
            {
                var connect = value as ActionConnect;
                if (connect.Connected)
                    putsc($"Client connected!{Environment.NewLine}", Color.DarkGoldenrod);
            }
            else if (value is ActionMessage)
            {
                var message = value as ActionMessage;
                putsc($"{message.message}{Environment.NewLine}", Color.Black);
            }
            else if (value is ActionChatMessage)
            {
                var message = value as ActionChatMessage;
                dynamic response = JObject.Parse(message.JSONMessage);
                if (response.extra != null)
                {
                    foreach (Object obj in response.extra as JArray)
                    {
                        if (obj is JValue)
                        {
                            putsc($"{obj.ToString()}", Color.White);
                        }
                        else if (obj is JToken)
                        {
                            JToken token = obj as JToken;
                            putsc($"{token.Value<string>("text")}", Color.FromName(token.Value<string>("color")));
                        }
                    }
                }
                else
                {
                    putsc(message.JSONMessage, Color.Black);
                }
                putsc(Environment.NewLine, Color.White);
                //putsc($"{message.JSONMessage}{Environment.NewLine}", Color.Black);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            mainController.HandleConnect(usernameTextBox.Text, serverTextbox.Text);
        }

        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            if(chatTextBox.Text != "")
            {
                mainController.HandleChatMessage(chatTextBox.Text);
                chatTextBox.Text = "";
            }
        }

        
        private void putsc(String text, Color color, String style = "")
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, Color, string>(putsc), text, color, style);
            }
            else
            {
                consoleWindow.SelectionStart = consoleWindow.Text.Length + 1;
                consoleWindow.AppendText(text);
                consoleWindow.SelectionLength = consoleWindow.Text.Length - consoleWindow.SelectionStart + 1;
                consoleWindow.SelectionColor = color;
                if (style == "italic")
                    consoleWindow.SelectionFont = new Font("Cambria", 12, FontStyle.Italic);
                if (text.Contains("\n") || text.Contains("\r"))
                {
                    consoleWindow.SelectionStart = consoleWindow.Text.Length;
                    consoleWindow.SelectionLength = 0;
                    consoleWindow.ScrollToCaret();
                }
            }
        }

        private void Viewer_Load(object sender, EventArgs e)
        {
            Settings settings = Settings.Default;
            serverTextbox.Text = settings.Server;
            usernameTextBox.Text = settings.Username;
        }

        private void Viewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings settings = Settings.Default;
            settings.Server = serverTextbox.Text;
            settings.Username = usernameTextBox.Text;
            settings.Save(); 
        }
    }
}
