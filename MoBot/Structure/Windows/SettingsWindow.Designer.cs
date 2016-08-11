namespace MoBot.Structure.Windows
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
            this.autoDiggerSettings = new MoBot.Structure.Windows.SelectionList();
            this.keepItemsSettings = new MoBot.Structure.Windows.SelectionList();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(433, 315);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(119, 28);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // autoDiggerSettings
            // 
            this.autoDiggerSettings.LabelCaption = "AutoDigger blocks";
            this.autoDiggerSettings.Location = new System.Drawing.Point(12, 12);
            this.autoDiggerSettings.Name = "autoDiggerSettings";
            this.autoDiggerSettings.Size = new System.Drawing.Size(147, 167);
            this.autoDiggerSettings.TabIndex = 5;
            // 
            // keepItemsSettings
            // 
            this.keepItemsSettings.LabelCaption = "KeepItems";
            this.keepItemsSettings.Location = new System.Drawing.Point(21, 185);
            this.keepItemsSettings.Name = "keepItemsSettings";
            this.keepItemsSettings.Size = new System.Drawing.Size(138, 145);
            this.keepItemsSettings.TabIndex = 6;
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 351);
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

        }

        #endregion
        private System.Windows.Forms.Button saveButton;
        private Windows.SelectionList autoDiggerSettings;
        private Windows.SelectionList keepItemsSettings;
    }
}