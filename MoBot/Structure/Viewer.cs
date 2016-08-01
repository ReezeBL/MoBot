using System;
using System.Drawing;
using System.Windows.Forms;
using MoBot.Settings;
using MoBot.Structure.Actions;
using Newtonsoft.Json.Linq;

namespace MoBot.Structure
{
    public partial class Viewer : Form, IObserver<SysAction>
    {
        public Controller MainController;
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
            var actionConnect = value as ActionConnect;
            if (actionConnect != null)
            {
                var connect = actionConnect;
                if (connect.Connected)
                    Putsc($"Client connected!{Environment.NewLine}", Color.DarkGoldenrod);
            }
            else if (value is ActionMessage)
            {
                var message = (ActionMessage) value;
                Putsc($"{message.Message}{Environment.NewLine}", Color.Black);
            }
            else
            {
                var chatMessage = value as ActionChatMessage;
                if (chatMessage == null) return;
                var message = chatMessage;
                dynamic response = JObject.Parse(message.JsonMessage);
                if (response.extra != null)
                {
                    foreach (var obj in (JArray) response.extra)
                    {
                        if (obj is JValue)
                        {
                            Putsc($"{obj}", Color.White);
                        }
                        else if (obj != null)
                        {
                            JToken token = obj;
                            Putsc($"{token.Value<string>("text")}", Color.FromName(token.Value<string>("color")));
                        }
                    }
                }
                else
                {
                    Putsc(message.JsonMessage, Color.Black);
                }
                Putsc(Environment.NewLine, Color.White);
                //putsc($"{message.JSONMessage}{Environment.NewLine}", Color.Black);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            MainController.HandleConnect(usernameTextBox.Text, serverTextbox.Text);
        }

        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            if (chatTextBox.Text == "") return;
            MainController.HandleChatMessage(chatTextBox.Text);
            chatTextBox.Text = "";
        }

        
        private void Putsc(String text, Color color, String style = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color, string>(Putsc), text, color, style);
            }
            else
            {
                consoleWindow.SelectionStart = consoleWindow.Text.Length + 1;
                consoleWindow.AppendText(text);
                consoleWindow.SelectionLength = consoleWindow.Text.Length - consoleWindow.SelectionStart + 1;
                consoleWindow.SelectionColor = color;
                if (style == "italic")
                    consoleWindow.SelectionFont = new Font("Cambria", 12, FontStyle.Italic);
                if (!text.Contains("\n") && !text.Contains("\r")) return;
                consoleWindow.SelectionStart = consoleWindow.Text.Length;
                consoleWindow.SelectionLength = 0;
                consoleWindow.ScrollToCaret();
            }
        }

        private void Viewer_Load(object sender, EventArgs e)
        {
            GameSettings settings = GameSettings.Default;
            serverTextbox.Text = settings.Server;
            usernameTextBox.Text = settings.Username;
        }

        private void Viewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            GameSettings settings = GameSettings.Default;
            settings.Server = serverTextbox.Text;
            settings.Username = usernameTextBox.Text;
            settings.Save(); 
        }
    }
}
