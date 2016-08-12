﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using MoBot.Structure.Actions;
using MoBot.Structure.Windows;
using Newtonsoft.Json.Linq;

namespace MoBot.Structure
{
    public partial class Viewer : Form, IObserver<SysAction>
    {
        private static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>()
        {
            {"dark_blue", Color.DarkBlue},
            {"gold", Color.Gold },
            {"green", Color.Green },
            {"red", Color.Red },
            {"dark_green", Color.Green },
            {"white", Color.White },
            {"aqua", Color.Aqua },
            {"dark_red", Color.DarkRed },
        };

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
                else
                {
                    if (reconnectCheckBox.Checked)
                    {
                        MainController.HandleConnect(userNames.SelectedItem.ToString(), serverTextbox.Text, 3000);
                    }
                    else
                    {
                        buttonConnect.Enabled = true;
                    }
                }
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

                            string colorName = token.Value<string>("color") ?? "";
                            var color = Color.FromName(colorName);

                            Putsc($"{token.Value<string>("text")}", color);
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
            MainController.HandleConnect(userNames.SelectedItem.ToString(), serverTextbox.Text);
            //buttonConnect.Enabled = false;
        }

        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            if (chatTextBox.Text == "") return;
            MainController.HandleChatMessage(chatTextBox.Text);
            chatTextBox.Text = "";
        }

        
        private void Putsc(string text, Color color, string style = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color, string>(Putsc), text, color, style);
            }
            else
            {
                consoleWindow.SelectionStart = consoleWindow.TextLength;
                consoleWindow.SelectedText = text;
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
            serverTextbox.Text = Settings.ServerIp;
            reconnectCheckBox.Checked = Settings.AutoReconnect;
            XmlDocument users = new XmlDocument();
            try
            {
                users.Load(Settings.UserIdsPath);
                if (users.DocumentElement != null)
                    foreach (XmlNode item in users.DocumentElement)
                    {
                        userNames.Items.Add(item?.Attributes?.GetNamedItem("name").Value ?? "");
                    }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Viewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.ServerIp = serverTextbox.Text;
            Settings.AutoReconnect = reconnectCheckBox.Checked;

            Settings.Serialize();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void userNames_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
