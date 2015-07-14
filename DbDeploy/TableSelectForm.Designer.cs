namespace OmniDbDeploy
{
    partial class TableSelectForm
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
            this.TableList = new System.Windows.Forms.ListBox();
            this.OkayButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TableList
            // 
            this.TableList.FormattingEnabled = true;
            this.TableList.Location = new System.Drawing.Point(12, 12);
            this.TableList.Name = "TableList";
            this.TableList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.TableList.Size = new System.Drawing.Size(359, 628);
            this.TableList.TabIndex = 0;
            // 
            // OkayButton
            // 
            this.OkayButton.Location = new System.Drawing.Point(378, 587);
            this.OkayButton.Name = "OkayButton";
            this.OkayButton.Size = new System.Drawing.Size(75, 23);
            this.OkayButton.TabIndex = 1;
            this.OkayButton.Text = "OK";
            this.OkayButton.UseVisualStyleBackColor = true;
            this.OkayButton.Click += new System.EventHandler(this.OkayButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(378, 616);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // TableSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 652);
            this.ControlBox = false;
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkayButton);
            this.Controls.Add(this.TableList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TableSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Table Selector";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListBox TableList;
        private System.Windows.Forms.Button OkayButton;
        private System.Windows.Forms.Button CancelButton;
    }
}