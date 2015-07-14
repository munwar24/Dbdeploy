namespace OmniDbDeploy
{
    partial class ConfigurationForm
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
            this.TitleText = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DeployTypeRadioLogOnly = new System.Windows.Forms.RadioButton();
            this.DeployTypeRadioTransfer = new System.Windows.Forms.RadioButton();
            this.DeployTypeRadioAlter = new System.Windows.Forms.RadioButton();
            this.DeployTypeRadioDropCreate = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.excludeSchemaTempCheck = new System.Windows.Forms.CheckBox();
            this.SchemaSelectRadioExclude = new System.Windows.Forms.RadioButton();
            this.SchemaSelectRadioInclude = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.excludeDataTempCheck = new System.Windows.Forms.CheckBox();
            this.DataSelectRadioExclude = new System.Windows.Forms.RadioButton();
            this.DataSelectRadioInclude = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.maxRowsText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DataTransferCheckTypes = new System.Windows.Forms.CheckBox();
            this.DataTransferCheckStatic = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.ddlLoggingRadioAll = new System.Windows.Forms.RadioButton();
            this.ddlLoggingRadioChanges = new System.Windows.Forms.RadioButton();
            this.ddlLoggingRadioNone = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.SchemaTablesEditButton = new System.Windows.Forms.Button();
            this.SchemaTableList = new System.Windows.Forms.CheckedListBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.DataTablesEditButton = new System.Windows.Forms.Button();
            this.DataTableList = new System.Windows.Forms.CheckedListBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OkayButton = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.backupLocationText = new System.Windows.Forms.TextBox();
            this.backupPerformCheck = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.FilterClauseText = new System.Windows.Forms.TextBox();
            this.FilterTablesEditButton = new System.Windows.Forms.Button();
            this.FilterTableList = new System.Windows.Forms.CheckedListBox();
            this.groupBoxFK = new System.Windows.Forms.GroupBox();
            this.SyncFKCheck = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBoxFK.SuspendLayout();
            this.SuspendLayout();
            // 
            // TitleText
            // 
            this.TitleText.Location = new System.Drawing.Point(6, 19);
            this.TitleText.Name = "TitleText";
            this.TitleText.Size = new System.Drawing.Size(339, 20);
            this.TitleText.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DeployTypeRadioLogOnly);
            this.groupBox1.Controls.Add(this.DeployTypeRadioTransfer);
            this.groupBox1.Controls.Add(this.DeployTypeRadioAlter);
            this.groupBox1.Controls.Add(this.DeployTypeRadioDropCreate);
            this.groupBox1.Location = new System.Drawing.Point(369, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(129, 101);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Deployment Type";
            // 
            // DeployTypeRadioLogOnly
            // 
            this.DeployTypeRadioLogOnly.AutoSize = true;
            this.DeployTypeRadioLogOnly.Location = new System.Drawing.Point(7, 77);
            this.DeployTypeRadioLogOnly.Name = "DeployTypeRadioLogOnly";
            this.DeployTypeRadioLogOnly.Size = new System.Drawing.Size(67, 17);
            this.DeployTypeRadioLogOnly.TabIndex = 3;
            this.DeployTypeRadioLogOnly.TabStop = true;
            this.DeployTypeRadioLogOnly.Text = "Log Only";
            this.DeployTypeRadioLogOnly.UseVisualStyleBackColor = true;
            // 
            // DeployTypeRadioTransfer
            // 
            this.DeployTypeRadioTransfer.AutoSize = true;
            this.DeployTypeRadioTransfer.Location = new System.Drawing.Point(7, 56);
            this.DeployTypeRadioTransfer.Name = "DeployTypeRadioTransfer";
            this.DeployTypeRadioTransfer.Size = new System.Drawing.Size(114, 17);
            this.DeployTypeRadioTransfer.TabIndex = 2;
            this.DeployTypeRadioTransfer.TabStop = true;
            this.DeployTypeRadioTransfer.Text = "Transfer Data Only";
            this.DeployTypeRadioTransfer.UseVisualStyleBackColor = true;
            // 
            // DeployTypeRadioAlter
            // 
            this.DeployTypeRadioAlter.AutoSize = true;
            this.DeployTypeRadioAlter.Checked = true;
            this.DeployTypeRadioAlter.Location = new System.Drawing.Point(7, 35);
            this.DeployTypeRadioAlter.Name = "DeployTypeRadioAlter";
            this.DeployTypeRadioAlter.Size = new System.Drawing.Size(46, 17);
            this.DeployTypeRadioAlter.TabIndex = 1;
            this.DeployTypeRadioAlter.TabStop = true;
            this.DeployTypeRadioAlter.Text = "Alter";
            this.DeployTypeRadioAlter.UseVisualStyleBackColor = true;
            // 
            // DeployTypeRadioDropCreate
            // 
            this.DeployTypeRadioDropCreate.AutoSize = true;
            this.DeployTypeRadioDropCreate.Location = new System.Drawing.Point(7, 14);
            this.DeployTypeRadioDropCreate.Name = "DeployTypeRadioDropCreate";
            this.DeployTypeRadioDropCreate.Size = new System.Drawing.Size(91, 17);
            this.DeployTypeRadioDropCreate.TabIndex = 0;
            this.DeployTypeRadioDropCreate.Text = "Drop && Create";
            this.DeployTypeRadioDropCreate.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.excludeSchemaTempCheck);
            this.groupBox2.Controls.Add(this.SchemaSelectRadioExclude);
            this.groupBox2.Controls.Add(this.SchemaSelectRadioInclude);
            this.groupBox2.Location = new System.Drawing.Point(12, 166);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(344, 92);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Schema Table Select Type";
            // 
            // excludeSchemaTempCheck
            // 
            this.excludeSchemaTempCheck.AutoSize = true;
            this.excludeSchemaTempCheck.Checked = true;
            this.excludeSchemaTempCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.excludeSchemaTempCheck.Location = new System.Drawing.Point(7, 67);
            this.excludeSchemaTempCheck.Name = "excludeSchemaTempCheck";
            this.excludeSchemaTempCheck.Size = new System.Drawing.Size(334, 17);
            this.excludeSchemaTempCheck.TabIndex = 2;
            this.excludeSchemaTempCheck.Text = "Exclude all tables starting with \"TEMP_\" or ending with \"_TEMP\"";
            this.excludeSchemaTempCheck.UseVisualStyleBackColor = true;
            // 
            // SchemaSelectRadioExclude
            // 
            this.SchemaSelectRadioExclude.AutoSize = true;
            this.SchemaSelectRadioExclude.Location = new System.Drawing.Point(7, 44);
            this.SchemaSelectRadioExclude.Name = "SchemaSelectRadioExclude";
            this.SchemaSelectRadioExclude.Size = new System.Drawing.Size(334, 17);
            this.SchemaSelectRadioExclude.TabIndex = 1;
            this.SchemaSelectRadioExclude.Text = "Exclude all tables/views by default, include tables/views selected";
            this.SchemaSelectRadioExclude.UseVisualStyleBackColor = true;
            // 
            // SchemaSelectRadioInclude
            // 
            this.SchemaSelectRadioInclude.AutoSize = true;
            this.SchemaSelectRadioInclude.Checked = true;
            this.SchemaSelectRadioInclude.Location = new System.Drawing.Point(7, 20);
            this.SchemaSelectRadioInclude.Name = "SchemaSelectRadioInclude";
            this.SchemaSelectRadioInclude.Size = new System.Drawing.Size(334, 17);
            this.SchemaSelectRadioInclude.TabIndex = 0;
            this.SchemaSelectRadioInclude.TabStop = true;
            this.SchemaSelectRadioInclude.Text = "Include all tables/views by default, exclude tables/views selected";
            this.SchemaSelectRadioInclude.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.excludeDataTempCheck);
            this.groupBox3.Controls.Add(this.DataSelectRadioExclude);
            this.groupBox3.Controls.Add(this.DataSelectRadioInclude);
            this.groupBox3.Location = new System.Drawing.Point(362, 166);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 92);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Data Table Select Type";
            // 
            // excludeDataTempCheck
            // 
            this.excludeDataTempCheck.AutoSize = true;
            this.excludeDataTempCheck.Checked = true;
            this.excludeDataTempCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.excludeDataTempCheck.Location = new System.Drawing.Point(7, 67);
            this.excludeDataTempCheck.Name = "excludeDataTempCheck";
            this.excludeDataTempCheck.Size = new System.Drawing.Size(334, 17);
            this.excludeDataTempCheck.TabIndex = 2;
            this.excludeDataTempCheck.Text = "Exclude all tables starting with \"TEMP_\" or ending with \"_TEMP\"";
            this.excludeDataTempCheck.UseVisualStyleBackColor = true;
            // 
            // DataSelectRadioExclude
            // 
            this.DataSelectRadioExclude.AutoSize = true;
            this.DataSelectRadioExclude.Checked = true;
            this.DataSelectRadioExclude.Location = new System.Drawing.Point(7, 44);
            this.DataSelectRadioExclude.Name = "DataSelectRadioExclude";
            this.DataSelectRadioExclude.Size = new System.Drawing.Size(334, 17);
            this.DataSelectRadioExclude.TabIndex = 1;
            this.DataSelectRadioExclude.TabStop = true;
            this.DataSelectRadioExclude.Text = "Exclude all tables/views by default, include tables/views selected";
            this.DataSelectRadioExclude.UseVisualStyleBackColor = true;
            // 
            // DataSelectRadioInclude
            // 
            this.DataSelectRadioInclude.AutoSize = true;
            this.DataSelectRadioInclude.Location = new System.Drawing.Point(7, 20);
            this.DataSelectRadioInclude.Name = "DataSelectRadioInclude";
            this.DataSelectRadioInclude.Size = new System.Drawing.Size(334, 17);
            this.DataSelectRadioInclude.TabIndex = 0;
            this.DataSelectRadioInclude.Text = "Include all tables/views by default, exclude tables/views selected";
            this.DataSelectRadioInclude.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.maxRowsText);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.DataTransferCheckTypes);
            this.groupBox4.Controls.Add(this.DataTransferCheckStatic);
            this.groupBox4.Location = new System.Drawing.Point(504, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(202, 101);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Automatic Data Transfer";
            // 
            // maxRowsText
            // 
            this.maxRowsText.Location = new System.Drawing.Point(73, 64);
            this.maxRowsText.Name = "maxRowsText";
            this.maxRowsText.Size = new System.Drawing.Size(106, 20);
            this.maxRowsText.TabIndex = 3;
            this.maxRowsText.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Max Rows:";
            // 
            // DataTransferCheckTypes
            // 
            this.DataTransferCheckTypes.AutoSize = true;
            this.DataTransferCheckTypes.Checked = true;
            this.DataTransferCheckTypes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DataTransferCheckTypes.Location = new System.Drawing.Point(7, 43);
            this.DataTransferCheckTypes.Name = "DataTransferCheckTypes";
            this.DataTransferCheckTypes.Size = new System.Drawing.Size(179, 17);
            this.DataTransferCheckTypes.TabIndex = 1;
            this.DataTransferCheckTypes.Text = "All tables ending with \"_TYPES\"";
            this.DataTransferCheckTypes.UseVisualStyleBackColor = true;
            // 
            // DataTransferCheckStatic
            // 
            this.DataTransferCheckStatic.AutoSize = true;
            this.DataTransferCheckStatic.Checked = true;
            this.DataTransferCheckStatic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DataTransferCheckStatic.Location = new System.Drawing.Point(7, 20);
            this.DataTransferCheckStatic.Name = "DataTransferCheckStatic";
            this.DataTransferCheckStatic.Size = new System.Drawing.Size(182, 17);
            this.DataTransferCheckStatic.TabIndex = 0;
            this.DataTransferCheckStatic.Text = "All tables ending with \"_STATIC\"";
            this.DataTransferCheckStatic.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.ddlLoggingRadioAll);
            this.groupBox5.Controls.Add(this.ddlLoggingRadioChanges);
            this.groupBox5.Controls.Add(this.ddlLoggingRadioNone);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.TitleText);
            this.groupBox5.Location = new System.Drawing.Point(12, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(351, 101);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Title";
            // 
            // ddlLoggingRadioAll
            // 
            this.ddlLoggingRadioAll.AutoSize = true;
            this.ddlLoggingRadioAll.Location = new System.Drawing.Point(216, 45);
            this.ddlLoggingRadioAll.Name = "ddlLoggingRadioAll";
            this.ddlLoggingRadioAll.Size = new System.Drawing.Size(36, 17);
            this.ddlLoggingRadioAll.TabIndex = 4;
            this.ddlLoggingRadioAll.Text = "All";
            this.ddlLoggingRadioAll.UseVisualStyleBackColor = true;
            // 
            // ddlLoggingRadioChanges
            // 
            this.ddlLoggingRadioChanges.AutoSize = true;
            this.ddlLoggingRadioChanges.Checked = true;
            this.ddlLoggingRadioChanges.Location = new System.Drawing.Point(142, 45);
            this.ddlLoggingRadioChanges.Name = "ddlLoggingRadioChanges";
            this.ddlLoggingRadioChanges.Size = new System.Drawing.Size(67, 17);
            this.ddlLoggingRadioChanges.TabIndex = 3;
            this.ddlLoggingRadioChanges.TabStop = true;
            this.ddlLoggingRadioChanges.Text = "Changes";
            this.ddlLoggingRadioChanges.UseVisualStyleBackColor = true;
            // 
            // ddlLoggingRadioNone
            // 
            this.ddlLoggingRadioNone.AutoSize = true;
            this.ddlLoggingRadioNone.Location = new System.Drawing.Point(84, 45);
            this.ddlLoggingRadioNone.Name = "ddlLoggingRadioNone";
            this.ddlLoggingRadioNone.Size = new System.Drawing.Size(51, 17);
            this.ddlLoggingRadioNone.TabIndex = 2;
            this.ddlLoggingRadioNone.Text = "None";
            this.ddlLoggingRadioNone.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "DDL Logging:";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.SchemaTablesEditButton);
            this.groupBox6.Controls.Add(this.SchemaTableList);
            this.groupBox6.Location = new System.Drawing.Point(12, 264);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(344, 437);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Schema Tables";
            // 
            // SchemaTablesEditButton
            // 
            this.SchemaTablesEditButton.Location = new System.Drawing.Point(263, 405);
            this.SchemaTablesEditButton.Name = "SchemaTablesEditButton";
            this.SchemaTablesEditButton.Size = new System.Drawing.Size(75, 23);
            this.SchemaTablesEditButton.TabIndex = 1;
            this.SchemaTablesEditButton.Text = "Add/Delete";
            this.SchemaTablesEditButton.UseVisualStyleBackColor = true;
            this.SchemaTablesEditButton.Click += new System.EventHandler(this.SchemaTablesEditButton_Click);
            // 
            // SchemaTableList
            // 
            this.SchemaTableList.CheckOnClick = true;
            this.SchemaTableList.FormattingEnabled = true;
            this.SchemaTableList.Location = new System.Drawing.Point(7, 20);
            this.SchemaTableList.Name = "SchemaTableList";
            this.SchemaTableList.Size = new System.Drawing.Size(331, 379);
            this.SchemaTableList.Sorted = true;
            this.SchemaTableList.TabIndex = 0;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.DataTablesEditButton);
            this.groupBox7.Controls.Add(this.DataTableList);
            this.groupBox7.Location = new System.Drawing.Point(359, 264);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(347, 437);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Data Tables";
            // 
            // DataTablesEditButton
            // 
            this.DataTablesEditButton.Location = new System.Drawing.Point(266, 405);
            this.DataTablesEditButton.Name = "DataTablesEditButton";
            this.DataTablesEditButton.Size = new System.Drawing.Size(75, 23);
            this.DataTablesEditButton.TabIndex = 1;
            this.DataTablesEditButton.Text = "Add/Delete";
            this.DataTablesEditButton.UseVisualStyleBackColor = true;
            this.DataTablesEditButton.Click += new System.EventHandler(this.DataTablesEditButton_Click);
            // 
            // DataTableList
            // 
            this.DataTableList.CheckOnClick = true;
            this.DataTableList.FormattingEnabled = true;
            this.DataTableList.Location = new System.Drawing.Point(7, 20);
            this.DataTableList.Name = "DataTableList";
            this.DataTableList.Size = new System.Drawing.Size(334, 379);
            this.DataTableList.Sorted = true;
            this.DataTableList.TabIndex = 0;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(931, 686);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 8;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OkayButton
            // 
            this.OkayButton.Location = new System.Drawing.Point(850, 686);
            this.OkayButton.Name = "OkayButton";
            this.OkayButton.Size = new System.Drawing.Size(75, 23);
            this.OkayButton.TabIndex = 7;
            this.OkayButton.Text = "Ok";
            this.OkayButton.UseVisualStyleBackColor = true;
            this.OkayButton.Click += new System.EventHandler(this.OkayButton_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.backupLocationText);
            this.groupBox8.Controls.Add(this.backupPerformCheck);
            this.groupBox8.Location = new System.Drawing.Point(12, 109);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(486, 51);
            this.groupBox8.TabIndex = 9;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Backup";
            // 
            // backupLocationText
            // 
            this.backupLocationText.Location = new System.Drawing.Point(129, 18);
            this.backupLocationText.Name = "backupLocationText";
            this.backupLocationText.Size = new System.Drawing.Size(349, 20);
            this.backupLocationText.TabIndex = 1;
            // 
            // backupPerformCheck
            // 
            this.backupPerformCheck.AutoSize = true;
            this.backupPerformCheck.Checked = true;
            this.backupPerformCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.backupPerformCheck.Location = new System.Drawing.Point(9, 20);
            this.backupPerformCheck.Name = "backupPerformCheck";
            this.backupPerformCheck.Size = new System.Drawing.Size(121, 17);
            this.backupPerformCheck.TabIndex = 0;
            this.backupPerformCheck.Text = "Perform     Location:";
            this.backupPerformCheck.UseVisualStyleBackColor = true;
            this.backupPerformCheck.CheckedChanged += new System.EventHandler(this.backupPerformCheck_CheckedChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label3);
            this.groupBox9.Controls.Add(this.label2);
            this.groupBox9.Controls.Add(this.FilterClauseText);
            this.groupBox9.Controls.Add(this.FilterTablesEditButton);
            this.groupBox9.Controls.Add(this.FilterTableList);
            this.groupBox9.Location = new System.Drawing.Point(712, 2);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(294, 678);
            this.groupBox9.TabIndex = 10;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Transfer Filters";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 377);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(153, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Filter (SQL Join/Where Clause)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Table Filters";
            // 
            // FilterClauseText
            // 
            this.FilterClauseText.Enabled = false;
            this.FilterClauseText.Location = new System.Drawing.Point(7, 392);
            this.FilterClauseText.Multiline = true;
            this.FilterClauseText.Name = "FilterClauseText";
            this.FilterClauseText.Size = new System.Drawing.Size(280, 270);
            this.FilterClauseText.TabIndex = 2;
            // 
            // FilterTablesEditButton
            // 
            this.FilterTablesEditButton.Location = new System.Drawing.Point(212, 359);
            this.FilterTablesEditButton.Name = "FilterTablesEditButton";
            this.FilterTablesEditButton.Size = new System.Drawing.Size(75, 23);
            this.FilterTablesEditButton.TabIndex = 1;
            this.FilterTablesEditButton.Text = "Add/Delete";
            this.FilterTablesEditButton.UseVisualStyleBackColor = true;
            this.FilterTablesEditButton.Click += new System.EventHandler(this.FilterTablesEditButton_Click);
            // 
            // FilterTableList
            // 
            this.FilterTableList.FormattingEnabled = true;
            this.FilterTableList.Location = new System.Drawing.Point(7, 34);
            this.FilterTableList.Name = "FilterTableList";
            this.FilterTableList.Size = new System.Drawing.Size(281, 319);
            this.FilterTableList.TabIndex = 0;
            this.FilterTableList.SelectedIndexChanged += new System.EventHandler(this.FilterTableList_SelectedIndexChanged);
            // 
            // groupBoxFK
            // 
            this.groupBoxFK.Controls.Add(this.SyncFKCheck);
            this.groupBoxFK.Location = new System.Drawing.Point(504, 109);
            this.groupBoxFK.Name = "groupBoxFK";
            this.groupBoxFK.Size = new System.Drawing.Size(202, 51);
            this.groupBoxFK.TabIndex = 11;
            this.groupBoxFK.TabStop = false;
            this.groupBoxFK.Text = "Foreign Key";
            // 
            // SyncFKCheck
            // 
            this.SyncFKCheck.AutoSize = true;
            this.SyncFKCheck.Checked = true;
            this.SyncFKCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SyncFKCheck.Location = new System.Drawing.Point(9, 20);
            this.SyncFKCheck.Name = "SyncFKCheck";
            this.SyncFKCheck.Size = new System.Drawing.Size(109, 17);
            this.SyncFKCheck.TabIndex = 0;
            this.SyncFKCheck.Text = "Sync Foreign Key";
            this.SyncFKCheck.UseVisualStyleBackColor = true;
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 714);
            this.ControlBox = false;
            this.Controls.Add(this.groupBoxFK);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkayButton);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBoxFK.ResumeLayout(false);
            this.groupBoxFK.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button SchemaTablesEditButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OkayButton;
        private System.Windows.Forms.Button DataTablesEditButton;
        public System.Windows.Forms.TextBox TitleText;
        public System.Windows.Forms.RadioButton DeployTypeRadioAlter;
        public System.Windows.Forms.RadioButton DeployTypeRadioDropCreate;
        public System.Windows.Forms.RadioButton SchemaSelectRadioInclude;
        public System.Windows.Forms.RadioButton SchemaSelectRadioExclude;
        public System.Windows.Forms.RadioButton DataSelectRadioExclude;
        public System.Windows.Forms.RadioButton DataSelectRadioInclude;
        public System.Windows.Forms.CheckBox DataTransferCheckTypes;
        public System.Windows.Forms.CheckBox DataTransferCheckStatic;
        public System.Windows.Forms.CheckedListBox SchemaTableList;
        public System.Windows.Forms.CheckedListBox DataTableList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox8;
        public System.Windows.Forms.CheckBox excludeSchemaTempCheck;
        public System.Windows.Forms.CheckBox excludeDataTempCheck;
        public System.Windows.Forms.RadioButton ddlLoggingRadioAll;
        public System.Windows.Forms.RadioButton ddlLoggingRadioChanges;
        public System.Windows.Forms.RadioButton ddlLoggingRadioNone;
        public System.Windows.Forms.TextBox backupLocationText;
        public System.Windows.Forms.CheckBox backupPerformCheck;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button FilterTablesEditButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FilterClauseText;
        public System.Windows.Forms.CheckedListBox FilterTableList;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox maxRowsText;
        public System.Windows.Forms.RadioButton DeployTypeRadioTransfer;
        public System.Windows.Forms.RadioButton DeployTypeRadioLogOnly;
        private System.Windows.Forms.GroupBox groupBoxFK;
        public System.Windows.Forms.CheckBox SyncFKCheck;
    }
}