namespace OmniDbDeploy
{
    partial class DbDeployForm
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
            this.GoButton = new System.Windows.Forms.Button();
            this.fullLogText = new System.Windows.Forms.RichTextBox();
            this.statusFooter = new System.Windows.Forms.StatusStrip();
            this.phaseLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.rowProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.targetList = new System.Windows.Forms.ListBox();
            this.configurationList = new System.Windows.Forms.ListBox();
            this.sourceList = new System.Windows.Forms.ListBox();
            this.sourceAddButton = new System.Windows.Forms.Button();
            this.sourceDeleteButton = new System.Windows.Forms.Button();
            this.TargetAddButton = new System.Windows.Forms.Button();
            this.TargetDeleteButton = new System.Windows.Forms.Button();
            this.configAddButton = new System.Windows.Forms.Button();
            this.configDeleteButton = new System.Windows.Forms.Button();
            this.sourceEditButton = new System.Windows.Forms.Button();
            this.targetEditButton = new System.Windows.Forms.Button();
            this.configEditButton = new System.Windows.Forms.Button();
            this.logTabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.changeLogText = new System.Windows.Forms.RichTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.warningLogText = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.errorLogText = new System.Windows.Forms.RichTextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.ddlLogText = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.statusFooter.SuspendLayout();
            this.logTabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // GoButton
            // 
            this.GoButton.Enabled = false;
            this.GoButton.Location = new System.Drawing.Point(932, 116);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(54, 23);
            this.GoButton.TabIndex = 9;
            this.GoButton.Text = "Deploy";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // fullLogText
            // 
            this.fullLogText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fullLogText.BackColor = System.Drawing.SystemColors.Window;
            this.fullLogText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fullLogText.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fullLogText.Location = new System.Drawing.Point(-1, 0);
            this.fullLogText.Name = "fullLogText";
            this.fullLogText.ReadOnly = true;
            this.fullLogText.Size = new System.Drawing.Size(970, 497);
            this.fullLogText.TabIndex = 13;
            this.fullLogText.Text = "";
            // 
            // statusFooter
            // 
            this.statusFooter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.phaseLabel,
            this.statusLabel,
            this.tableProgressBar,
            this.rowProgressBar,
            this.toolStripStatusLabel1});
            this.statusFooter.Location = new System.Drawing.Point(0, 681);
            this.statusFooter.Name = "statusFooter";
            this.statusFooter.Size = new System.Drawing.Size(992, 22);
            this.statusFooter.TabIndex = 2;
            this.statusFooter.Text = "statusStrip1";
            // 
            // phaseLabel
            // 
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(50, 17);
            this.phaseLabel.Text = "Startup";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(922, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Starting...";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableProgressBar
            // 
            this.tableProgressBar.Name = "tableProgressBar";
            this.tableProgressBar.Size = new System.Drawing.Size(100, 16);
            this.tableProgressBar.Visible = false;
            // 
            // rowProgressBar
            // 
            this.rowProgressBar.Name = "rowProgressBar";
            this.rowProgressBar.Size = new System.Drawing.Size(100, 16);
            this.rowProgressBar.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(5, 17);
            // 
            // targetList
            // 
            this.targetList.FormattingEnabled = true;
            this.targetList.Location = new System.Drawing.Point(6, 19);
            this.targetList.Name = "targetList";
            this.targetList.Size = new System.Drawing.Size(240, 108);
            this.targetList.Sorted = true;
            this.targetList.TabIndex = 4;
            this.targetList.SelectedIndexChanged += new System.EventHandler(this.targetList_SelectedIndexChanged);
            // 
            // configurationList
            // 
            this.configurationList.FormattingEnabled = true;
            this.configurationList.Location = new System.Drawing.Point(6, 19);
            this.configurationList.Name = "configurationList";
            this.configurationList.Size = new System.Drawing.Size(240, 108);
            this.configurationList.Sorted = true;
            this.configurationList.TabIndex = 8;
            this.configurationList.SelectedIndexChanged += new System.EventHandler(this.configurationList_SelectedIndexChanged);
            // 
            // sourceList
            // 
            this.sourceList.FormattingEnabled = true;
            this.sourceList.Location = new System.Drawing.Point(6, 19);
            this.sourceList.Name = "sourceList";
            this.sourceList.Size = new System.Drawing.Size(240, 108);
            this.sourceList.Sorted = true;
            this.sourceList.TabIndex = 0;
            this.sourceList.SelectedIndexChanged += new System.EventHandler(this.sourceList_SelectedIndexChanged);
            // 
            // sourceAddButton
            // 
            this.sourceAddButton.Location = new System.Drawing.Point(252, 46);
            this.sourceAddButton.Name = "sourceAddButton";
            this.sourceAddButton.Size = new System.Drawing.Size(42, 23);
            this.sourceAddButton.TabIndex = 1;
            this.sourceAddButton.Text = "Add";
            this.sourceAddButton.UseVisualStyleBackColor = true;
            this.sourceAddButton.Click += new System.EventHandler(this.sourceAddButton_Click);
            // 
            // sourceDeleteButton
            // 
            this.sourceDeleteButton.Enabled = false;
            this.sourceDeleteButton.Location = new System.Drawing.Point(252, 104);
            this.sourceDeleteButton.Name = "sourceDeleteButton";
            this.sourceDeleteButton.Size = new System.Drawing.Size(42, 23);
            this.sourceDeleteButton.TabIndex = 3;
            this.sourceDeleteButton.Text = "Del";
            this.sourceDeleteButton.UseVisualStyleBackColor = true;
            this.sourceDeleteButton.Click += new System.EventHandler(this.sourceDeleteButton_Click);
            // 
            // TargetAddButton
            // 
            this.TargetAddButton.Enabled = false;
            this.TargetAddButton.Location = new System.Drawing.Point(252, 46);
            this.TargetAddButton.Name = "TargetAddButton";
            this.TargetAddButton.Size = new System.Drawing.Size(42, 23);
            this.TargetAddButton.TabIndex = 5;
            this.TargetAddButton.Text = "Add";
            this.TargetAddButton.UseVisualStyleBackColor = true;
            this.TargetAddButton.Click += new System.EventHandler(this.TargetAddButton_Click);
            // 
            // TargetDeleteButton
            // 
            this.TargetDeleteButton.Enabled = false;
            this.TargetDeleteButton.Location = new System.Drawing.Point(252, 104);
            this.TargetDeleteButton.Name = "TargetDeleteButton";
            this.TargetDeleteButton.Size = new System.Drawing.Size(42, 23);
            this.TargetDeleteButton.TabIndex = 7;
            this.TargetDeleteButton.Text = "Del";
            this.TargetDeleteButton.UseVisualStyleBackColor = true;
            this.TargetDeleteButton.Click += new System.EventHandler(this.TargetDeleteButton_Click);
            // 
            // configAddButton
            // 
            this.configAddButton.Enabled = false;
            this.configAddButton.Location = new System.Drawing.Point(252, 46);
            this.configAddButton.Name = "configAddButton";
            this.configAddButton.Size = new System.Drawing.Size(42, 23);
            this.configAddButton.TabIndex = 10;
            this.configAddButton.Text = "Add";
            this.configAddButton.UseVisualStyleBackColor = true;
            this.configAddButton.Click += new System.EventHandler(this.configAddButton_Click);
            // 
            // configDeleteButton
            // 
            this.configDeleteButton.Enabled = false;
            this.configDeleteButton.Location = new System.Drawing.Point(252, 104);
            this.configDeleteButton.Name = "configDeleteButton";
            this.configDeleteButton.Size = new System.Drawing.Size(42, 23);
            this.configDeleteButton.TabIndex = 12;
            this.configDeleteButton.Text = "Del";
            this.configDeleteButton.UseVisualStyleBackColor = true;
            this.configDeleteButton.Click += new System.EventHandler(this.configDeleteButton_Click);
            // 
            // sourceEditButton
            // 
            this.sourceEditButton.Enabled = false;
            this.sourceEditButton.Location = new System.Drawing.Point(252, 75);
            this.sourceEditButton.Name = "sourceEditButton";
            this.sourceEditButton.Size = new System.Drawing.Size(42, 23);
            this.sourceEditButton.TabIndex = 2;
            this.sourceEditButton.Text = "Edit";
            this.sourceEditButton.UseVisualStyleBackColor = true;
            this.sourceEditButton.Click += new System.EventHandler(this.sourceEditButton_Click);
            // 
            // targetEditButton
            // 
            this.targetEditButton.Enabled = false;
            this.targetEditButton.Location = new System.Drawing.Point(252, 75);
            this.targetEditButton.Name = "targetEditButton";
            this.targetEditButton.Size = new System.Drawing.Size(42, 23);
            this.targetEditButton.TabIndex = 6;
            this.targetEditButton.Text = "Edit";
            this.targetEditButton.UseVisualStyleBackColor = true;
            this.targetEditButton.Click += new System.EventHandler(this.targetEditButton_Click);
            // 
            // configEditButton
            // 
            this.configEditButton.Enabled = false;
            this.configEditButton.Location = new System.Drawing.Point(252, 75);
            this.configEditButton.Name = "configEditButton";
            this.configEditButton.Size = new System.Drawing.Size(42, 23);
            this.configEditButton.TabIndex = 11;
            this.configEditButton.Text = "Edit";
            this.configEditButton.UseVisualStyleBackColor = true;
            this.configEditButton.Click += new System.EventHandler(this.configEditButton_Click);
            // 
            // logTabs
            // 
            this.logTabs.Controls.Add(this.tabPage1);
            this.logTabs.Controls.Add(this.tabPage4);
            this.logTabs.Controls.Add(this.tabPage3);
            this.logTabs.Controls.Add(this.tabPage2);
            this.logTabs.Controls.Add(this.tabPage5);
            this.logTabs.Location = new System.Drawing.Point(8, 151);
            this.logTabs.Name = "logTabs";
            this.logTabs.SelectedIndex = 0;
            this.logTabs.Size = new System.Drawing.Size(978, 523);
            this.logTabs.TabIndex = 14;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.fullLogText);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(970, 497);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Full Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.changeLogText);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(970, 497);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Change Log";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // changeLogText
            // 
            this.changeLogText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.changeLogText.BackColor = System.Drawing.SystemColors.Window;
            this.changeLogText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.changeLogText.Location = new System.Drawing.Point(0, 0);
            this.changeLogText.Name = "changeLogText";
            this.changeLogText.ReadOnly = true;
            this.changeLogText.Size = new System.Drawing.Size(964, 508);
            this.changeLogText.TabIndex = 0;
            this.changeLogText.Text = "";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.warningLogText);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(970, 497);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Warning Log";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // warningLogText
            // 
            this.warningLogText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.warningLogText.BackColor = System.Drawing.SystemColors.Window;
            this.warningLogText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.warningLogText.Location = new System.Drawing.Point(0, 0);
            this.warningLogText.Name = "warningLogText";
            this.warningLogText.ReadOnly = true;
            this.warningLogText.Size = new System.Drawing.Size(964, 508);
            this.warningLogText.TabIndex = 0;
            this.warningLogText.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.errorLogText);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(970, 497);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Error Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // errorLogText
            // 
            this.errorLogText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.errorLogText.BackColor = System.Drawing.SystemColors.Window;
            this.errorLogText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.errorLogText.Location = new System.Drawing.Point(0, 0);
            this.errorLogText.Name = "errorLogText";
            this.errorLogText.ReadOnly = true;
            this.errorLogText.Size = new System.Drawing.Size(964, 508);
            this.errorLogText.TabIndex = 0;
            this.errorLogText.Text = "";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.ddlLogText);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(970, 497);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "DDL";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // ddlLogText
            // 
            this.ddlLogText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlLogText.BackColor = System.Drawing.SystemColors.Window;
            this.ddlLogText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ddlLogText.Location = new System.Drawing.Point(0, 0);
            this.ddlLogText.Name = "ddlLogText";
            this.ddlLogText.ReadOnly = true;
            this.ddlLogText.Size = new System.Drawing.Size(964, 508);
            this.ddlLogText.TabIndex = 0;
            this.ddlLogText.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sourceList);
            this.groupBox1.Controls.Add(this.sourceAddButton);
            this.groupBox1.Controls.Add(this.sourceEditButton);
            this.groupBox1.Controls.Add(this.sourceDeleteButton);
            this.groupBox1.Location = new System.Drawing.Point(8, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 133);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sources";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.targetList);
            this.groupBox2.Controls.Add(this.targetEditButton);
            this.groupBox2.Controls.Add(this.TargetAddButton);
            this.groupBox2.Controls.Add(this.TargetDeleteButton);
            this.groupBox2.Location = new System.Drawing.Point(624, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 133);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Targets";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.configurationList);
            this.groupBox3.Controls.Add(this.configEditButton);
            this.groupBox3.Controls.Add(this.configAddButton);
            this.groupBox3.Controls.Add(this.configDeleteButton);
            this.groupBox3.Location = new System.Drawing.Point(316, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(302, 133);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Configurations";
            // 
            // DbDeployForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 703);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.logTabs);
            this.Controls.Add(this.statusFooter);
            this.Name = "DbDeployForm";
            this.Text = "DB Deploy Utility";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusFooter.ResumeLayout(false);
            this.statusFooter.PerformLayout();
            this.logTabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.RichTextBox fullLogText;
        private System.Windows.Forms.StatusStrip statusFooter;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel phaseLabel;
        private System.Windows.Forms.ToolStripProgressBar rowProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar tableProgressBar;
        private System.Windows.Forms.ListBox targetList;
        private System.Windows.Forms.ListBox sourceList;
        private System.Windows.Forms.Button sourceAddButton;
        private System.Windows.Forms.Button sourceDeleteButton;
        private System.Windows.Forms.Button TargetAddButton;
        private System.Windows.Forms.Button TargetDeleteButton;
        private System.Windows.Forms.Button configAddButton;
        private System.Windows.Forms.Button configDeleteButton;
        private System.Windows.Forms.Button sourceEditButton;
        private System.Windows.Forms.Button targetEditButton;
        private System.Windows.Forms.Button configEditButton;
        public System.Windows.Forms.ListBox configurationList;
        private System.Windows.Forms.TabControl logTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RichTextBox errorLogText;
        private System.Windows.Forms.RichTextBox warningLogText;
        private System.Windows.Forms.RichTextBox changeLogText;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.RichTextBox ddlLogText;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

