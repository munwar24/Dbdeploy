using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data.Odbc;
using System.IO;
using System.Text.RegularExpressions;

namespace OmniDbDeploy
{
    public partial class DbDeployForm : Form
    {
        Thread scriptThread;

        OdbcConnection meta;
        public TableSelectForm tableSelectForm = new TableSelectForm();
        private Logger log = Logger.Singleton;

        public DbDeployForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Source source = (Source)(sourceList.SelectedItem);
            Target target = (Target)(targetList.SelectedItem);
            Configuration config = (Configuration)(configurationList.SelectedItem);

            if (source != null && target != null && config != null)
            {
                if (source.connectionString.ToUpper() != target.connectionString.ToUpper())
                {
                    GoButton.Enabled = false;
                    TransactionScript script = new TransactionScript(source, target, config, doneHandler);
                    scriptThread = new Thread(script.deploy);
                    scriptThread.Start();
                }
                else
                {
                    MessageBox.Show("The source, and target, should not be same.");
                }
            }
            else
            {
                MessageBox.Show("A source, target, and configuration must be selected before a deployment can be made.");
            }
        }

        delegate void doneHandlerDelegate();

        private void doneHandler()
        {
            if (GoButton.InvokeRequired)
            {
                doneHandlerDelegate d = new doneHandlerDelegate(doneHandler);
                GoButton.Invoke(d, new object[] { });
            }
            else
            {

                try
                {
                    DateTime now = DateTime.Now;
                    string dirName = String.Format("Deployment {0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                    System.IO.Directory.CreateDirectory(dirName);
                    fullLogText.SaveFile(dirName + "\\Full Log.rtf");
                    changeLogText.SaveFile(dirName + "\\Change Log.rtf");
                    warningLogText.SaveFile(dirName + "\\Warning Log.rtf");
                    errorLogText.SaveFile(dirName + "\\Error Log.rtf");
                    ddlLogText.SaveFile(dirName + "\\DDL Log.rtf");
                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.error, "Unable to save deployment logs due to an exception.");
                    log.log(Logger.LogLevel.error, ex.Message);
                    log.log(Logger.LogLevel.error, ex.StackTrace);
                }

                GoButton.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tableSelectForm.dbDeployForm = this;

            this.Show();

            log.initialize(fullLogText, 
                changeLogText, 
                warningLogText, 
                errorLogText,
                ddlLogText,
                logTabs.TabPages[1],
                logTabs.TabPages[2],
                logTabs.TabPages[3],
                statusFooter, 
                phaseLabel, 
                statusLabel, 
                tableProgressBar, 
                rowProgressBar
            );

            log.log(Logger.LogLevel.info, "Improvement Interactive Database Deployment Utility");
            log.log(Logger.LogLevel.info, "DbDeploy Version 0.5.0");

            SourcesRead();

        }

        private void SourcesRead()
        {
            log.phaseUpdate("Loading sources from source file...");
            log.statusUpdate("Openning file...");

            StreamReader reader = File.OpenText("Source.txt");
            MatchCollection matches;

            string line; ;
            while ((line = reader.ReadLine()) != null)
            {
                matches = Regex.Matches(line,"(?!\").+?(?=\")");
                sourceList.Items.Add(new Source(matches[0].Value,matches[2].Value,matches[4].Value));
            }

            reader.Close();

            log.phaseUpdate("Waiting for User");
            log.statusUpdate("");
        }

        private void SourcesSave()
        {
            log.phaseUpdate("Saving sources to source file...");
            log.statusUpdate("Openning file...");

            if( File.Exists("Source.txt") )
            {
                File.Delete("Source.txt");
            }

            StreamWriter writer = File.CreateText("Source.txt");

            foreach (Source source in sourceList.Items)
            {
                writer.WriteLine("\"" + source.server + "\",\"" + source.name + "\",\"" + source.connectionString + "\"");
            }

            writer.Close();

            log.phaseUpdate("Waiting for User");
            log.statusUpdate("");
        }

        private void TargetsRead()
        {
            log.phaseUpdate("Loading targets from source database...");
            log.statusUpdate("Openning connection...");

            meta.Open();

            log.statusUpdate("Connection open, reading targets...");

            OdbcCommand command = new OdbcCommand();

            command.Connection = meta;
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from meta_db_deploy_targets;";

            targetList.Items.Clear();
            //configurationList.Items.Clear();

            try
            {
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    targetList.Items.Add(
                        new Target(
                            (Int64)reader["meta_db_deploy_target"],
                            (string)reader["meta_dbdt_server"],
                            (string)reader["meta_dbdt_name"],
                            (string)reader["meta_dbdt_connection_string"]
                         )
                    );
                }

                reader.Close();
            }
            catch 
            {
                log.log(Logger.LogLevel.error, "Error accessing source database meta_db_deploy_targets table.");
                if (MessageBox.Show("Would you like to create the meta_db_deploy tables in the source database?", "Meta Tables Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    MetaTablesCreate();
                }
            }

            log.statusUpdate("Targets read, closing database...");

            meta.Close();

            log.statusUpdate("Database closed.");

            log.phaseUpdate("Waiting for User");
            log.statusUpdate("");

        }

        private void MetaTablesCreate()
        {
            try
            {
                OdbcCommand command = new OdbcCommand();

                command.Connection = meta;
                command.CommandType = CommandType.Text;


                command.CommandText = "";
                MessageBox.Show("Not Yet!");
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Unable to create meta db deploy tables...");
                log.log(Logger.LogLevel.error, ex.Message);
            }
        }

        private void DeployEnable()
        {
            if (targetList.SelectedIndex >= 0 && configurationList.SelectedIndex >= 0 && (scriptThread == null || scriptThread.ThreadState != ThreadState.Running))
                GoButton.Enabled = true;
            else
                GoButton.Enabled = false;
        }

        /*
        private void targetLoad(Target target)
        {
            ConfigurationsRead(target.id);
        }
        */

        private string nullClean(object o)
        {
            if (o == DBNull.Value)
                return "";

            return (string)o;
        }

        private void ConfigurationsRead()
        {
            log.phaseUpdate("Loading target's configurations from source database...");
            log.statusUpdate("Openning connection...");

            meta.Open();

            log.statusUpdate("Connection open, reading configurations...");

            configurationList.Items.Clear();

            OdbcCommand command = new OdbcCommand();

            command.Connection = meta;
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from meta_db_deploy_configurations"; // where meta_db_deploy_target = " + id + ";";

            OdbcDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                configurationList.Items.Add(
                    new Configuration(
                        (Int64)reader["meta_db_deploy_configuration"],
                        //(Int64)reader["meta_db_deploy_target"],
                        (Int64)reader["meta_db_deploy_type"],
                        (Int64)reader["meta_db_deploy_tbl_sel_schema"],
                        (Int64)reader["meta_db_deploy_tbl_sel_data"],
                        (string)reader["meta_dbdc_title"],
                        (string)reader["meta_dbdc_transfer_static_data"],
                        (string)reader["meta_dbdc_transfer_types_data"],
                        (long)reader["meta_dbdc_transfer_max_rows"],
                        (Configuration.DdlLogging)(long)reader["meta_db_deploy_log_type"],
                        (string)reader["meta_dbdc_temp_exclude_schema"],
                        (string)reader["meta_dbdc_temp_exclude_data"],
                        (string)reader["meta_dbdc_backup_perform"],
                        (string)nullClean(reader["meta_dbdc_backup_location"]),
                        (string)reader["meta_dbdc_exclude_fk"]
                     )
                );
            }

            reader.Close();

            log.statusUpdate("Configurations read, closing database...");

            meta.Close();

            log.statusUpdate("Database closed.");

            log.phaseUpdate("Waiting for User");
            log.statusUpdate("");
        }

