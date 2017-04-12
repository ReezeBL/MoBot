namespace MoBot.Core.Windows
{
    partial class SettingsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.saveButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.keepItemsSettings = new SelectionList();
            this.autoDiggerSettings = new SelectionList();
            this.label2 = new System.Windows.Forms.Label();
            this.serverTextbox = new System.Windows.Forms.TextBox();
            this.reconnectCheckBox = new System.Windows.Forms.CheckBox();
            this.homeWarp = new System.Windows.Forms.Label();
            this.homeWarpText = new System.Windows.Forms.TextBox();
            this.returnWarp = new System.Windows.Forms.Label();
            this.returnWarpText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(432, 311);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(119, 28);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(307, 311);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(119, 28);
            this.applyButton.TabIndex = 7;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // keepItemsSettings
            // 
            this.keepItemsSettings.GlobalCollection = null;
            this.keepItemsSettings.Items = new object[0];
            this.keepItemsSettings.LabelCaption = "KeepItems";
            this.keepItemsSettings.Location = new System.Drawing.Point(225, 12);
            this.keepItemsSettings.Name = "keepItemsSettings";
            this.keepItemsSettings.Size = new System.Drawing.Size(164, 167);
            this.keepItemsSettings.TabIndex = 6;
            // 
            // autoDiggerSettings
            // 
            this.autoDiggerSettings.GlobalCollection = null;
            this.autoDiggerSettings.Items = new object[0];
            this.autoDiggerSettings.LabelCaption = "AutoDigger blocks";
            this.autoDiggerSettings.Location = new System.Drawing.Point(404, 12);
            this.autoDiggerSettings.Name = "autoDiggerSettings";
            this.autoDiggerSettings.Size = new System.Drawing.Size(147, 167);
            this.autoDiggerSettings.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Server";
            // 
            // serverTextbox
            // 
            this.serverTextbox.Location = new System.Drawing.Point(12, 50);
            this.serverTextbox.Name = "serverTextbox";
            this.serverTextbox.Size = new System.Drawing.Size(130, 20);
            this.serverTextbox.TabIndex = 8;
            // 
            // reconnectCheckBox
            // 
            this.reconnectCheckBox.AutoSize = true;
            this.reconnectCheckBox.Location = new System.Drawing.Point(12, 89);
            this.reconnectCheckBox.Name = "reconnectCheckBox";
            this.reconnectCheckBox.Size = new System.Drawing.Size(99, 17);
            this.reconnectCheckBox.TabIndex = 10;
            this.reconnectCheckBox.Text = "Auto reconnect";
            this.reconnectCheckBox.UseVisualStyleBackColor = true;
            // 
            // homeWarp
            // 
            this.homeWarp.AutoSize = true;
            this.homeWarp.Location = new System.Drawing.Point(18, 138);
            this.homeWarp.Name = "homeWarp";
            this.homeWarp.Size = new System.Drawing.Size(64, 13);
            this.homeWarp.TabIndex = 11;
            this.homeWarp.Text = "Home Warp";
            // 
            // homeWarpText
            // 
            this.homeWarpText.Location = new System.Drawing.Point(16, 167);
            this.homeWarpText.Name = "homeWarpText";
            this.homeWarpText.Size = new System.Drawing.Size(125, 20);
            this.homeWarpText.TabIndex = 12;
            // 
            // returnWarp
            // 
            this.returnWarp.AutoSize = true;
            this.returnWarp.Location = new System.Drawing.Point(18, 211);
            this.returnWarp.Name = "returnWarp";
            this.returnWarp.Size = new System.Drawing.Size(68, 13);
            this.returnWarp.TabIndex = 13;
            this.returnWarp.Text = "Return Warp";
            // 
            // returnWarpText
            // 
            this.returnWarpText.Location = new System.Drawing.Point(17, 244);
            this.returnWarpText.Name = "returnWarpText";
            this.returnWarpText.Size = new System.Drawing.Size(125, 20);
            this.returnWarpText.TabIndex = 14;
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 351);
            this.Controls.Add(this.returnWarpText);
            this.Controls.Add(this.returnWarp);
            this.Controls.Add(this.homeWarpText);
            this.Controls.Add(this.homeWarp);
            this.Controls.Add(this.reconnectCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverTextbox);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.keepItemsSettings);
            this.Controls.Add(this.autoDiggerSettings);
            this.Controls.Add(this.saveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsWindow";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button saveButton;
        private Windows.SelectionList autoDiggerSettings;
        private Windows.SelectionList keepItemsSettings;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serverTextbox;
        private System.Windows.Forms.CheckBox reconnectCheckBox;
        private System.Windows.Forms.Label homeWarp;
        private System.Windows.Forms.TextBox homeWarpText;
        private System.Windows.Forms.Label returnWarp;
        private System.Windows.Forms.TextBox returnWarpText;
    }
}