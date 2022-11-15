namespace Plugin {
    partial class Update {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.updateLabel = new System.Windows.Forms.Label();
            this.DLLabel = new System.Windows.Forms.LinkLabel();
            this.OKButton = new System.Windows.Forms.Button();
            this.IgnoreUpdateCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // updateLabel
            // 
            this.updateLabel.AutoSize = true;
            this.updateLabel.Location = new System.Drawing.Point(4, 9);
            this.updateLabel.Name = "updateLabel";
            this.updateLabel.Size = new System.Drawing.Size(295, 13);
            this.updateLabel.TabIndex = 0;
            this.updateLabel.Text = "The latest version of LRV P4 is 0.0.0 released on 00-00-0000";
            // 
            // DLLabel
            // 
            this.DLLabel.AutoSize = true;
            this.DLLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.DLLabel.Location = new System.Drawing.Point(5, 31);
            this.DLLabel.Name = "DLLabel";
            this.DLLabel.Size = new System.Drawing.Size(119, 13);
            this.DLLabel.TabIndex = 1;
            this.DLLabel.TabStop = true;
            this.DLLabel.Text = "Click Here to Download";
            this.DLLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DLLabel_LinkClicked);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(229, 72);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // IgnoreUpdateCheckbox
            // 
            this.IgnoreUpdateCheckbox.AutoSize = true;
            this.IgnoreUpdateCheckbox.Location = new System.Drawing.Point(7, 58);
            this.IgnoreUpdateCheckbox.Name = "IgnoreUpdateCheckbox";
            this.IgnoreUpdateCheckbox.Size = new System.Drawing.Size(174, 17);
            this.IgnoreUpdateCheckbox.TabIndex = 3;
            this.IgnoreUpdateCheckbox.Text = "Don\'t show update in the future";
            this.IgnoreUpdateCheckbox.UseVisualStyleBackColor = true;
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 104);
            this.Controls.Add(this.IgnoreUpdateCheckbox);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.DLLabel);
            this.Controls.Add(this.updateLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Update";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update";
            this.Load += new System.EventHandler(this.Update_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label updateLabel;
        private System.Windows.Forms.LinkLabel DLLabel;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.CheckBox IgnoreUpdateCheckbox;
    }
}