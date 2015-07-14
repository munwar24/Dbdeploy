namespace OmniDbDeploy
{
    partial class DatabaseForm
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
            this.ServerText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DatabaseNameText = new System.Windows.Forms.TextBox();
            this.ConnectionStringText = new System.Windows.Forms.TextBox();
            this.OkayButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ServerText
            // 
            this.ServerText.Location = new System.Drawing.Point(113, 10);
            this.ServerText.Name = "ServerText";
            this.ServerText.Size = new System.Drawing.Size(124, 20);
            this.ServerText.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(19, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Database Name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(19, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Connection String:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // DatabaseNameText
            // 
            this.DatabaseNameText.Location = new System.Drawing.Point(113, 36);
            this.DatabaseNameText.Name = "DatabaseNameText";
            this.DatabaseNameText.Size = new System.Drawing.Size(124, 20);
            this.DatabaseNameText.TabIndex = 1;
            // 
            // ConnectionStringText
            // 
            this.ConnectionStringText.Location = new System.Drawing.Point(113, 62);
            this.ConnectionStringText.Name = "ConnectionStringText";
            this.ConnectionStringText.Size = new System.Drawing.Size(867, 20);
            this.ConnectionStringText.TabIndex = 2;
            // 
            // OkayButton
            // 
            this.OkayButton.Location = new System.Drawing.Point(113, 97);
            this.OkayButton.Name = "OkayButton";
            this.OkayButton.Size = new System.Drawing.Size(75, 23);
            this.OkayButton.TabIndex = 3;
            this.OkayButton.Text = "Ok";
            this.OkayButton.UseVisualStyleBackColor = true;
            this.OkayButton.Click += new System.EventHandler(this.OkayButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(194, 97);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // DatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 138);
            this.ControlBox = false;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.OkayButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ConnectionStringText);
            this.Controls.Add(this.DatabaseNameText);
            this.Controls.Add(this.ServerText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DatabaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Database Connection Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button OkayButton;
        private System.Windows.Forms.Button cancelButton;
        public System.Windows.Forms.TextBox DatabaseNameText;
        public System.Windows.Forms.TextBox ConnectionStringText;
        public System.Windows.Forms.TextBox ServerText;
    }
}