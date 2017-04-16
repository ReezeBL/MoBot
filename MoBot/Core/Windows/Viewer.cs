﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using fNbt;
using MoBot.Core.Actions;
using MoBot.Core.Game;
using Newtonsoft.Json.Linq;
using static System.String;

namespace MoBot.Core.Windows
{
    public partial class Viewer : Form, IObserver<SysAction>
    {
        private static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>
        {
            {"dark_blue", Color.DarkBlue},
            {"gold", Color.Gold},
            {"green", Color.Green},
            {"red", Color.Red},
            {"dark_green", Color.Green},
            {"white", Color.White},
            {"aqua", Color.Aqua},
            {"dark_red", Color.DarkRed}
        };

        public Controller MainController;

        public Viewer()
        {
            InitializeComponent();
            entityList.DoubleBuffering(true);
            GameController.Instance.PropertyChanged += InstanceOnPropertyChanged;
        }

        public List<Entity> Entities { get; set; } = new List<Entity> {new Player(0, "kek")};

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x2000000;
                return cp;
            }
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
                    if (Settings.AutoReconnect)
                    {
                        Connect(10000);
                    }
                    else
                    {
                        buttonConnect.Text = "Connect";
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
                            var token = obj;

                            var colorName = token.Value<string>("color") ?? "";
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
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (!NetworkController.Connected)
            {
                Connect(0);
                buttonConnect.Text = "Disconnect";
            }
            else
            {
                Disconnect();
            }
        }

        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            if (chatTextBox.Text == "") return;
            MainController.HandleChatMessage(chatTextBox.Text);
            chatTextBox.Text = "";
        }

        private void Connect(int delay)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(Connect), delay);
            }
            else
            {
                var username = userNames.SelectedItem;
                if (username == null)
                {
                    Putsc($"Username is empty!{Environment.NewLine}", Color.Black);
                    return;
                }
                MainController.HandleConnect(username.ToString(), Settings.ServerIp, delay);
            }
        }

        private void controlButton_Click(object sender, EventArgs e)
        {
            controlPanel.Visible = !controlPanel.Visible;
        }

        private void Disconnect()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Disconnect));
            }
            else
            {
                NetworkController.Disconnect();
                Putsc($"Client disconected!{Environment.NewLine}", Color.Black);
            }
        }

        private void FormattedItemInfo(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(string)) return;

            var stack = cevent.Value as ItemStack;
            if (stack?.Item == null || stack.Item.Id == -1) return;

            var sb = new StringBuilder();
            sb.AppendLine(stack.Item.ToString());
            sb.AppendLine($"Item count: {stack.ItemCount}");
            sb.AppendLine($"Item damage: {stack.ItemDamage}");
            if (stack.NbtData != null)
            {
                foreach (var tag in stack.NbtRoot.Names)
                {
                    sb.AppendLine($"{tag} = {stack.NbtRoot[tag].StringValue}");
                }
            }
            cevent.Value = sb.ToString();
        }

        private string FormatTileEntity(object value)
        {
            var entity = value as TileEntity;
            if (entity == null) return Empty;

            var sb = new StringBuilder();
            sb.AppendLine(entity.ToString());
            if (entity.Root != null)
                sb.AppendLine(entity.Root.ToString("\t"));
            if (entity.Tags != null)
            {
                foreach (var tag in entity.Tags)
                {
                    sb.AppendLine($"{tag.Key} = {(tag.Value as NbtCompound)?.ToString("\t") ?? tag.Value?.ToString()}");
                }
            }

            return sb.ToString();
        }

        private void InstanceOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Player" && GameController.Player != null)
            {
                healthTextBox.DataBindings.Clear();
                foodTextbox.DataBindings.Clear();
                inventoryGui.DataBindings.Clear();

                healthTextBox.DataBindings.Add("Text", GameController.Player, "Health", true,
                    DataSourceUpdateMode.OnPropertyChanged);
                foodTextbox.DataBindings.Add("Text", GameController.Player, "Food", true,
                    DataSourceUpdateMode.OnPropertyChanged);
                inventoryGui.DataBindings.Add("Container", GameController.Player, "CurrentContainer", true,
                    DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        private void LoadSettings()
        {
            if (userNames.SelectedItem != null)
                Settings.Load(userNames.SelectedItem.ToString());
        }


        private void Putsc(string text, Color color, string style = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color, string>(Putsc), text, color, style);
            }
            else
            {
                if (consoleWindow.TextLength > 65536)
                {
                    consoleWindow.Clear();
                }

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

        private void settingsButton_Click(object sender, EventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void userNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void Viewer_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void Viewer_Load(object sender, EventArgs e)
        {
            var writer = new TextBoxWriter(ConsoleOutput, this);
            Console.SetOut(writer);

            entityList.DataSource = GameController.LivingEntities;
            tileEntitiesList.DataSource = GameController.TileEntities;

            tileEntitiesList.SelectedIndexChanged +=
                (s, args) => tileEntityInfo.Text = FormatTileEntity(tileEntitiesList.SelectedItem);
            inventorySelectMode.CheckedChanged +=
                (s, args) => inventoryGui.SelectMode = inventorySelectMode.Checked;
            var bind = new Binding("Text", inventoryGui, "SelectedItem");
            bind.Format += FormattedItemInfo;
            inventoryItemInfo.DataBindings.Add(bind);

            var users = new XmlDocument();
            try
            {
                users.Load(Settings.UserIdsPath);
                if (users.DocumentElement != null)
                    foreach (XmlNode item in users.DocumentElement)
                    {
                        userNames.Items.Add(item?.Attributes?.GetNamedItem("name").Value ?? "");
                        Settings.Users.Add(item?.Attributes?.GetNamedItem("name").Value ?? "", item?.InnerText);
                    }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                userNames.SelectedItem = userNames.Items.Count > 0 ? userNames.Items[0] : null;
                LoadSettings();
            }
        }

        private class TextBoxWriter : TextWriter
        {
            private readonly StringBuilder builder = new StringBuilder();
            private readonly RichTextBox output;
            private readonly Form owner;
            private readonly Action<char> writeCharFunc;
            private readonly Action<char[], int, int> writeFunc;

            public TextBoxWriter(RichTextBox output, Form owner)
            {
                this.output = output;
                this.owner = owner;
                writeFunc = Write;
                writeCharFunc = Write;
            }

            public override Encoding Encoding => Encoding.UTF8;

            public override void Write(char value)
            {
                if (owner.InvokeRequired)
                    owner.Invoke(writeCharFunc, value);
                else
                {
                    if (value != '\r')
                        try
                        {
                            output.AppendText(value.ToString());
                        }
                        catch (InvalidOperationException)
                        {
                        }
                }
            }

            public override void Write(char[] buffer, int index, int count)
            {
                if (!owner.InvokeRequired)
                {
                    if(output.TextLength > 65536)
                        output.Clear();

                    builder.Clear();
                    builder.Append(buffer);
                    try
                    {
                        output.AppendText(builder.ToString());
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    output.ScrollToCaret();
                }
                else
                {
                    owner.Invoke(writeFunc, buffer, index, count);
                }
            }
        }
    }
}