        private void TablesRead(Configuration config)
        {
            log.phaseUpdate("Loading schema tables from source database...");
            log.statusUpdate("Openning connection...");

            meta.Open();

            log.statusUpdate("Connection open, reading schema tables...");

            OdbcCommand command = new OdbcCommand();

            command.Connection = meta;
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from meta_db_deploy_schema_tables where meta_db_deploy_configuration = " + config.id + ";";

            config.schemaTables.Clear();

            OdbcDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                config.schemaTables.Add((string)reader["meta_dbdst_name"], new TableSelect(reader["meta_dbdst_active"].ToString().ToUpper() == "Y",(string)reader["meta_dbdst_name"]));
            }

            reader.Close();

            log.statusUpdate(" schema tables read, readin data tables...");

            command.CommandText = "select * from meta_db_deploy_data_tables where meta_db_deploy_configuration = " + config.id + ";";

            config.dataTables.Clear();

            reader = command.ExecuteReader();

            while (reader.Read())
            {
                config.dataTables.Add((string)reader["meta_dbddt_name"], new TableSelect(reader["meta_dbddt_active"].ToString().ToUpper() == "Y", (string)reader["meta_dbddt_name"]));
            }

            reader.Close();

            log.statusUpdate(" reader tables read, reading filter tables...");

            command.CommandText = "select * from meta_db_deploy_filter_tables where meta_db_deploy_configuration = " + config.id + ";";

            config.filterTables.Clear();

            reader = command.ExecuteReader();

            while (reader.Read())
            {
                config.filterTables.Add((string)reader["meta_dbdft_name"], new TableFilter(reader["meta_dbdft_active"].ToString().ToUpper() == "Y", (string)reader["meta_dbdft_name"], (string)reader["meta_dbdft_filter_clause"]));
            }

            reader.Close();

            log.statusUpdate(" filter tables read, closing database...");

            meta.Close();

