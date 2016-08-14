using System.ComponentModel;
using System.Windows.Forms;

namespace MoBot.Structure.Windows
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
            this.label1 = new System.Windows.Forms.Label();
            this.settingsButton = new System.Windows.Forms.Button();
            this.userNames = new System.Windows.Forms.ComboBox();
            this.ConsoleOutput = new System.Windows.Forms.RichTextBox();
            this.controlButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.foodTextbox = new System.Windows.Forms.TextBox();
            this.labelFood = new System.Windows.Forms.Label();
            this.healthTextBox = new System.Windows.Forms.TextBox();
            this.labelHealth = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.itemPage = new System.Windows.Forms.TabPage();
            this.blockPage = new System.Windows.Forms.TabPage();
            this.entityPage = new System.Windows.Forms.TabPage();
            this.entityList = new System.Windows.Forms.ListBox();
            this.inventoryGui = new MoBot.Structure.Windows.ContainerGui();
            this.inventorySelectMode = new System.Windows.Forms.CheckBox();
            this.inventoryItemInfo = new System.Windows.Forms.RichTextBox();
            this.panel2.SuspendLayout();
            this.controlPanel.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.itemPage.SuspendLayout();
            this.entityPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // consoleWindow
            // 
            this.consoleWindow.BackColor = System.Drawing.SystemColors.GrayText;
            this.consoleWindow.Location = new System.Drawing.Point(3, 4);
            this.consoleWindow.Name = "consoleWindow";
            this.consoleWindow.ReadOnly = true;
            this.consoleWindow.Size = new System.Drawing.Size(428, 240);
            this.consoleWindow.TabIndex = 0;
            this.consoleWindow.Text = "";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(444, 197);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(135, 36);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // chatTextBox
            // 
            this.chatTextBox.Location = new System.Drawing.Point(3, 270);
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.Size = new System.Drawing.Size(428, 20);
            this.chatTextBox.TabIndex = 3;
            // 
            // buttonSendMessage
            // 
            this.buttonSendMessage.Location = new System.Drawing.Point(444, 270);
            this.buttonSendMessage.Name = "buttonSendMessage";
            this.buttonSendMessage.Size = new System.Drawing.Size(135, 20);
            this.buttonSendMessage.TabIndex = 3;
            this.buttonSendMessage.Text = "Send Message";
            this.buttonSendMessage.UseVisualStyleBackColor = true;
            this.buttonSendMessage.Click += new System.EventHandler(this.buttonSendMessage_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(441, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Username";
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(444, 140);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(135, 36);
            this.settingsButton.TabIndex = 7;
            this.settingsButton.Text = "Settings";
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // userNames
            // 
            this.userNames.FormattingEnabled = true;
            this.userNames.Location = new System.Drawing.Point(444, 41);
            this.userNames.Name = "userNames";
            this.userNames.Size = new System.Drawing.Size(135, 21);
            this.userNames.TabIndex = 8;
            // 
            // ConsoleOutput
            // 
            this.ConsoleOutput.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ConsoleOutput.Location = new System.Drawing.Point(12, 327);
            this.ConsoleOutput.Name = "ConsoleOutput";
            this.ConsoleOutput.ReadOnly = true;
            this.ConsoleOutput.Size = new System.Drawing.Size(582, 238);
            this.ConsoleOutput.TabIndex = 9;
            this.ConsoleOutput.Text = "";
            // 
            // controlButton
            // 
            this.controlButton.Location = new System.Drawing.Point(444, 83);
            this.controlButton.Name = "controlButton";
            this.controlButton.Size = new System.Drawing.Size(135, 36);
            this.controlButton.TabIndex = 10;
            this.controlButton.Text = "Control";
            this.controlButton.UseVisualStyleBackColor = true;
            this.controlButton.Click += new System.EventHandler(this.controlButton_Click);
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.controlButton);
            this.panel2.Controls.Add(this.userNames);
            this.panel2.Controls.Add(this.settingsButton);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.buttonSendMessage);
            this.panel2.Controls.Add(this.chatTextBox);
            this.panel2.Controls.Add(this.buttonConnect);
            this.panel2.Controls.Add(this.consoleWindow);
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(582, 293);
            this.panel2.TabIndex = 12;
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.foodTextbox);
            this.controlPanel.Controls.Add(this.labelFood);
            this.controlPanel.Controls.Add(this.healthTextBox);
            this.controlPanel.Controls.Add(this.labelHealth);
            this.controlPanel.Controls.Add(this.tabControl);
            this.controlPanel.Location = new System.Drawing.Point(617, 13);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(468, 552);
            this.controlPanel.TabIndex = 13;
            this.controlPanel.Visible = false;
            // 
            // foodTextbox
            // 
            this.foodTextbox.Location = new System.Drawing.Point(34, 398);
            this.foodTextbox.Name = "foodTextbox";
            this.foodTextbox.ReadOnly = true;
            this.foodTextbox.Size = new System.Drawing.Size(143, 20);
            this.foodTextbox.TabIndex = 4;
            // 
            // labelFood
            // 
            this.labelFood.AutoSize = true;
            this.labelFood.Location = new System.Drawing.Point(28, 378);
            this.labelFood.Name = "labelFood";
            this.labelFood.Size = new System.Drawing.Size(31, 13);
            this.labelFood.TabIndex = 3;
            this.labelFood.Text = "Food";
            // 
            // healthTextBox
            // 
            this.healthTextBox.Location = new System.Drawing.Point(34, 349);
            this.healthTextBox.Name = "healthTextBox";
            this.healthTextBox.ReadOnly = true;
            this.healthTextBox.Size = new System.Drawing.Size(143, 20);
            this.healthTextBox.TabIndex = 2;
            // 
            // labelHealth
            // 
            this.labelHealth.AutoSize = true;
            this.labelHealth.Location = new System.Drawing.Point(28, 329);
            this.labelHealth.Name = "labelHealth";
            this.labelHealth.Size = new System.Drawing.Size(38, 13);
            this.labelHealth.TabIndex = 1;
            this.labelHealth.Text = "Health";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.itemPage);
            this.tabControl.Controls.Add(this.blockPage);
            this.tabControl.Controls.Add(this.entityPage);
            this.tabControl.Location = new System.Drawing.Point(11, 11);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(462, 293);
            this.tabControl.TabIndex = 0;
            // 
            // itemPage
            // 
            this.itemPage.AutoScroll = true;
            this.itemPage.Controls.Add(this.inventoryItemInfo);
            this.itemPage.Controls.Add(this.inventorySelectMode);
            this.itemPage.Controls.Add(this.inventoryGui);
            this.itemPage.Location = new System.Drawing.Point(4, 22);
            this.itemPage.Name = "itemPage";
            this.itemPage.Padding = new System.Windows.Forms.Padding(3);
            this.itemPage.Size = new System.Drawing.Size(454, 267);
            this.itemPage.TabIndex = 0;
            this.itemPage.Text = "Inventory";
            this.itemPage.UseVisualStyleBackColor = true;
            // 
            // blockPage
            // 
            this.blockPage.Location = new System.Drawing.Point(4, 22);
            this.blockPage.Name = "blockPage";
            this.blockPage.Padding = new System.Windows.Forms.Padding(3);
            this.blockPage.Size = new System.Drawing.Size(454, 267);
            this.blockPage.TabIndex = 1;
            this.blockPage.Text = "Blocks";
            this.blockPage.UseVisualStyleBackColor = true;
            // 
            // entityPage
            // 
            this.entityPage.Controls.Add(this.entityList);
            this.entityPage.Location = new System.Drawing.Point(4, 22);
            this.entityPage.Name = "entityPage";
            this.entityPage.Size = new System.Drawing.Size(454, 267);
            this.entityPage.TabIndex = 2;
            this.entityPage.Text = "Entities";
            this.entityPage.UseVisualStyleBackColor = true;
            // 
            // entityList
            // 
            this.entityList.FormattingEnabled = true;
            this.entityList.Location = new System.Drawing.Point(3, 3);
            this.entityList.Name = "entityList";
            this.entityList.Size = new System.Drawing.Size(284, 264);
            this.entityList.TabIndex = 0;
            // 
            // inventoryGui
            // 
            this.inventoryGui.AutoSize = true;
            this.inventoryGui.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.inventoryGui.BackColor = System.Drawing.Color.Transparent;
            this.inventoryGui.Container = null;
            this.inventoryGui.Location = new System.Drawing.Point(3, 3);
            this.inventoryGui.Name = "inventoryGui";
            this.inventoryGui.SelectedItem = null;
            this.inventoryGui.SelectMode = false;
            this.inventoryGui.Size = new System.Drawing.Size(0, 0);
            this.inventoryGui.TabIndex = 0;
            // 
            // inventorySelectMode
            // 
            this.inventorySelectMode.AutoSize = true;
            this.inventorySelectMode.Location = new System.Drawing.Point(316, 78);
            this.inventorySelectMode.Name = "inventorySelectMode";
            this.inventorySelectMode.Size = new System.Drawing.Size(86, 17);
            this.inventorySelectMode.TabIndex = 1;
            this.inventorySelectMode.Text = "Select Mode";
            this.inventorySelectMode.UseVisualStyleBackColor = true;
            // 
            // inventoryItemInfo
            // 
            this.inventoryItemInfo.Location = new System.Drawing.Point(291, 115);
            this.inventoryItemInfo.Name = "inventoryItemInfo";
            this.inventoryItemInfo.Size = new System.Drawing.Size(160, 141);
            this.inventoryItemInfo.TabIndex = 2;
            this.inventoryItemInfo.Text = "";
            // 
            // Viewer
            // 
            this.AcceptButton = this.buttonSendMessage;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1097, 577);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.ConsoleOutput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Viewer";
            this.Text = "MoBot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Viewer_FormClosing);
            this.Load += new System.EventHandler(this.Viewer_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.itemPage.ResumeLayout(false);
            this.itemPage.PerformLayout();
            this.entityPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RichTextBox consoleWindow;
        private Button buttonConnect;
        private TextBox chatTextBox;
        private Button buttonSendMessage;
        private Label label1;
        private Button settingsButton;
        private ComboBox userNames;
        private RichTextBox ConsoleOutput;
        private Button controlButton;
        private Panel panel2;
        private Panel controlPanel;
        private TabControl tabControl;
        private TabPage itemPage;
        private TabPage blockPage;
        private TabPage entityPage;
        private ListBox entityList;
        private Label labelHealth;
        private TextBox healthTextBox;
        private TextBox foodTextbox;
        private Label labelFood;
        private ContainerGui inventoryGui;
        private CheckBox inventorySelectMode;
        private RichTextBox inventoryItemInfo;
    }
}