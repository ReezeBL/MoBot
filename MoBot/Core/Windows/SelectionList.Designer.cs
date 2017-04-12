namespace MoBot.Core.Windows
{
    partial class SelectionList
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.collection = new System.Windows.Forms.ListBox();
            this.caption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // collection
            // 
            this.collection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.collection.FormattingEnabled = true;
            this.collection.Location = new System.Drawing.Point(0, 35);
            this.collection.Name = "collection";
            this.collection.Size = new System.Drawing.Size(170, 225);
            this.collection.TabIndex = 0;
            // 
            // caption
            // 
            this.caption.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.caption.AutoSize = true;
            this.caption.Location = new System.Drawing.Point(13, 10);
            this.caption.Name = "caption";
            this.caption.Size = new System.Drawing.Size(35, 13);
            this.caption.TabIndex = 1;
            this.caption.Text = "label1";
            // 
            // SelectionList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.caption);
            this.Controls.Add(this.collection);
            this.Name = "SelectionList";
            this.Size = new System.Drawing.Size(170, 260);
            this.Load += new System.EventHandler(this.SelectionList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox collection;
        private System.Windows.Forms.Label caption;
    }
}