            log.statusUpdate("Database closed.");

        }

        private void configurationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //log.phaseUpdate("Loading configuration's specifics from source database...");
            //log.statusUpdate("Openning connection...");

            Configuration config = (Configuration)(configurationList.SelectedItem);

            DeployEnable();

            if (config == null)
            {
                configEditButton.Enabled = false;
                configDeleteButton.Enabled = false;
                return;
            }

            configEditButton.Enabled = true;
            configDeleteButton.Enabled = true;
            /*
            meta.Open();

            log.statusUpdate("Connection open, reading schema tables...");

            config.schemaTables.Clear();

            OdbcCommand command = new OdbcCommand();

            command.Connection = meta;
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from meta_db_deploy_schema_tables where meta_db_deploy_configuration = " + config.id + ";";

            OdbcDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                config.schemaTables.Add((string)reader["meta_dbdst_name"]);
            }
            reader.Close();

            log.statusUpdate("Schema tables read, closing database...");


            log.statusUpdate("Connection open, reading schema tables...");

            config.dataTables.Clear();

            command.CommandText = "select * from meta_db_deploy_data_tables where meta_db_deploy_configuration = " + config.id + ";";

            reader = command.ExecuteReader();

            while (reader.Read())
            {
                config.dataTables.Add((string)reader["meta_dbddt_name"]);
            }
            reader.Close();

            log.statusUpdate("Data tables read, closing database...");
            
            meta.Close();

            log.statusUpdate("Database closed.");
              
             */

            TablesRead(config);

            /*

            log.log(Logger.LogLevel.info, "Configuration Selected:");
            log.indent();

            log.log(Logger.LogLevel.info, "                    Title: " + config.title);
            log.log(Logger.LogLevel.info, "              Deploy type: " + config.type + " (" + DeployTypeToString(config.type) + ")");
            log.log(Logger.LogLevel.info, " Schema table select type: " + config.tableSelectSchema + " (" + TableSelectTypeToString(config.tableSelectSchema) + ")");
            log.log(Logger.LogLevel.info, "   Data table select type: " + config.tableSelectData + " (" + TableSelectTypeToString(config.tableSelectData) + ")");
            log.log(Logger.LogLevel.info, "     Transfer static data: " + config.transferStaticData);
            log.log(Logger.LogLevel.info, "      Transfer types data: " + config.transferStaticData);
             */

            /*

            log.log("");
            log.log("   Schema Tables Selected:");

            IndentBig();
            foreach (TableSelect select in config.schemaTables.Values )
            {
                log.log(select.name + " (Active=" + select.active + ")");
            }
            UnindentBig();

            log.log("");
            log.log("     Data Tables Selected:");

            IndentBig();
            foreach (TableSelect select in config.dataTables.Values)
            {
                log.log(select.name + " (Active=" + select.active + ")");
            }
            UnindentBig();
             */

            /*
            log.unindent();

            log.log(Logger.LogLevel.info, "");

            //GoButton.Enabled = true;

            log.phaseUpdate("Waiting for User");
            log.statusUpdate("");
             */

        }

        private void IndentBig()
        {
            log.indent();
            log.indent();
            log.indent();
            log.indent();
            log.indent();
            log.indent();
            log.indent();
        }

        private void UnindentBig()
        {
            log.unindent();
            log.unindent();
            log.unindent();
            log.unindent();
            log.unindent();
            log.unindent();
            log.unindent();
        }

        private string DeployTypeToString(Int64 id)
        {
            string value = "Unknown";

            switch (id)
            {
                case 1:
                    value = "Drop & Create";
                    break;

                case 2:
                    value = "Alter";
                    break;

                case 3:
                    value = "Transer Data Only";
                    break;
            }

            return value;
        }

        private string TableSelectTypeToString(Int64 id)
        {
            string value = "Unknown";

            switch (id)
            {
                case 1:
                    value = "Include all tables/views by default, exclude tables/views selected.";
                    break;

                case 2:
                    value = "Exclude all tables/views by default, include tables/views selected.";
                    break;
            }

            return value;
        }

        private void sourceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            tableSelectForm.tablesLoaded = false;

            targetList.Items.Clear();
            configurationList.Items.Clear();
            DeployEnable();
            sourceEditButton.Enabled = false;
            sourceDeleteButton.Enabled = false;
            configAddButton.Enabled = false;
            TargetAddButton.Enabled = false;

            if (sourceList.SelectedIndex >= 0)
            {
                if (sourceLoad((Source)(sourceList.SelectedItem)))
                {
                    sourceEditButton.Enabled = true;
                    sourceDeleteButton.Enabled = true;
                    configAddButton.Enabled = true;
                    TargetAddButton.Enabled = true;
                }
            }
        }

        private bool sourceLoad(Source source)
        {
            bool success = false;
            try
            {
                meta = new OdbcConnection(source.connectionString);
                TargetsRead();
                ConfigurationsRead();
                success = true;
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Error reading targets...");
                log.log(Logger.LogLevel.error, ex.Message);
            }
            return success;
        }

        private void sourceAddButton_Click(object sender, EventArgs e)
        {
            DatabaseForm form = new DatabaseForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Source source = new Source(form.ServerText.Text, form.DatabaseNameText.Text, form.ConnectionStringText.Text);
                sourceList.Items.Add(source);
                sourceList.SelectedItem = source;
                SourcesSave();
            }
        }

        private void TargetAddButton_Click(object sender, EventArgs e)
        {
            Source source = (Source)(sourceList.SelectedItem);

            if (source == null)
            {
                MessageBox.Show("Please select a source to which you want to add a target.");
            }
            else
            {
                bool AlreadyClosed = false;
                DatabaseForm form = new DatabaseForm();
                if (form.ShowDialog() == DialogResult.OK)
                {

                    OdbcCommand command = new OdbcCommand();
                    command.Connection = meta;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "insert into meta_db_deploy_targets(meta_dbdt_server, meta_dbdt_name, meta_dbdt_connection_string) values(?,?,?)";
                    command.Parameters.Add(new OdbcParameter("1", form.ServerText.Text));
                    command.Parameters.Add(new OdbcParameter("2", form.DatabaseNameText.Text));
                    command.Parameters.Add(new OdbcParameter("3", form.ConnectionStringText.Text));

                    try
                    {
                        meta.Open();
                        try
                        {
                            if (command.ExecuteNonQuery() == 1)
                            {
                                log.log(Logger.LogLevel.info, "Target added successfully.");

                                command.CommandText = "select max(meta_db_deploy_target) from meta_db_deploy_targets";
                                long id = (long)(command.ExecuteScalar());

                                Target target = new Target(id, form.ServerText.Text, form.DatabaseNameText.Text, form.ConnectionStringText.Text);
                                targetList.Items.Add(target);

                                meta.Close();
                                AlreadyClosed = true;
                                targetList.SelectedItem = target;
                            }
                            else
                            {
                                log.log(Logger.LogLevel.error, "Target was not added.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to add target...");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                        finally
                        {
                            if (!AlreadyClosed)
                            {
                                meta.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to add target...");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }

                }
            }
        }

        private void sourceEditButton_Click(object sender, EventArgs e)
        {
            Source source = (Source)(sourceList.SelectedItem);

            if (source == null)
            {
                MessageBox.Show("Please select a source to edit.");
            }
            else
            {
                DatabaseForm form = new DatabaseForm();
                form.ServerText.Text = source.server;
                form.DatabaseNameText.Text = source.name;
                form.ConnectionStringText.Text = source.connectionString;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    source.server = form.ServerText.Text;
                    source.name = form.DatabaseNameText.Text;
                    source.connectionString = form.ConnectionStringText.Text;
                    SourcesSave();
                    sourceLoad(source);
                }
            }
        }

        private void targetEditButton_Click(object sender, EventArgs e)
        {
            Target target = (Target)(targetList.SelectedItem);

            if (target == null)
            {
                MessageBox.Show("Please select a target to edit.");
            }
            else
            {
                DatabaseForm form = new DatabaseForm();
                form.ServerText.Text = target.server;
                form.DatabaseNameText.Text = target.name;
                form.ConnectionStringText.Text = target.connectionString;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    target.server = form.ServerText.Text;
                    target.name = form.DatabaseNameText.Text;
                    target.connectionString = form.ConnectionStringText.Text;

                    OdbcCommand command = new OdbcCommand();
                    command.Connection = meta;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "update meta_db_deploy_targets set meta_dbdt_server = ?, meta_dbdt_name = ?, meta_dbdt_connection_string = ? where meta_db_deploy_target = ?";
                    command.Parameters.Add(new OdbcParameter("1",target.server));
                    command.Parameters.Add(new OdbcParameter("2",target.name));
                    command.Parameters.Add(new OdbcParameter("3",target.connectionString));
                    command.Parameters.Add(new OdbcParameter("4",target.id));

                    try
                    {
                        meta.Open();
                        try
                        {
                            if (command.ExecuteNonQuery() == 1)
                            {
                                log.log(Logger.LogLevel.info, "Target updated successfully.");
                            }
                            else
                            {
                                log.log(Logger.LogLevel.error, "Target was not updated.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to update target...");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                        finally
                        {
                            meta.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to udpate target...");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }

                    //targetLoad(target);
                }
            }

        }

        private void sourceDeleteButton_Click(object sender, EventArgs e)
        {
            Source source = (Source)(sourceList.SelectedItem);

            if (source == null)
            {
                MessageBox.Show("Please select a source to delete.");
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to delete the source database " + source.name + "?", "Delete Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,  MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    sourceList.Items.Remove(source);
                    SourcesSave();
                }
            }
        }

        private void TargetDeleteButton_Click(object sender, EventArgs e)
        {
            Target target = (Target)(targetList.SelectedItem);

            if (target == null)
            {
                MessageBox.Show("Please select a target to delete.");
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to delete the target database " + target.name + "?", "Delete Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    targetList.Items.Remove(target);

                    OdbcCommand command = new OdbcCommand();
                    command.Connection = meta;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "delete from meta_db_deploy_targets where meta_db_deploy_target = ?";
                    command.Parameters.Add(new OdbcParameter("1",target.id));

                    try
                    {
                        meta.Open();
                        try
                        {
                            if (command.ExecuteNonQuery() == 1)
                            {
                                log.log(Logger.LogLevel.info, "Target deleted successfully.");
                            }
                            else
                            {
                                log.log(Logger.LogLevel.error, "Target was not deleted.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to delete target...");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                        finally
                        {
                            meta.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to delete target...");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
            }
        }

        private void deployTypeSet(ConfigurationForm form, Configuration config)
        {
            if (form.DeployTypeRadioDropCreate.Checked)
            {
                config.type = 1;
            }
            else
            {
                if (form.DeployTypeRadioAlter.Checked)
                {
                    config.type = 2;
                }
                else if (form.DeployTypeRadioTransfer.Checked)
                {
                    config.type = 3;
                }
                else 
                {
                    config.type = 4;
                }
            }

        }

        private void configAddButton_Click(object sender, EventArgs e)
        {
            Source source = (Source)(sourceList.SelectedItem);

            if (source == null)
            {
                MessageBox.Show("Please select a source to which you want to add a configuration.");
            }
            else
            {
                bool AlreadyClosed = false;

                ConfigurationForm form = new ConfigurationForm();

                form.meta = meta;
                form.dbDeployForm = this;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    Configuration config = new Configuration();

                    config.title = form.TitleText.Text;
                    config.transferMaxRows = long.Parse(form.maxRowsText.Text);

                    //config.targetId = target.id;

                    deployTypeSet(form, config);

                    if (form.SchemaSelectRadioInclude.Checked)
                        config.tableSelectSchema = 1;
                    else
                        config.tableSelectSchema = 2;

                    if (form.DataSelectRadioInclude.Checked)
                        config.tableSelectData = 1;
                    else
                        config.tableSelectData = 2;

                    if (form.DataTransferCheckStatic.Checked)
                        config.transferStaticData = "Y";
                    else
                        config.transferStaticData = "N";

                    if (form.DataTransferCheckTypes.Checked)
                        config.transferTypesData = "Y";
                    else
                        config.transferTypesData = "N";

                    if (form.ddlLoggingRadioNone.Checked)
                        config.ddlLogging = Configuration.DdlLogging.none;
                    else if (form.ddlLoggingRadioChanges.Checked)
                        config.ddlLogging = Configuration.DdlLogging.changes;
                    else
                        config.ddlLogging = Configuration.DdlLogging.all;

                    // Could get this from the DB but iiSDLC says it should ALWAYS be done
                    if (form.backupPerformCheck.Checked)
                        config.backupPerform = "Y";
                    else
                        config.backupPerform = "N";

                    config.backupLocation = form.backupLocationText.Text;

                    if (form.excludeSchemaTempCheck.Checked)
                        config.tempExcludeSchema = "Y";
                    else
                        config.tempExcludeSchema = "N";

                    if (form.excludeDataTempCheck.Checked)
                        config.tempExcludeData = "Y";
                    else
                        config.tempExcludeData = "N";

                    if (form.SyncFKCheck.Checked)
                        config.FKCheckSync = "Y";
                    else
                        config.FKCheckSync = "N";

                    OdbcCommand command = new OdbcCommand();
                    command.Connection = meta;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "insert into meta_db_deploy_configurations( " +
                        " meta_db_deploy_type, " +
                        " meta_db_deploy_tbl_sel_schema, " +
                        " meta_db_deploy_tbl_sel_data, " +
                        " meta_dbdc_title, " +
                        " meta_dbdc_transfer_static_data, " +
                        " meta_dbdc_transfer_types_data, " +
                        " meta_dbdc_transfer_max_rows, " +
                        " meta_db_deploy_log_type, " +
                        " meta_dbdc_temp_exclude_schema, " +
                        " meta_dbdc_temp_exclude_data, " +
                        " meta_dbdc_backup_perform, " +
                        " meta_dbdc_backup_location, " +
                        " meta_dbdc_exclude_fk " +
                        ") " +
                        " values (" +
                        config.type + ", " +
                        config.tableSelectSchema + ", " +
                        config.tableSelectData + ", " +
                        " ?, " +
                        " '" + config.transferStaticData + "', " +
                        " '" + config.transferTypesData + "', " +
                        config.transferMaxRows + ", " + 
                        ((long)config.ddlLogging) + ", " +
                        " '" + config.tempExcludeSchema + "', " +
                        " '" + config.tempExcludeData + "', " +
                        " '" + config.backupPerform +"', " +
                        " ? " + ", " +
                        " '" + config.FKCheckSync + "'" +
                        ")";
                    command.Parameters.Add(new OdbcParameter("1", config.title));
                    command.Parameters.Add(new OdbcParameter("2", config.backupLocation));

                    try
                    {
                        meta.Open();
                        try
                        {
                            if (command.ExecuteNonQuery() == 1)
                            {
                                log.log(Logger.LogLevel.info, "Configuration added successfully.");

                                command.CommandText = "select max(meta_db_deploy_configuration) from meta_db_deploy_configurations";
                                config.id = (long)(command.ExecuteScalar());

                                meta.Close();
                                AlreadyClosed = true;

                                configurationList.Items.Add(config);
                                configurationList.SelectedItem = config;
                                TableSelectUpdate(config, form.SchemaTableList, form.DataTableList, form.FilterTableList);
                            }
                            else
                            {
                                log.log(Logger.LogLevel.error, "Configuration was not added.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to add configuration...");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                        finally
                        {
                            if (!AlreadyClosed)
                            {
                                meta.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to add configuration...");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
            }
        }

        private void configEditButton_Click(object sender, EventArgs e)
        {
            Configuration config = (Configuration)(configurationList.SelectedItem);

            if (config == null)
            {
                MessageBox.Show("Please select a configuration to edit.");
            }
            else
            {
                ConfigurationForm form = new ConfigurationForm();

                form.meta = meta;
                form.dbDeployForm = this;

                form.TitleText.Text = config.title;
                form.maxRowsText.Text = config.transferMaxRows.ToString();

                switch (config.type)
                {
                    case 1:
                        form.DeployTypeRadioDropCreate.Checked = true;
                        break;

                    case 2:
                        form.DeployTypeRadioAlter.Checked = true;
                        break;

                    case 3:
                        form.DeployTypeRadioTransfer.Checked = true;
                        break;

                    case 4:
                        form.DeployTypeRadioLogOnly.Checked = true;
                        break;
                }

                if (config.tableSelectSchema == 1)
                    form.SchemaSelectRadioInclude.Checked = true;
                else
                    form.SchemaSelectRadioExclude.Checked = true;

                if (config.tableSelectData == 1)
                    form.DataSelectRadioInclude.Checked = true;
                else
                    form.DataSelectRadioExclude.Checked = true;

                if (config.transferStaticData == "Y")
                    form.DataTransferCheckStatic.Checked = true;
                else
                    form.DataTransferCheckStatic.Checked = false;

                if (config.transferTypesData == "Y")
                    form.DataTransferCheckTypes.Checked = true;
                else
                    form.DataTransferCheckTypes.Checked = false;

                switch (config.ddlLogging)
                {
                    case Configuration.DdlLogging.none:
                        form.ddlLoggingRadioNone.Checked = true;
                        break;

                    case Configuration.DdlLogging.changes:
                        form.ddlLoggingRadioChanges.Checked = true;
                        break;

                    case Configuration.DdlLogging.all:
                        form.ddlLoggingRadioAll.Checked = true;
                        break;
                }

                // Could get this from the DB but iiSDLC says it should ALWAYS be done
                form.backupPerformCheck.Checked = true;
                form.backupLocationText.Text = config.backupLocation;

                if (config.tempExcludeSchema == "Y")
                    form.excludeSchemaTempCheck.Checked = true;
                else
                    form.excludeSchemaTempCheck.Checked = false;

                if (config.tempExcludeData == "Y")
                    form.excludeDataTempCheck.Checked = true;
                else
                    form.excludeDataTempCheck.Checked = false;

                if (config.FKCheckSync == "Y")
                    form.SyncFKCheck.Checked = true;
                else
                    form.SyncFKCheck.Checked = false;

                TableSelectSet(form.SchemaTableList, config.schemaTables);
                TableSelectSet(form.DataTableList, config.dataTables);
                TableSelectSet(form.FilterTableList, config.filterTables);
                
                if (form.ShowDialog() == DialogResult.OK)
                {
                    config.title = form.TitleText.Text;
                    config.transferMaxRows = long.Parse(form.maxRowsText.Text);

                    deployTypeSet(form,config);

                    if (form.SchemaSelectRadioInclude.Checked)
                        config.tableSelectSchema = 1;
                    else
                        config.tableSelectSchema = 2;

                    if (form.DataSelectRadioInclude.Checked)
                        config.tableSelectData = 1;
                    else
                        config.tableSelectData = 2;

                    if (form.DataTransferCheckStatic.Checked)
                        config.transferStaticData = "Y";
                    else
                        config.transferStaticData = "N";

                    if (form.DataTransferCheckTypes.Checked)
                        config.transferTypesData = "Y";
                    else
                        config.transferTypesData = "N";

                    if (form.ddlLoggingRadioNone.Checked)
                        config.ddlLogging = Configuration.DdlLogging.none;
                    else if (form.ddlLoggingRadioChanges.Checked)
                        config.ddlLogging = Configuration.DdlLogging.changes;
                    else
                        config.ddlLogging = Configuration.DdlLogging.all;

                    // Could get this from the DB but iiSDLC says it should ALWAYS be done
                    if (form.backupPerformCheck.Checked)
                        config.backupPerform = "Y";
                    else
                        config.backupPerform = "N";

                    config.backupLocation = form.backupLocationText.Text;

                    if (form.excludeSchemaTempCheck.Checked)
                        config.tempExcludeSchema = "Y";
                    else
                        config.tempExcludeSchema = "N";

                    if (form.excludeDataTempCheck.Checked)
                        config.tempExcludeData = "Y";
                    else
                        config.tempExcludeData = "N";

                    if (form.SyncFKCheck.Checked)
                        config.FKCheckSync = "Y";
                    else
                        config.FKCheckSync = "N";

                    OdbcCommand command = new OdbcCommand();
                    command.Connection = meta;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "update meta_db_deploy_configurations" +
                        //" set meta_db_deploy_target = " + config.targetId + "," +
                        " set meta_db_deploy_type = " + config.type + "," +
                        " meta_db_deploy_tbl_sel_schema = " + config.tableSelectSchema + "," +
                        " meta_db_deploy_tbl_sel_data = " + config.tableSelectData + "," +
                        " meta_dbdc_title = ?," +
                        " meta_dbdc_transfer_static_data = '" + config.transferStaticData + "'," +
                        " meta_dbdc_transfer_types_data = '" + config.transferTypesData + "', " +
                        " meta_dbdc_transfer_max_rows = " + config.transferMaxRows + ", " +
                        " meta_db_deploy_log_type = " + ((long)config.ddlLogging) + "," +
                        " meta_dbdc_temp_exclude_schema = '" + config.tempExcludeSchema + "'," +
                        " meta_dbdc_temp_exclude_data = '" + config.tempExcludeData + "'," +
                        " meta_dbdc_backup_perform = '" + config.backupPerform + "'," +
                        " meta_dbdc_backup_location = ?," +
                        " meta_dbdc_exclude_fk = '" + config.FKCheckSync + "'" +
                        " where meta_db_deploy_configuration = " + config.id;
                    command.Parameters.Add(new OdbcParameter("1", config.title));
                    command.Parameters.Add(new OdbcParameter("2", config.backupLocation));

                    try
                    {
                        meta.Open();
                        try
                        {
                            if (command.ExecuteNonQuery() == 1)
                            {
                                log.log(Logger.LogLevel.info, "Configuration udpated successfully.");
                            }
                            else
                            {
                                log.log(Logger.LogLevel.error, "Configuration was not updated.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to update configuration...");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                        finally
                        {
                            meta.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to add target...");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                     
                    TableSelectUpdate(config, form.SchemaTableList,form.DataTableList, form.FilterTableList);

                    // Force the title in the list to be updated.
                    configurationList.Items.Remove(config);
                    configurationList.Items.Add(config);
                    configurationList.SelectedItem = config;
                }
            }
        }

        private void TableSelectSet(CheckedListBox control, Dictionary<string, TableFilter> list)
        {
            control.Items.Clear();
            foreach (TableFilter select in list.Values)
            {
                control.Items.Add(select);
                if (select.active)
                    control.SetItemChecked(control.Items.IndexOf(select), true);
            }
        }

        private void TableSelectSet(CheckedListBox control, Dictionary<string, TableSelect> list)
        {
            control.Items.Clear();
            foreach (TableSelect select in list.Values)
            {
                control.Items.Add(select);
                if (select.active)
                    control.SetItemChecked(control.Items.IndexOf(select), true);
            }
        }

        private void TableSelectUpdate(Configuration config, CheckedListBox schemaList, CheckedListBox dataList, CheckedListBox filterList)
        {
            TableSelectUpdateFromList(config, schemaList, "meta_db_deploy_schema_tables", "meta_dbdst");
            TableSelectUpdateFromList(config, dataList, "meta_db_deploy_data_tables", "meta_dbddt");
            TableSelectUpdateFilterFromList(config, filterList, "meta_db_deploy_filter_tables", "meta_dbdft");

            TablesRead(config);
        }

        private void TableSelectUpdateFromList(Configuration config, CheckedListBox list, string tableName, string columnPrefix)
        {
            string keyName = tableName.Substring(0, tableName.Length - 1);

            OdbcCommand selectCommand = new OdbcCommand();
            selectCommand.Connection = meta;
            selectCommand.CommandType = CommandType.Text;

            OdbcCommand updateCommand = new OdbcCommand();
            updateCommand.Connection = meta;
            updateCommand.CommandType = CommandType.Text;
            updateCommand.CommandText = "update " + tableName +
                " set " + columnPrefix + "_active = ?" +
                " where " + keyName + " = ? ";
            OdbcParameter updateActive = new OdbcParameter("1", OdbcType.Char);
            OdbcParameter updateKey = new OdbcParameter("2", OdbcType.BigInt);
            updateCommand.Parameters.Add(updateActive);
            updateCommand.Parameters.Add(updateKey);

            OdbcCommand insertCommand = new OdbcCommand();
            insertCommand.Connection = meta;
            insertCommand.CommandType = CommandType.Text;
            insertCommand.CommandText = "insert into " + tableName +
                " (meta_db_deploy_configuration, " + columnPrefix + "_name, " + columnPrefix + "_active)" +
                " values(" + config.id + ",?,?)";
            OdbcParameter insertName = new OdbcParameter("1", OdbcType.VarChar);
            OdbcParameter insertActive = new OdbcParameter("2", OdbcType.Char);
            insertCommand.Parameters.Add(insertName);
            insertCommand.Parameters.Add(insertActive);

            /*
            OdbcCommand getIdCommand = new OdbcCommand();
            getIdCommand.Connection = meta;
            getIdCommand.CommandType = CommandType.Text;
            getIdCommand.CommandText = "select max( " + keyName + ") from " + tableName +
                " where meta_db_deploy_configuration = " + config.id;
            */

            OdbcCommand deleteCommand = new OdbcCommand();
            deleteCommand.Connection = meta;
            deleteCommand.CommandType = CommandType.Text;
            deleteCommand.CommandText = "delete from " + tableName +
                " where " + keyName + " = ? ";
            OdbcParameter deleteKey = new OdbcParameter("1", OdbcType.BigInt);
            deleteCommand.Parameters.Add(deleteKey);

            OdbcDataReader reader;

            meta.Open();

            /*
            long id;
            */
            char active;

            foreach (TableSelect select in list.Items)
            {
                if (list.CheckedItems.Contains(select))
                    active = 'Y';
                else
                    active = 'N';

                selectCommand.CommandText = "select * from " + tableName + 
                    " where meta_db_deploy_configuration = " + config.id + 
                    "   and " + columnPrefix + "_name = '" + select.name + "'" +
                    ";";

                reader = selectCommand.ExecuteReader();
                if( reader.Read() )
                {
                    // item was found ... update it
                    updateKey.Value = (long)reader[keyName];
                    updateActive.Value = active;
                    //TODO: 0. check return value here (also put in a try block)
                    updateCommand.ExecuteNonQuery();
                }
                else
                {
                    // item was not found ... insert it
                    insertName.Value = select.name;
                    insertActive.Value = active;
                    //TODO: 0. check return value here (also put in a try block)
                    insertCommand.ExecuteNonQuery();

                    /*
                    //TODO: 0. put in try block
                    id = ((long)getIdCommand.ExecuteScalar());
                    */
                }

                reader.Close();
            }

            selectCommand.CommandText = "select * from " + tableName + " where meta_db_deploy_configuration = " + config.id + ";";
            reader = selectCommand.ExecuteReader();

            bool found;
            string name;

            while (reader.Read())
            {
                name = (string)reader[columnPrefix + "_name"];
                found = false;
                foreach (TableSelect select in list.Items)
                {
                    if (select.name == name)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // delete item from database
                    deleteKey.Value = (long)reader[keyName];
                    //TODO: 0. check return value here (also put in a try block)
                    deleteCommand.ExecuteNonQuery();
                }
            }

            reader.Close();

            meta.Close();

        }

        private void TableSelectUpdateFilterFromList(Configuration config, CheckedListBox list, string tableName, string columnPrefix)
        {
            string keyName = tableName.Substring(0, tableName.Length - 1);

            OdbcCommand selectCommand = new OdbcCommand();
            selectCommand.Connection = meta;
            selectCommand.CommandType = CommandType.Text;

            OdbcCommand updateCommand = new OdbcCommand();
            updateCommand.Connection = meta;
            updateCommand.CommandType = CommandType.Text;
            updateCommand.CommandText = "update " + tableName +
                " set " + columnPrefix + "_active = ?, " +
                columnPrefix + "_filter_clause = ? " +
                " where " + keyName + " = ? ";
            OdbcParameter updateActive = new OdbcParameter("1", OdbcType.Char);
            OdbcParameter updateFilter = new OdbcParameter("2", OdbcType.VarChar);
            OdbcParameter updateKey = new OdbcParameter("3", OdbcType.BigInt);
            updateCommand.Parameters.Add(updateActive);
            updateCommand.Parameters.Add(updateFilter);
            updateCommand.Parameters.Add(updateKey);

            OdbcCommand insertCommand = new OdbcCommand();
            insertCommand.Connection = meta;
            insertCommand.CommandType = CommandType.Text;
            insertCommand.CommandText = "insert into " + tableName +
                " (meta_db_deploy_configuration, " + columnPrefix + "_name, " + columnPrefix + "_active, " +
                columnPrefix + "_filter_clause)" + 
                " values(" + config.id + ",?,?,?)";
            OdbcParameter insertName = new OdbcParameter("1", OdbcType.VarChar);
            OdbcParameter insertActive = new OdbcParameter("2", OdbcType.Char);
            OdbcParameter insertFilter = new OdbcParameter("2", OdbcType.VarChar);
            insertCommand.Parameters.Add(insertName);
            insertCommand.Parameters.Add(insertActive);
            insertCommand.Parameters.Add(insertFilter);

            OdbcCommand deleteCommand = new OdbcCommand();
            deleteCommand.Connection = meta;
            deleteCommand.CommandType = CommandType.Text;
            deleteCommand.CommandText = "delete from " + tableName +
                " where " + keyName + " = ? ";
            OdbcParameter deleteKey = new OdbcParameter("1", OdbcType.BigInt);
            deleteCommand.Parameters.Add(deleteKey);

            OdbcDataReader reader;

            meta.Open();

            char active;

            foreach (TableFilter select in list.Items)
            {
                if (list.CheckedItems.Contains(select))
                    active = 'Y';
                else
                    active = 'N';

                selectCommand.CommandText = "select * from " + tableName +
                    " where meta_db_deploy_configuration = " + config.id +
                    "   and " + columnPrefix + "_name = '" + select.name + "'" +
                    ";";

                reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    // item was found ... update it
                    updateKey.Value = (long)reader[keyName];
                    updateActive.Value = active;
                    updateFilter.Value = select.filter;
                    //TODO: 0. check return value here (also put in a try block)
                    updateCommand.ExecuteNonQuery();
                }
                else
                {
                    // item was not found ... insert it
                    insertName.Value = select.name;
                    insertActive.Value = active;
                    insertFilter.Value = select.filter;
                    //TODO: 0. check return value here (also put in a try block)
                    insertCommand.ExecuteNonQuery();

                    /*
                    //TODO: 0. put in try block
                    id = ((long)getIdCommand.ExecuteScalar());
                    */
                }

                reader.Close();
            }

            selectCommand.CommandText = "select * from " + tableName + " where meta_db_deploy_configuration = " + config.id + ";";
            reader = selectCommand.ExecuteReader();

            bool found;
            string name;

            while (reader.Read())
            {
                name = (string)reader[columnPrefix + "_name"];
                found = false;
                foreach (TableSelect select in list.Items)
                {
                    if (select.name == name)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // delete item from database
                    deleteKey.Value = (long)reader[keyName];
                    //TODO: 0. check return value here (also put in a try block)
                    deleteCommand.ExecuteNonQuery();
                }
            }

            reader.Close();

            meta.Close();

        }

        private void configDeleteButton_Click(object sender, EventArgs e)
        {
            Configuration config = (Configuration)(configurationList.SelectedItem);

            if (config == null)
            {
                MessageBox.Show("Please select a configuration to delete.");
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to delete the configuration " + config.title + "?", "Delete Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    configurationList.Items.Remove(config);

                    OdbcCommand command = new OdbcCommand();
                    command.Connection = meta;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "delete from meta_db_deploy_configurations where meta_db_deploy_configuration = ?";
                    command.Parameters.Add(new OdbcParameter("1", config.id));

                    try
                    {
                        meta.Open();
                        try
                        {
                            if (command.ExecuteNonQuery() == 1)
                            {
                                log.log(Logger.LogLevel.info, "Configuration deleted successfully.");
                            }
                            else
                            {
                                log.log(Logger.LogLevel.error, "Configuration was not deleted.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to delete configuration...");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                        finally
                        {
                            meta.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to delete configuration...");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
            }
        }

        private void targetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GoButton.Enabled = false;

            //configurationList.ClearSelected();

            DeployEnable();

            if (targetList.SelectedIndex >= 0)
            {
                //targetLoad((Target)(targetList.SelectedItem));
                targetEditButton.Enabled = true;
                TargetDeleteButton.Enabled = true;
            }
            else
            {
                targetEditButton.Enabled = false;
                TargetDeleteButton.Enabled = false;
            }

        }

        /*
        private void OleDbVersion()
        {
            //OleDbConnection connection = new OleDbConnection("Provider=IBMDADB2.DB2COPY1;Data Source=RMSE2V3;Persist Security Info=True;User ID=db2admin;Password=pT88!S@fe;Location=tstcompassaz2:50001");
            OleDbConnection connection = new OleDbConnection("Provider=IBMDADB2.DB2COPY1;Data Source=RMSINT01;Persist Security Info=True;User ID=appint;Password=RMSeeR0cK5;Location=cgunx82:55001");

            connection.Open();

            DataTable tableSchema;

            tableSchema = connection.GetSchema("Tables"); //"Indexes", new String[] { null, null, "ADM_BATCH_START_SEQUENCES", null });

            //tableSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });
            ////tableSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, "ADM_BATCH_START_SEQUENCES", "TABLE" });
            ////tableSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, null });

            //tableSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid, new Object[] { null, null, null, null, null });


            connection.Close();
        }
        */
    }
}
