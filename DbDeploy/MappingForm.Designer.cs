namespace OmniDbDeploy
{
    partial class MappingForm
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
            this.oldColumnList = new System.Windows.Forms.ListBox();
            this.newColumnList = new System.Windows.Forms.ListBox();
            this.mappedColumnList = new System.Windows.Forms.ListBox();
            this.UnmapButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableNameText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // oldColumnList
            // 
            this.oldColumnList.FormattingEnabled = true;
            this.oldColumnList.Location = new System.Drawing.Point(12, 58);
            this.oldColumnList.Name = "oldColumnList";
            this.oldColumnList.Size = new System.Drawing.Size(300, 264);
            this.oldColumnList.TabIndex = 1;
            this.oldColumnList.SelectedIndexChanged += new System.EventHandler(this.oldColumnList_SelectedIndexChanged);
            // 
            // newColumnList
            // 
            this.newColumnList.FormattingEnabled = true;
            this.newColumnList.Location = new System.Drawing.Point(324, 58);
            this.newColumnList.Name = "newColumnList";
            this.newColumnList.Size = new System.Drawing.Size(300, 264);
            this.newColumnList.TabIndex = 2;
            this.newColumnList.SelectedIndexChanged += new System.EventHandler(this.newColumnList_SelectedIndexChanged);
            // 
            // mappedColumnList
            // 
            this.mappedColumnList.FormattingEnabled = true;
            this.mappedColumnList.Location = new System.Drawing.Point(12, 333);
            this.mappedColumnList.Name = "mappedColumnList";
            this.mappedColumnList.Size = new System.Drawing.Size(612, 277);
            this.mappedColumnList.TabIndex = 3;
            this.mappedColumnList.SelectedIndexChanged += new System.EventHandler(this.mappedColumnList_SelectedIndexChanged);
            // 
            // UnmapButton
            // 
            this.UnmapButton.Location = new System.Drawing.Point(12, 614);
            this.UnmapButton.Name = "UnmapButton";
            this.UnmapButton.Size = new System.Drawing.Size(75, 23);
            this.UnmapButton.TabIndex = 4;
            this.UnmapButton.Text = "Unmap";
            this.UnmapButton.UseVisualStyleBackColor = true;
            this.UnmapButton.Click += new System.EventHandler(this.UnmapButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Table:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tableNameText
            // 
            this.tableNameText.Location = new System.Drawing.Point(53, 10);
            this.tableNameText.Name = "tableNameText";
            this.tableNameText.ReadOnly = true;
            this.tableNameText.Size = new System.Drawing.Size(571, 20);
            this.tableNameText.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(240, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Previous columns: (will be dropped if not mapped)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(324, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(292, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "New columns (different than existing): (available for mapping)";
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(549, 614);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 6;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(468, 614);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // MappingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 642);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tableNameText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.UnmapButton);
            this.Controls.Add(this.mappedColumnList);
            this.Controls.Add(this.newColumnList);
            this.Controls.Add(this.oldColumnList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MappingForm";
            this.Text = "Mapping Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ListBox oldColumnList;
        public System.Windows.Forms.ListBox newColumnList;
        public System.Windows.Forms.ListBox mappedColumnList;
        private System.Windows.Forms.Button UnmapButton;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tableNameText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OkButton;

    }
}