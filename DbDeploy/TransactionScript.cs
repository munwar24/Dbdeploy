using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;
using System.Text.RegularExpressions;



using System.ComponentModel;
using System.Drawing;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk;
using System.IO;
using System.Xml;

namespace OmniDbDeploy
{
    class TransactionScript
    {
        private Logger log = Logger.Singleton;
        private Configuration config;
        private Target targetTarget;
        private Source sourceSource;

        private ForeignKeyRefresh foreignKeyRefresh;

        private Database _source;
        private Database _target;
        private List<string> sequencesDifferent = new List<string>();
        private List<string> modulesDifferent = new List<string>();
        private bool makeChanges = false;
        private Button goButton;

        private string tempTableSchema;

        public delegate void DoneDelegate();

        private DoneDelegate doneHandler;

        private bool FileXMLStatus = false;
        private string fileXMLname = "";
        private string TableSpaceSQL = "";

        public Database source
        {
            get
            {
                return _source;
            }
        }

        public Database target
        {
            get
            {
                return _target;
            }
        }

        public void deploy()
        {
            try
            {

                log.log(Logger.LogLevel.progress, "DEPLOYMENT COMMENCING...");
                log.log(Logger.LogLevel.info, "Deployment started at " + DateTime.Now.ToString());

                log.countsReset();

                DbOpen();

                //MetaData(source);
                //MetaData(target);

                DbSchemaLoad();

                //if (config.type == 3)
                //{
                //    log.log(Logger.LogLevel.info, "Data transfer only.  No schema synchronization will be done.");
                //    DbDataSync();
                //}
                //else
                //{
                if (DbSync() > 0)
                {
                    if (log.errorsDetected == 0)
                    {
                        log.log(Logger.LogLevel.info, "");
                        log.log(Logger.LogLevel.progress, "SCHEMA CHANGES MADE. REFRESHING SCHEMA...");
                        source.reset();
                        target.reset();
                        DbSchemaLoad();

                        if (DbSync() > 0)
                        {
                            log.log(Logger.LogLevel.info, "");
                            log.log(Logger.LogLevel.error, "SCHEMAS DID NOT SUCCESSFULLY SYNCHORNIZE.  NO DATA TRANSFER FUNCTIONS WILL BE ATTEMPTED.");
                        }
                    }
                    else
                    {
                        log.log(Logger.LogLevel.info, "");
                        log.log(Logger.LogLevel.error, "SCHEMAS DID NOT SUCCESSFULLY SYNCHORNIZE.  NO DATA TRANSFER FUNCTIONS WILL BE ATTEMPTED.");
                    }
                }
                //}

                DbClose();

                log.log(Logger.LogLevel.info, "");
                log.log(Logger.LogLevel.progress, "DEPLOYMENT COMPLETED AT " + DateTime.Now.ToString() + ".");
                log.log(Logger.LogLevel.progress, log.changesMade + " changes were made to " + target.name + ".");
                log.log(Logger.LogLevel.progress, log.warningsIssued + " warnings were issued.");
                log.log(Logger.LogLevel.progress, log.errorsDetected + " errors were detected.");

                log.phaseUpdate("Completed");
                log.statusUpdate("");


                doneHandler();

            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Deployment ended due to an exception.");
                log.log(Logger.LogLevel.error, ex.Message);
                log.log(Logger.LogLevel.error, ex.StackTrace);
            }
        }

        private delegate void GoButtonEnableDelegate();

        private void GoButtonEnable()
        {
            goButton.Enabled = true;
        }

        private void DbOpen()
        {
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "OPENNING DATABASES...");
            log.log(Logger.LogLevel.progress, "");
            log.indent();

            log.phaseUpdate("DB Connect");
            log.statusUpdate("Open Source");

            //_source = new Database("Source", "tstcompassaz2", "RMSE2REF", "Driver={IBM DB2 ODBC DRIVER};Hostname=tstcompassaz2;Port=50001;Protocol=TCPIP;Database=RMSE2REF;UID=db2admin;PWD=pT88!S@fe;");
            //_source = new Database("Source", "tstcompassaz2", "RMSE2V3", "Driver={IBM DB2 ODBC DRIVER};Hostname=tstcompassaz2;Port=50001;Protocol=TCPIP;Database=RMSE2V3;UID=db2admin;PWD=pT88!S@fe;");
            //_source = new Database("Source", "tstcompassaz2", "ESMREF", "Driver={IBM DB2 ODBC DRIVER};Hostname=tstcompassaz2;Port=50001;Protocol=TCPIP;Database=ESMREF;UID=db2admin;PWD=pT88!S@fe;");
            //_source = new Database("Source", "tstcompassaz2", "RMSE2TST", "Driver={IBM DB2 ODBC DRIVER};Hostname=tstcompassaz2;Port=50001;Protocol=TCPIP;Database=RMSE2TST;UID=db2admin;PWD=pT88!S@fe;");
            _source = new Database("Source", sourceSource.server, sourceSource.name, sourceSource.connectionString);

            log.statusUpdate("Open Target");
            //_target = new Database("Target", "WRK-CEO-AZ1", "RMSE2LOC", "Driver={IBM DB2 ODBC DRIVER};Hostname=wrk-ceo-az1;Port=50000;Protocol=TCPIP;Database=RMSE2LOC;UID=db2admin;PWD=pT88!S@fe;");
            //_target = new Database("Target", "LAP-CEO-AZ2", "RMSE2LOC", "Driver={IBM DB2 ODBC DRIVER};Hostname=lap-ceo-az2;Port=50000;Protocol=TCPIP;Database=RMSE2LOC;UID=db2admin;PWD=pT88!S@fe;");
            //_target = new Database("Target", "cgunx82", "RMSINT01", "Driver={IBM DB2 ODBC DRIVER};Hostname=cgunx82;Port=55001;Protocol=TCPIP;Database=RMSINT01;UID=appint;PWD=RMSeeR0cK5;");
            //_target = new Database("Target", "cgunx188", "ESMDEV01", "Driver={IBM DB2 ODBC DRIVER};Hostname=cgunx188;Port=51001;Protocol=TCPIP;Database=ESMDEV01;UID=appint;PWD=sp1derw3b$$5");
            //_target = new Database("Target", "WRK-CEO-AZ1", "RMSE2SQL", "Driver={SQL Native Client};Server=wrk-ceo-az1;Database=RMSE2SQL;UID=RMSe2;PWD=RMSe2;");
            //_target = new Database("Target", "LAP-CEO-AZ2", "RMSE2SQL", "Driver={SQL Native Client};Server=lap-ceo-az2;Database=RMSE2SQL;UID=RMSe2;PWD=RMSe2;");
            //_target = new Database("Target", "172.29.19.138", "RMSE2SQL", "Driver={SQL Native Client};Server=172.29.19.138;Database=RMSE2SQL;UID=RMSe2;PWD=RMSe2;");
            //_target = new Database("Target", "cgunx188", "RMSE2TST", "Driver={IBM DB2 ODBC DRIVER};Hostname=cgunx188;Port=51001;Protocol=TCPIP;Database=RMSE2TST;UID=appint;PWD=sp1derw3b$$5");

            _target = new Database("Target", targetTarget.server, targetTarget.name, targetTarget.connectionString);

            //source = new OdbcConnection("Driver={IBM DB2 ODBC DRIVER};Hostname=tstcompassaz2;Port=50001;Protocol=TCPIP;Database=RMSE2REF;UID=db2admin;PWD=pT88!S@fe;");
            ////target = new OdbcConnection("Driver={IBM DB2 ODBC DRIVER};Hostname=wrk-ceo-az1;Port=50000;Protocol=TCPIP;Database=RMSE2LOC;UID=db2admin;PWD=pT88!S@fe;");
            //target = new OdbcConnection("Driver={SQL Native Client};Server=wrk-ceo-az1;Database=RMSe2;UID=RMSe2;PWD=RMSe2;");

            source.open();
            target.open();

            log.unindent();
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "DATABASES OPEN.");
        }

        private void DbClose()
        {
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "CLOSING DATABASES...");
            log.log(Logger.LogLevel.progress, "");
            log.indent();

            target.close();
            source.close();

            log.unindent();
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "DATABASES CLOSED.");
        }

        private void DbSchemaLoad()
        {
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING SCHEMAS...");
            log.indent();

            if (config.type == 4)
            {
                log.log(Logger.LogLevel.info, "Logging the changes between source and target, but perform no changes to any database.");
            }

            DbSchemaTablesLoad();

            if (config.type == 3)
            {
                log.log(Logger.LogLevel.info, " ");
                log.log(Logger.LogLevel.info, "Data transfer only. Skipping load of all non-table objects.");
            }
            else
            {
                //DbSchemaViewsLoad();

                if (source.dialect == Ddl.Dialect.db2 && target.dialect == Ddl.Dialect.db2)
                {
                    DbSchemaMQTSLoad();
                    DbSchemaViewsLoad();
                    DbSchemaStoredProceduresLoad();
                    DbSchemaFunctionsLoad();
                    DbSchemaSequencesLoad();
                    DbSchemaModulesLoad();
                }

                if (source.dialect != Ddl.Dialect.db2 && target.dialect != Ddl.Dialect.db2)
                {
                    DbSchemaViewsLoad();
                    DbSchemaStoredProceduresLoad();
                    DbSchemaFunctionsLoad();
                    DbSchemaTriggersLoad();
                  // DbSchemaModulesLoad();
                }
            }

            log.unindent();
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "SCHEMAS LOADED.");
        }

        private void DbSchemaTablesLoad()
        {
            log.phaseUpdate("Table Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING TABLES FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");
            log.indent();

            string dial;
            dial = source.dialect.ToString();
            source.tables.load(this, this.config, 1, dial);

            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING TABLES FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");
            log.indent();

            dial = target.dialect.ToString();
            target.tables.load(this, this.config, 2, dial);

            log.unindent();

            //source.tables.list();
        }

        private void DbSchemaViewsLoad()
        {
            log.phaseUpdate("View Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING VIEWS FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            string dial;
            dial = source.dialect.ToString();
            source.views.load(this, this.config, 1, dial);
            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING VIEWS FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            dial = target.dialect.ToString();
            target.views.load(this, this.config, 2, dial);
            log.unindent();

            //source.views.list();

        }

        private void DbSchemaSequencesLoad()
        {
            log.phaseUpdate("Sequences Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING SEQUENCES FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            source.sequences.load(this, this.config, 1);
            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING SEQUENCES FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            target.sequences.load(this, this.config, 2);
            log.unindent();

            //source.sequences.list();

        }

        private int DbSync()
        {
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "SYNCHRONIZING DATABASES...");
            log.indent();

            if (DbSchemaSync() > 0)
            {
                log.unindent();
                return 1;
            }

            DbDataSync();
            //if (source.sequences.Count > 0 && target.sequences.Count > 0)
            if (source.sequences.Count > 0)
            {
                DbSequenceSync();
            }

            log.unindent();
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "DATABASES SYNCHRONIZED.");

            return 0;
        }

        private int BackupDatabase()
        {
            try
            {
                log.unindent();
                log.statusUpdate("Backingup database");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "Backing up database  " + target.name);
                //log.indent();

                //destinationPath = "\\\\"+target.server + "\\AutomatedDeployment\\DbBackups\\Crothall\\" + target.name + "\\20110316\\";
                String format = "yyyyMMdd";
                String destinationPath = "\\\\" + target.server + "\\AutomatedDeployment\\DbBackups\\Crothall\\" + target.name + "\\" + DateTime.Now.ToString(format);
                String filePath = destinationPath + "\\" + target.name + ".BAK";

                if (File.Exists(filePath))
                {
                    log.log(Logger.LogLevel.progress, "");
                    log.log(Logger.LogLevel.progress, "Backup of database " + target.name + " already present for today.");
                    log.log(Logger.LogLevel.progress, "");
                }
                else
                {
                    Directory.CreateDirectory(destinationPath);

                    String databaseName = target.name;
                    Backup sqlBackup = new Backup();

                    sqlBackup.Action = BackupActionType.Database;

                    sqlBackup.Database = databaseName;

                    string usrname = "";
                    string pwd = "";

                    string Con = target.connectionString;
                    string[] words = Con.Split(';');
                    foreach (string word in words)
                    {
                        if ((word.ToUpper().StartsWith("UID")) || (word.ToUpper().StartsWith("USER")))
                        {
                            usrname = word.Substring(word.IndexOf("=") + 1);
                        }

                        if ((word.ToUpper().StartsWith("PWD")) || (word.ToUpper().StartsWith("PASS")))
                        {
                            pwd = word.Substring(word.IndexOf("=") + 1);
                        }
                    }

                    //log.log(Logger.LogLevel.progress, "usrname : " + usrname);
                    //log.log(Logger.LogLevel.progress, "Passwrd : " + pwd);

                    BackupDeviceItem deviceItem = new BackupDeviceItem(@destinationPath + "\\" + target.name + ".bak", DeviceType.File);
                    //ServerConnection connection = new ServerConnection("SSI-SQL2", "Esm", "Esm");
                    ServerConnection connection = new ServerConnection(target.server, usrname, pwd);

                    Server sqlServer = new Server(connection);

                    Microsoft.SqlServer.Management.Smo.Database db = sqlServer.Databases[databaseName];

                    sqlBackup.Initialize = true;
                    sqlBackup.Checksum = true;
                    sqlBackup.ContinueAfterError = true;

                    sqlBackup.Devices.Add(deviceItem);
                    sqlBackup.Incremental = false;

                    sqlBackup.FormatMedia = false;

                    //connection.ConnectTimeout = 0;
                    sqlBackup.SqlBackup(sqlServer);

                    log.log(Logger.LogLevel.progress, "Backup Path = " + filePath);
                    log.log(Logger.LogLevel.progress, "Backup completed");
                }
                return 0;
            }
            catch (Exception x)
            {
                //MessageBox.Show("ERROR: An error ocurred while backing up DataBase" + x, "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.log(Logger.LogLevel.error, "Exception occurred while backing up DataBase " + ".");
                log.log(Logger.LogLevel.error, x.Message);
                String format = "yyyyMMdd";
                String destinationPath = "\\\\" + target.server + "\\AutomatedDeployment\\DbBackups\\Crothall\\" + target.name + "\\" + DateTime.Now.ToString(format);
                String filePath = destinationPath + "\\" + target.name + ".BAK";
                log.log(Logger.LogLevel.progress, "Please backup the database manually here : " + filePath);
                return 1;
            }
        }

        private int DbSchemaSync()
        {
            log.phaseUpdate("Schema Sync");
            log.statusUpdate("");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "SYNCHRONIZING SCHEMAS...");
            log.indent();

            if (target.dialect == Ddl.Dialect.sqlServer && config.type != 4)
            {
                if (BackupDatabase() > 0)
                    return 1;
            }

            if (config.FKCheckSync == "Y")
            {
                //Remove target Foreign Keys 
                //if (target.tables.Count > 0 && config.type !=4)
                if (target.tables.Count > 0)
                {
                    foreach (Table table in target.tables.Values)
                    {
                        tempTableSchema = table.schema;
                        break;
                    }
                    foreignKeyRefresh = new ForeignKeyRefresh(_target, tempTableSchema);
                    foreignKeyRefresh.load();
                    //foreignKeyRefresh.dropForeignKey();
                    string tempFKvalues = foreignKeyRefresh.sqlFKRemove;

                    OdbcCommand alter = new OdbcCommand();
                    alter.CommandType = CommandType.Text;
                    alter.Connection = target.connection;

                    if (tempFKvalues != "")
                    {
                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "-- Foreign key constraints drop commands --");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));

                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, tempFKvalues);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "-- Foreign key constraints drop commands --");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));

                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, tempFKvalues);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));

                                log.statusUpdate("Dropping foreign key constraints in target database");
                                alter.CommandText = tempFKvalues;
                                alter.CommandTimeout = 0;
                                alter.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, " ");
                                log.log(Logger.LogLevel.change, "Dropped foreign key constraints in target database. ");
                                log.log(Logger.LogLevel.change, " ");
                                log.statusUpdate("");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to drop foreign key constraint " + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                    }
                }
            }

            int changeCount = DbSchemaTableSync();
            if (changeCount == 0)
            {
                DbSchemaIndexSync();
                //DbSchemaViewSync();

                if (source.dialect == Ddl.Dialect.db2 && target.dialect == Ddl.Dialect.db2)
                {
                    DbSchemaMQTSync();
                    MQTRefresh(1);
                    DbSchemaMQTIndexSync();
                    DbSchemaViewSync();
                    DbSchemaStoredProceduresSync();
                    DbSchemaFunctionsSync();
                    DbSchemaModulesSync();
                    //MQTRefresh(2);
                    DbSchemaSequenceSync();
                }

                if (source.dialect != Ddl.Dialect.db2 && target.dialect != Ddl.Dialect.db2)
                {
                    DbSchemaViewSync();
                    DbSchemaStoredProceduresSync();
                    DbSchemaFunctionsSync();
                    DbSchemaTriggersSync();
                  
                }

                //log.unindent();
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SCHEMAS SYNCHRONIZED.");
            }

            return changeCount;
        }

        private void DbSchemaIndexSync()
        {
            // Load indexes for each table here...
            // This is seperated from DbSchemaLoad() because the syncing of the table schemas,
            // in particular constraints (primary keys), can cause indexes to be generated.
            DbSchemaLoadIndexes();
            if (config.type != 3)
            {
                log.statusUpdate("Indexes");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SYNCHRONIZING INDEXES...");
                log.log(Logger.LogLevel.progress, "");
            }

            log.indent();
            string key;
            Table targetTable;
            Index targetIndex;

            log.progressSet(0, source.tables.Count);

            foreach (Table table in source.tables.Values)
            {
                key = table.name;

                if (!ProcessSchema(key))
                {
                    continue;
                }

                if (target.tables.ContainsKey(key))
                {
                    targetTable = target.tables[key];

                    // Check what indexes may need to be added or updated
                    foreach (Index index in table.indexes.Values)
                    {
                        if (targetTable.indexes.ContainsKey(index.name))
                        {
                            targetIndex = targetTable.indexes[index.name];

                            if (config.ddlLogging == Configuration.DdlLogging.all)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, Ddl.ddl(index, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, Ddl.ddl(targetIndex, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, "");
                            }

                            if (Ddl.ddl(index, Ddl.Dialect.generic) == Ddl.ddl(targetIndex, Ddl.Dialect.generic))
                            {
                                log.log(Logger.LogLevel.progress, "Index: " + index.name + " schema is the same in the source and target databases. No Syncronization is necessary.");
                            }
                            else
                            {
                                log.log(Logger.LogLevel.change, "Index: " + index.name + " schema differs in the source and target databases. Syncronization is required.");

                                try
                                {
                                    if (config.type == 4)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, "DROP INDEX " + index.name.ToString() + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }
                                    else
                                    {
                                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                        {
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                            log.log(Logger.LogLevel.ddlChange, "DROP INDEX " + index.name.ToString() + ";");
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        }
                                        OdbcCommand drop = new OdbcCommand();
                                        drop.Connection = target.connection;
                                        drop.CommandType = CommandType.Text;
                                        if (target.dialect == Ddl.Dialect.db2)
                                        {
                                            drop.CommandText = "DROP INDEX " + index.name;
                                        }
                                        else
                                        {
                                            drop.CommandText = "DROP INDEX " + index.table.name + "." + index.name;
                                        }
                                        //OdbcCommand drop = new OdbcCommand("DROP INDEX " + index.name, target.connection);
                                        drop.CommandTimeout = 0;
                                        drop.ExecuteNonQuery();
                                        log.log(Logger.LogLevel.change, "Index " + index.name + " dropped successfully.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.log(Logger.LogLevel.error, "Exception occurred while trying to drop index " + index.name + ".");
                                    log.log(Logger.LogLevel.error, ex.Message);
                                    continue;
                                }

                                try
                                {
                                    if (config.type == 4)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, Ddl.ddl(index, Ddl.Dialect.generic) + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }
                                    else
                                    {
                                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                        {
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(index, Ddl.Dialect.generic) + ";");
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        }

                                        OdbcCommand create = new OdbcCommand(Ddl.ddl(index, target.dialect), target.connection);
                                        create.CommandTimeout = 0;
                                        create.ExecuteNonQuery();
                                        log.log(Logger.LogLevel.change, "Index " + index.name + " created successfully.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.log(Logger.LogLevel.error, "Exception occurred while trying to create index " + index.name + ".");
                                    log.log(Logger.LogLevel.error, ex.Message);
                                }
                            }

                        }
                        else
                        {
                            try
                            {
                                if (config.type == 4)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(index, Ddl.Dialect.generic) + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }
                                else
                                {
                                    if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, Ddl.ddl(index, Ddl.Dialect.generic) + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }

                                    OdbcCommand create = new OdbcCommand(Ddl.ddl(index, target.dialect), target.connection);
                                    create.CommandTimeout = 0;
                                    create.ExecuteNonQuery();
                                    log.log(Logger.LogLevel.change, "Index " + index.name + " created successfully.");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.log(Logger.LogLevel.error, "Exception occurred while trying to create index " + index.name + ".");
                                log.log(Logger.LogLevel.error, ex.Message);
                            }
                        }
                    }

                    // check what indexes need to be deleted.
                    foreach (Index index in targetTable.indexes.Values)
                    {

                        if (!table.indexes.ContainsKey(index.name))
                        {
                            try
                            {
                                if (config.type == 4)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP INDEX " + index.name + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }
                                else
                                {
                                    if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, "DROP INDEX " + index.name + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }

                                    OdbcCommand drop = new OdbcCommand();
                                    drop.Connection = target.connection;
                                    drop.CommandType = CommandType.Text;
                                    if (target.dialect == Ddl.Dialect.db2)
                                    {
                                        drop.CommandText = "DROP INDEX " + index.name;
                                    }
                                    else
                                    {
                                        drop.CommandText = "DROP INDEX " + index.table.name + "." + index.name;
                                    }
                                    drop.CommandTimeout = 0;
                                    drop.ExecuteNonQuery();
                                    log.log(Logger.LogLevel.change, "Index: " + index.name + " dropped successfully.");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.log(Logger.LogLevel.error, "Exception occurred while trying to drop index " + index.name + ".");
                                log.log(Logger.LogLevel.error, ex.Message);
                            }

                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.error, "Source table " + key + " was not found in the target database when trying to synchronize indexes.");
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);

            log.unindent();
            if (config.type != 3)
            {
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "INDEXES SYNCHRONIZED.");
            }
        }

        private void DbSchemaLoadIndexes()
        {
            if (config.type != 3)
            {
                log.phaseUpdate("Index Load");
                log.statusUpdate("Source");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "LOADING INDEXES FOR SOURCE: " + source.name);
                log.log(Logger.LogLevel.progress, "");
                log.indent();
            }

            foreach (Table table in source.tables.Values)
            {
                log.log(Logger.LogLevel.progress, "LOADING INDEXES FOR Source Table: " + table.name);
                table.indexes.load();
            }
            log.unindent();

            if (config.type != 3)
            {
                log.statusUpdate("Target");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "LOADING INDEXES FOR TARGET: " + target.name);
                log.log(Logger.LogLevel.progress, "");
            }

            log.indent();
            foreach (Table table in target.tables.Values)
            {
                log.log(Logger.LogLevel.progress, "LOADING INDEXES FOR target Table: " + table.name);
                table.indexes.load();
            }
            log.unindent();
        }

        private int DbSchemaTableSync()
        {
            log.statusUpdate("Tables");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "SYNCHRONIZING TABLES...");
            log.log(Logger.LogLevel.progress, "");
            log.indent();

            //Check the Table space info 2011-08-04
            if (target.dialect == Ddl.Dialect.db2)
            {
                string ConStr = target.connectionString;
                string[] ConStrValues = new string[10];
                ConStrValues = ConStr.Split(';');
                for (int count = 0; count < ConStrValues.Length; count = count + 1)
                {
                    if (ConStrValues[count].Trim() == "")
                    {
                        break;
                    }
                    if (ConStrValues[count].Substring(0, 8).ToUpper() == "HOSTNAME")
                    {
                        fileXMLname += ConStrValues[count].Replace(ConStrValues[count].Substring(0, 9), "").ToUpper();
                        fileXMLname += "_";
                        //break;
                    }
                    if (ConStrValues[count].Substring(0, 8).ToUpper() == "DATABASE")
                    {
                        fileXMLname += ConStrValues[count].Replace(ConStrValues[count].Substring(0, 9), "").ToUpper();
                        fileXMLname += ".xml";
                        //break;
                    }
                }
                findTablespaceFileExists(fileXMLname);
            }
            //Check the Table space info 2011-08-04

            string key;
            Table targetTable;
            int changeCount = 0;

            log.progressSet(0, source.tables.Count);

            foreach (Table table in source.tables.Values)
            {
                //key = table.schema + "." + table.name;
                key = table.name;
                if (!ProcessSchema(key))
                {
                    continue;
                }

                if (target.tables.ContainsKey(key))
                {
                    targetTable = target.tables[key];

                    if (config.ddlLogging == Configuration.DdlLogging.all)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(table, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(targetTable, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, "");
                    }

                    if (Ddl.ddl(table, Ddl.Dialect.generic) == Ddl.ddl(targetTable, Ddl.Dialect.generic))
                    {
                        log.log(Logger.LogLevel.progress, "Table: " + key + " schema is the same in the source and target databases. No Syncronization is necessary.");
                    }
                    else
                    {
                        log.log(Logger.LogLevel.change, "Table: " + key + " schema differs in the source and target databases. Syncronization is required.");
                        switch (config.type)
                        {
                            case 1:
                                DropAndCreate(table, targetTable, target.dialect, target.connection);
                                break;

                            case 2:
                                Alter(table, targetTable, target.dialect, target.connection);
                                break;

                            case 3:
                                //Alter(table, targetTable, target.dialect, target.connection);
                                DropAndCreate(table, targetTable, target.dialect, target.connection);
                                break;

                            case 4:
                                Alter(table, targetTable, target.dialect, target.connection);
                                break;
                        }
                        changeCount++;
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.change, "Table: " + key + " does not exist in target database. The table must be created.");
                    try
                    {
                        //TableSapce Change 
                        if (target.dialect == Ddl.Dialect.db2)
                        {
                            findTableSpaceSQL(table.name);
                        }
                        //TableSapce Change

                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, (Ddl.ddl(table, Ddl.Dialect.generic) + TableSpaceSQL + ";").Replace("DATETIME", "TIMESTAMP"));
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, (Ddl.ddl(table, Ddl.Dialect.generic) + TableSpaceSQL + ";").Replace("DATETIME", "TIMESTAMP"));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand create = new OdbcCommand();
                            if (target.dialect == Ddl.Dialect.sqlServer)
                                create = new OdbcCommand(Ddl.ddl(table, target.dialect).Replace("getdate", "getdate()"), target.connection);
                            else
                                create = new OdbcCommand(Ddl.ddl(table, target.dialect), target.connection);

                            create.CommandTimeout = 0;
                            create.CommandText += TableSpaceSQL;
                            create.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Table " + key + " created successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to create table " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }

                    changeCount++;
                }
                log.progressIncrement(0);
            }

            log.progressSet(0, target.tables.Count);

            foreach (Table table in target.tables.Values)
            {
                //key = table.schema + "." + table.name;
                key = table.name;

                if (!ProcessSchema(key))
                {
                    continue;
                }

                if (!source.tables.ContainsKey(key))
                {
                    log.log(Logger.LogLevel.change, "Target table: " + key + " does not exist in the source data. The table must be deleted.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "drop table " + key + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "drop table " + key + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand drop = new OdbcCommand();
                            drop.Connection = target.connection;
                            drop.CommandType = CommandType.Text;
                            drop.CommandText = "drop table " + key;
                            drop.CommandTimeout = 0;
                            drop.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Table: " + key + " dropped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to drop table " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }

                    changeCount++;
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);

            log.unindent();
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "TABLES SYNCHRONIZED.");
            log.log(Logger.LogLevel.progress, "");

            if (config.type == 4)
                changeCount = 0;

            return changeCount;
        }

        private void DropAndCreate(Table masterTable, Table targetTable, Ddl.Dialect targetDialect, OdbcConnection targetConnection)
        {
            log.log(Logger.LogLevel.change, "Dropping and recreating table " + targetTable.name + ".");

            try
            {
                if (config.type == 4)
                {
                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                    log.log(Logger.LogLevel.ddlChange, "drop table " + targetTable.name + ";");
                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                }
                else
                {
                    if (config.ddlLogging >= Configuration.DdlLogging.changes)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddlChange, "drop table " + targetTable.name + ";");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                    }

                    OdbcCommand drop = new OdbcCommand();
                    drop.Connection = targetConnection;
                    drop.CommandType = CommandType.Text;
                    drop.CommandText = "drop table " + targetTable.name;
                    drop.CommandTimeout = 0;
                    drop.ExecuteNonQuery();
                    log.log(Logger.LogLevel.change, "Table " + targetTable.name + " dropped successfully.");
                }
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Exception occurred while trying to drop table " + targetTable.name + ".");
                log.log(Logger.LogLevel.error, ex.Message);
            }

            try
            {
                //TableSapce Change
                if (targetDialect == Ddl.Dialect.db2)
                {
                    findTableSpaceSQL(masterTable.name);
                }
                //TableSapce Change

                if (config.type == 4)
                {
                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                    log.log(Logger.LogLevel.ddlChange, (Ddl.ddl(masterTable, Ddl.Dialect.generic) + TableSpaceSQL + ";").Replace("DATETIME", "TIMESTAMP"));
                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                }
                else
                {
                    if (config.ddlLogging >= Configuration.DdlLogging.changes)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddlChange, (Ddl.ddl(masterTable, Ddl.Dialect.generic) + TableSpaceSQL + ";").Replace("DATETIME", "TIMESTAMP"));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                    }

                    //OdbcCommand create = new OdbcCommand(Ddl.ddl(masterTable, targetDialect), targetConnection);
                    OdbcCommand create = new OdbcCommand();
                    if (targetDialect == Ddl.Dialect.sqlServer)
                        create = new OdbcCommand(Ddl.ddl(masterTable, targetDialect).Replace("getdate", "getdate()"), targetConnection);
                    else
                        create = new OdbcCommand(Ddl.ddl(masterTable, targetDialect), targetConnection);

                    create.CommandTimeout = 0;
                    create.CommandText += TableSpaceSQL;
                    create.ExecuteNonQuery();
                    log.log(Logger.LogLevel.change, "Table " + targetTable.name + " created successfully.");
                }
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Exception occurred while trying to create table " + masterTable.name + ".");
                log.log(Logger.LogLevel.error, ex.Message);
            }
        }

        private void Alter(Table masterTable, Table targetTable, Ddl.Dialect targetDialect, OdbcConnection targetConnection)
        {
            //TODO: 1. Check for appropriate return value here.
            Alteration alteration = Ddl.alter(masterTable, targetTable, targetDialect);

            // Check for simple alteration
            if (alteration.alterColumns.Count == 0 &&
                ((alteration.addColumns.Count > 0 && alteration.dropColumns.Count == 0) ||
                (alteration.addColumns.Count == 0 && alteration.dropColumns.Count > 0))
            )
            {
                if (!AlterSimple(alteration, masterTable, targetTable, targetDialect, targetConnection))
                {
                    AlterComplex(alteration, masterTable, targetTable, targetDialect, targetConnection);
                }
            }
            else
            {
                // Complex alteration
                // Are we moving data from the source?
                //if (ProcessData(masterTable.name))
                if (ProcessData(masterTable.name) && (!config.filterTables.ContainsKey(masterTable.name)))
                {
                    // Yes - drop the current table and recreate it, data will be copied over later via standard process
                    DropAndCreate(masterTable, targetTable, targetDialect, targetConnection);
                }
                else
                {
                    // No - need to copy the data in the current table to the newly created one...
                    // See if we need to get a mapping from the user...
                    int mappedCount = 0;
                    if (alteration.addColumns.Count > 0 && alteration.dropColumns.Count > 0)
                    {
                        MappingForm form = new MappingForm();

                        form.tableNameText.Text = masterTable.name;

                        foreach (Column column in alteration.dropColumns.Values)
                            form.oldColumnList.Items.Add(column);

                        foreach (Column column in alteration.addColumns.Values)
                            form.newColumnList.Items.Add(column);

                        if (form.ShowDialog() != DialogResult.OK)
                        {
                            log.log(Logger.LogLevel.error, "User cancelled out of a required mapping.  Alteration of table " + targetTable.name + " skipped.");
                            return;
                        }
                        mappedCount = form.mappedColumnList.Items.Count;

                        ////Update the alteration structure from the mapping form editor
                        //alteration.dropColumns.Clear();
                        //foreach (Column column in form.oldColumnList.Items)
                        //    alteration.dropColumns.Add(column.name, column);

                        //alteration.addColumns.Clear();
                        //foreach (Column column in form.newColumnList.Items)
                        //    alteration.addColumns.Add(column.name, column);

                        alteration.mappings.Clear();
                        foreach (Mapping mapping in form.mappedColumnList.Items)
                            alteration.mappings.Add(mapping);
                    }

                    if (mappedCount == 0 && alteration.alterColumns.Count == 0)
                    {
                        if (Ddl.ddlTemp(masterTable, Ddl.Dialect.generic) == Ddl.ddlTemp(targetTable, Ddl.Dialect.generic))
                        {
                            if (!AlterSimple(alteration, masterTable, targetTable, targetDialect, targetConnection))
                            {
                                AlterComplex(alteration, masterTable, targetTable, targetDialect, targetConnection);
                            }
                        }
                        else
                        {
                            AlterComplex(alteration, masterTable, targetTable, targetDialect, targetConnection);
                        }
                    }
                    else
                    {
                        if (targetDialect == Ddl.Dialect.sqlServer)
                        {
                            if (Ddl.ddlTemp(masterTable, Ddl.Dialect.generic) == Ddl.ddlTemp(targetTable, Ddl.Dialect.generic))
                            {
                                if (!AlterSimple(alteration, masterTable, targetTable, targetDialect, targetConnection))
                                {
                                    AlterComplex(alteration, masterTable, targetTable, targetDialect, targetConnection);
                                }
                            }
                            else
                            {
                                AlterComplex(alteration, masterTable, targetTable, targetDialect, targetConnection);
                            }
                        }

                        //else if (mappedCount != 0 && alteration.mappings.Count != 0)
                        //{
                        //    if (!AlterEasy(alteration, masterTable, targetTable, targetDialect, targetConnection))
                        //    {
                        //        AlterComplex(alteration, masterTable, targetTable, targetDialect, targetConnection);
                        //    }
                        //}

                        else if (mappedCount >= 0)
                        {
                            if (Ddl.ddlTemp(masterTable, Ddl.Dialect.generic) == Ddl.ddlTemp(targetTable, Ddl.Dialect.generic))
                            {
                                if (!AlterEasy(alteration, masterTable, targetTable, targetDialect, targetConnection))
                                {
                                    AlterComplex(alteration, masterTable, targetTable, targetDialect, targetConnection);
                                }
                            }
                            else
                            {
                                AlterComplex(alteration, masterTable, targetTable, targetDialect, targetConnection);
                            }
                        }
                    }
                }
            }
        }

        private bool AlterEasy(Alteration alteration, Table masterTable, Table targetTable, Ddl.Dialect targetDialect, OdbcConnection targetConnection)
        {
            bool canDo = true;
            bool success = false;

            string alterDdl = "ALTER TABLE " + targetTable.name + "\r\n";
            string nochangeDdl = alterDdl;

            foreach (Column col in alteration.dropColumns.Values)
                alterDdl += "    DROP COLUMN " + col.name + "\r\n";

            foreach (Column col in alteration.addColumns.Values)
            {
                if (targetDialect == Ddl.Dialect.db2)
                {
                    alterDdl += "    ADD COLUMN " + Ddl.columnDefinition(col, targetDialect) + "\r\n";
                }
                else
                {
                    alterDdl += "    ADD " + Ddl.columnDefinition(col, targetDialect) + "\r\n";
                }
            }

            foreach (ColumnAlterCandidate candidate in alteration.alterColumns)
            {
                if (candidate.masterColumn.typeName.ToUpper() != candidate.targetColumn.typeName.ToUpper())
                {
                    canDo = false;
                    log.log(Logger.LogLevel.warning, "Complex alteration required because " + candidate.targetColumn.name + "'s type was changed.");
                    break;
                }

                if (candidate.masterColumn.columnSize != candidate.targetColumn.columnSize)
                {
                    if (candidate.masterColumn.typeName.ToUpper() != "VARCHAR")
                    {
                        canDo = false;
                        log.log(Logger.LogLevel.warning, "Complex alteration required because " + candidate.targetColumn.name + "'s length was changed whose type was not VARCHAR.");
                        break;
                    }

                    if (candidate.masterColumn.columnSize > candidate.targetColumn.columnSize)
                    {
                        canDo = false;
                        log.log(Logger.LogLevel.warning, "Complex alteration required because " + candidate.targetColumn.name + "'s length was changed to less than its current length.");
                        break;
                    }

                    if (canDo)
                    {
                        alterDdl += "    ALTER COLUMN " + candidate.targetColumn.name + " SET DATA TYPE VARCHAR(" + candidate.masterColumn.columnSize + ")\r\n";
                    }
                }

                if (candidate.masterColumn.decimalDigits != candidate.targetColumn.decimalDigits)
                {
                    canDo = false;
                    log.log(Logger.LogLevel.warning, "Complex alteration required because " + candidate.targetColumn.name + "'s decimal digits changed.");
                    break;
                }

                if (candidate.masterColumn.nullable != candidate.targetColumn.nullable)
                {
                    if (candidate.targetColumn.nullable)
                    {
                        canDo = false;
                        log.log(Logger.LogLevel.warning, "Complex alteration required because " + candidate.targetColumn.name + " that was defined as NOT NULL must be changed to NULL.");
                        break;
                    }

                    if (canDo)
                    {
                        //alterDdl += "    ALTER COLUMN " + candidate.targetColumn.name + " SET NOT NULL\r\n";
                        alterDdl += "    ALTER COLUMN " + candidate.targetColumn.name + " DROP NOT NULL\r\n";
                    }
                }

                if (candidate.masterColumn.defaultValue != candidate.targetColumn.defaultValue)
                {
                    // Alter the default value here!
                    if (canDo)
                    {
                        if (candidate.masterColumn.defaultValue == null)
                            alterDdl += "    ALTER COLUMN " + candidate.targetColumn.name + " DROP DEFAULT " + "\r\n";
                        else
                            alterDdl += "    ALTER COLUMN " + candidate.targetColumn.name + " SET DEFAULT " + candidate.masterColumn.defaultValue + "\r\n";
                    }
                }
            }

            if (canDo)
            {
                if (alterDdl == nochangeDdl)
                {
                    AlterPrimaryKey(alteration, masterTable, targetTable, targetDialect, targetConnection);
                    success = true;
                }
                else
                {
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, alterDdl + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, alterDdl + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand alter = new OdbcCommand(alterDdl, target.connection);
                            alter.CommandTimeout = 0;
                            alter.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Table " + targetTable.name + " altered successfully.");
                            AlterPrimaryKey(alteration, masterTable, targetTable, targetDialect, targetConnection);
                            success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.warning, "Exception occurred while trying to alter table " + targetTable.name + ".");
                        log.log(Logger.LogLevel.warning, ex.Message);
                        log.log(Logger.LogLevel.warning, "Complex alteration will be attempted for table " + targetTable.name + ".");
                    }
                }
            }

            if (canDo)
            {
                if (alterDdl == nochangeDdl)
                {
                    //AlterForeignKey(alteration, masterTable, targetTable, targetDialect, targetConnection);
                    //success = true;
                }
                else
                {
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, alterDdl + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, alterDdl + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand alter = new OdbcCommand(alterDdl, target.connection);
                            alter.CommandTimeout = 0;
                            alter.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Table " + targetTable.name + " altered successfully.");
                            //AlterForeignKey(alteration, masterTable, targetTable, targetDialect, targetConnection);
                            //success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.warning, "Exception occurred while trying to alter table " + targetTable.name + ".");
                        log.log(Logger.LogLevel.warning, ex.Message);
                        log.log(Logger.LogLevel.warning, "Complex alteration will be attempted for table " + targetTable.name + ".");
                    }
                }
            }
            return success;
        }

        private void AlterComplex(Alteration alteration, Table masterTable, Table targetTable, Ddl.Dialect targetDialect, OdbcConnection targetConnection)
        {
            string PrimaryColumnName = "";
            if (config.type != 4)
            {
                bool failureDetected = false;
                // Make sure that the META_DB_TRANSFER_TEMP is deleted
                OdbcCommand command = new OdbcCommand("DROP TABLE META_DB_DEPLOY_TRANSFER_TEMP", target.connection);
                try
                {
                    command.CommandTimeout = 0;
                    command.ExecuteNonQuery();
                }
                catch {/*Do Nothing*/}

                // Drop the Primary key constraint in target table to create the Meta Temp table in SQL server database 20110811
                if (targetDialect == Ddl.Dialect.sqlServer)
                {
                    PrimaryColumnName = findPrimaryKeyColumnNameSqlServer(targetTable);
                    if (PrimaryColumnName != "")
                    {
                        command.CommandText = " ALTER TABLE " + targetTable.name + " DROP CONSTRAINT " + PrimaryColumnName;
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                    }
                }

                // CREATE the META_DB_TRANSFER_TEMP table with the new target schema
                try
                {
                    command.CommandText = Ddl.ddl(masterTable, targetDialect).Replace("CREATE TABLE " + masterTable.name, "CREATE TABLE META_DB_DEPLOY_TRANSFER_TEMP");
                    if (targetDialect == Ddl.Dialect.sqlServer)
                    {
                        command.CommandText = command.CommandText.Replace("getdate", "getdate()");
                    }
                    //TableSapce Change 
                    if (targetDialect == Ddl.Dialect.db2)
                    {
                        findTableSpaceSQL(masterTable.name);
                    }
                    //TableSapce Change

                    command.CommandTimeout = 0;
                    command.CommandText += TableSpaceSQL;
                    command.ExecuteNonQuery();
                    log.log(Logger.LogLevel.progress, "Transfer temporary table for complex alteration of " + masterTable.name + " created.");
                }
                catch (Exception ex)
                {
                    failureDetected = true;
                    log.log(Logger.LogLevel.error, "Exception occurred while trying to perform complex alteration of table " + masterTable.name + ".");
                    log.log(Logger.LogLevel.error, "CREATE TABLE META_DB_DEPLOY_TRANSFER_TEMP failed.");
                    log.log(Logger.LogLevel.error, ex.Message);
                }

                if (!failureDetected)
                {
                    try
                    {
                        command.CommandText = Ddl.ddl(alteration, "META_DB_DEPLOY_TRANSFER_TEMP");
                        if (targetDialect == Ddl.Dialect.sqlServer)
                        {
                            if (PrimaryColumnName != "")
                            {
                                command.CommandText = "SET IDENTITY_INSERT META_DB_DEPLOY_TRANSFER_TEMP ON;" + command.CommandText + "; SET IDENTITY_INSERT META_DB_DEPLOY_TRANSFER_TEMP OFF";
                            }
                            else
                            {
                                command.CommandText = command.CommandText + ";";
                            }
                            //command.CommandText = "SET IDENTITY_INSERT META_DB_DEPLOY_TRANSFER_TEMP ON;" + command.CommandText + "; SET IDENTITY_INSERT META_DB_DEPLOY_TRANSFER_TEMP OFF";
                        }

                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            log.log(Logger.LogLevel.ddlChange, command.CommandText);

                        command.CommandTimeout = 0;
                        int insertCount = command.ExecuteNonQuery();
                        log.log(Logger.LogLevel.progress, insertCount + " rows were successfully transfered to temp table from table " + masterTable.name + ".");
                    }
                    catch (Exception ex)
                    {
                        failureDetected = true;
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to perform complex alteration of table " + masterTable.name + ".");
                        log.log(Logger.LogLevel.error, "Transfer of data to tempory table failed.");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }

                if (!failureDetected)
                {
                    try
                    {
                        command.CommandText = "DROP TABLE " + targetTable.name;
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                        log.log(Logger.LogLevel.progress, "Successfully dropped " + targetTable.name + ".");
                    }
                    catch (Exception ex)
                    {
                        failureDetected = true;
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to perform complex alteration of table " + masterTable.name + ".");
                        log.log(Logger.LogLevel.error, "Drop of original table " + targetTable.name + " failed.");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }

                if (!failureDetected)
                {
                    try
                    {
                        if (targetDialect == Ddl.Dialect.db2)
                            command.CommandText = "RENAME TABLE META_DB_DEPLOY_TRANSFER_TEMP TO " + targetTable.name;
                        else if (targetDialect == Ddl.Dialect.sqlServer)
                            command.CommandText = "SP_RENAME 'META_DB_DEPLOY_TRANSFER_TEMP','" + targetTable.name + "'";

                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                        log.log(Logger.LogLevel.progress, "Successfully renamed temp table to " + targetTable.name + ".");
                    }
                    catch (Exception ex)
                    {
                        failureDetected = true;
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to perform complex alteration of table " + masterTable.name + ".");
                        log.log(Logger.LogLevel.error, "RENAMING OF TEMP TABLE (META_DB_DEPLY_TRANSFER_TEMP) TO " + targetTable.name + " FAILED.");
                        log.log(Logger.LogLevel.error, "FAILURE TO FIX THE PROBLEM MANUALLY PRIOR TO THE NEXT RUN OF DB DEPLOY WILL RESULT IN ALL DATA THAT WAS IN " + targetTable.name + "!");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }

                if (!failureDetected)
                {
                    log.log(Logger.LogLevel.progress, "Complex alteration of " + targetTable.name + " completed successfully.");
                }
            }
        }

        private bool AlterSimple(Alteration alteration, Table masterTable, Table targetTable, Ddl.Dialect targetDialect, OdbcConnection targetConnection)
        {
            bool success = false;

            string alterDdl = alteration.ddl;

            if (alterDdl == "")
            {
                AlterPrimaryKey(alteration, masterTable, targetTable, targetDialect, targetConnection);
                success = true;
            }
            else
            {
                try
                {
                    if (config.type == 4)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddlChange, alteration.ddl + ";");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                    }
                    else
                    {
                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, alteration.ddl + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }

                        OdbcCommand alter = new OdbcCommand(alteration.ddl, target.connection);
                        alter.CommandTimeout = 0;
                        alter.ExecuteNonQuery();
                        log.log(Logger.LogLevel.change, "Table " + targetTable.name + " altered successfully.");
                        AlterPrimaryKey(alteration, masterTable, targetTable, targetDialect, targetConnection);
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.warning, "Exception occurred while trying to perform simple alteration of table " + targetTable.name + ".");
                    log.log(Logger.LogLevel.warning, ex.Message);
                }
            }
            return success;
        }

        private void AlterPrimaryKey(Alteration alteration, Table masterTable, Table targetTable, Ddl.Dialect targetDialect, OdbcConnection targetConnection)
        {
            OdbcCommand alter = new OdbcCommand();
            alter.CommandType = CommandType.Text;
            alter.Connection = target.connection;

            if (alteration.ddlPrimaryKeyDrop != "")
            {
                try
                {
                    if (config.type == 4)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddlChange, alteration.ddlPrimaryKeyDrop + ";");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                    }
                    else
                    {
                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, alteration.ddlPrimaryKeyDrop + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }

                        alter.CommandText = alteration.ddlPrimaryKeyDrop;
                        alter.CommandTimeout = 0;
                        alter.ExecuteNonQuery();
                        log.log(Logger.LogLevel.change, "Primary key constraint " + targetTable.primaryKey.constraintName + " dropped from table " + targetTable.name + " successfully.");
                    }
                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.error, "Exception occurred while trying to drop primary key constraint " + masterTable.primaryKey.constraintName + " from table " + targetTable.name + ".");
                    log.log(Logger.LogLevel.error, ex.Message);
                }
            }

            if (alteration.ddlPrimaryKeyAdd != "")
            {
                try
                {
                    if (config.type == 4)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddlChange, alteration.ddlPrimaryKeyAdd + ";");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                    }
                    else
                    {
                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, alteration.ddlPrimaryKeyAdd + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }

                        alter.CommandText = alteration.ddlPrimaryKeyAdd;
                        alter.CommandTimeout = 0;
                        //Database["DB2"].ExecuteNonQuery("call SYSPROC.ADMIN_CMD ('REORG TABLE MyTable')");
                        if (targetDialect != Ddl.Dialect.sqlServer)
                        {
                            alter.CommandText = "CALL SYSPROC.ADMIN_CMD ('REORG TABLE " + targetTable.name + "'); " + alter.CommandText;
                        }
                        alter.ExecuteNonQuery();
                        log.log(Logger.LogLevel.change, "Primary key constraint " + masterTable.primaryKey.constraintName + " added to table " + targetTable.name + " successfully.");
                    }
                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.error, "Exception occurred while trying to add primary key constraint " + masterTable.primaryKey.constraintName + " from table " + targetTable.name + ".");
                    log.log(Logger.LogLevel.error, ex.Message);
                }
            }
        }

        private void AlterForeignKey(Alteration alteration, Table masterTable, Table targetTable, Ddl.Dialect targetDialect, OdbcConnection targetConnection)
        {
            OdbcCommand alter = new OdbcCommand();
            alter.CommandType = CommandType.Text;
            alter.Connection = target.connection;

            if (alteration.ddlForeignKeyDrop != "")
            {
                try
                {
                    alter.CommandText = alteration.ddlForeignKeyDrop;
                    alter.CommandTimeout = 0;
                    alter.ExecuteNonQuery();
                    for (int count = 0; count < targetTable.foreignKey.columnNames.Count; count = count + 4)
                    {
                        log.log(Logger.LogLevel.change, "Foreign key constraint " + targetTable.foreignKey.columnNames[count] + " dropped from table " + targetTable.name + " successfully.");
                    }

                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.error, "Exception occurred while trying to drop foreign key constraint " + masterTable.foreignKey.columnNames[0] + " from table " + targetTable.name + ".");
                    log.log(Logger.LogLevel.error, ex.Message);
                }
            }

            if (alteration.ddlForeignKeyAdd != "")
            {
                try
                {
                    alter.CommandText = alteration.ddlForeignKeyAdd;
                    alter.CommandTimeout = 0;
                    alter.ExecuteNonQuery();
                    log.log(Logger.LogLevel.change, "Foreign key constraint " + masterTable.foreignKey.columnNames[0] + " added to table " + targetTable.name + " successfully.");
                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.error, "Exception occurred while trying to add foreign key constraint " + masterTable.primaryKey.columnNames[0] + " from table " + targetTable.name + ".");
                    log.log(Logger.LogLevel.error, ex.Message);
                }
            }
        }

        private void DbSchemaViewSync()
        {
            if (config.type != 3)
            {
                log.statusUpdate("Views");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SYNCHRONIZING VIEWS...");
                log.log(Logger.LogLevel.progress, "");
            }
            log.indent();

            string key;
            View targetView;

            log.progressSet(0, source.views.Count);

            foreach (View view in source.views.Values)
            {
                //key = view.schema + "." + view.name;
                key = view.name;

                //if (!ProcessSchema(key))
                //{
                //    continue;
                //}

                if (target.views.ContainsKey(key))
                {
                    targetView = target.views[key];

                    if (config.ddlLogging == Configuration.DdlLogging.all)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(view, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(targetView, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, "");
                    }

                    //TODO 0. Should we just drop and recreate all views as a matter of course to make sure any table changes are incorporated into the views?
                    //TODO 0. The standard should be not to use * in view definitions (we could have DB Deploy check that).
                    string viewDdl = Ddl.ddl(view, Ddl.Dialect.generic);
                    if (viewDdl.Contains("*"))
                        log.log(Logger.LogLevel.warning, "View " + view.name + " contains a wildcard '*' to define returned columns.  This should be avoided because the view may need to be dropped and recreated when underlying tables change.");

                    if (Ddl.viewsEqual(view, targetView)) //  viewDdl == Ddl.ddl(targetView, Ddl.Dialect.generic))
                    {
                        log.log(Logger.LogLevel.progress, "View: " + key + " schema is the same in the source and target databases. No Syncronization is necessary.");
                    }
                    else
                    {
                        log.log(Logger.LogLevel.change, "View: " + key + " schema differs in the source and target databases. Syncronization is required.");

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP VIEW " + key + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP VIEW " + key + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand drop = new OdbcCommand();
                                drop.Connection = target.connection;
                                drop.CommandType = CommandType.Text;
                                drop.CommandText = "DROP VIEW " + key;
                                drop.CommandTimeout = 0;
                                drop.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "View " + key + " dropped successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to drop view " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(view, Ddl.Dialect.generic) + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(view, Ddl.Dialect.generic) + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand create = new OdbcCommand(Ddl.ddl(view, target.dialect), target.connection);
                                create.CommandTimeout = 0;
                                create.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "View " + key + " created successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to create view " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.change, "View: " + key + " does not exist in target database. The view must be created.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(view, Ddl.Dialect.generic) + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(view, Ddl.Dialect.generic) + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand create = new OdbcCommand(Ddl.ddl(view, target.dialect), target.connection);
                            create.CommandTimeout = 0;
                            create.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "View " + key + " created successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to create view " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressSet(0, target.views.Count);

            foreach (View view in target.views.Values)
            {
                //key = view.schema + "." + view.name;
                key = view.name;

                //if (!ProcessSchema(key))
                //{
                //    continue;
                //}

                if (!source.views.ContainsKey(key))
                {
                    log.log(Logger.LogLevel.change, "Target view: " + key + " does not exist in the source data. The view must be deleted.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "DROP VIEW " + key + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP VIEW " + key + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand drop = new OdbcCommand();
                            drop.Connection = target.connection;
                            drop.CommandType = CommandType.Text;
                            drop.CommandText = "DROP VIEW " + key;
                            drop.CommandTimeout = 0;
                            drop.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "View " + key + " dropped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to drop view " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);

            log.unindent();
            if (config.type != 3)
            {
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "VIEWS SYNCHRONIZED.");
            }
        }

        private void DbSchemaSequenceSync()
        {
            if (config.type != 3)
            {
                log.statusUpdate("Sequences");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SYNCHRONIZING SEQUENCES...");
                log.log(Logger.LogLevel.progress, "");
            }
            log.indent();

            string key;
            Sequence targetSequence;

            log.progressSet(0, source.sequences.Count);

            foreach (Sequence sequence in source.sequences.Values)
            {
                //key = view.schema + "." + view.name;
                key = sequence.name;

                if (target.sequences.ContainsKey(key))
                {
                    targetSequence = target.sequences[key];

                    if (config.ddlLogging == Configuration.DdlLogging.all)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(sequence, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(targetSequence, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, "");
                    }

                    if (Ddl.ddl(sequence, Ddl.Dialect.generic) == Ddl.ddl(targetSequence, Ddl.Dialect.generic))
                    {
                        log.log(Logger.LogLevel.progress, "Sequence: " + key + " schema is the same in the source and target databases. No Syncronization is necessary.");
                    }
                    else
                    {
                        log.log(Logger.LogLevel.change, "Sequence: " + key + " schema differs in the source and target databases. Syncronization is required.");
                        sequencesDifferent.Add(key);

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP SEQUENCE " + key + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP SEQUENCE " + key + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand drop = new OdbcCommand();
                                drop.Connection = target.connection;
                                drop.CommandType = CommandType.Text;
                                drop.CommandText = "DROP SEQUENCE " + key;
                                drop.CommandTimeout = 0;
                                drop.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "Sequence: " + key + " dropped successfully: ");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to drop sequence " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(sequence, Ddl.Dialect.generic) + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(sequence, Ddl.Dialect.generic) + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand create = new OdbcCommand(Ddl.ddl(sequence, target.dialect), target.connection);
                                create.CommandTimeout = 0;
                                create.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "Sequence: " + key + " created successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to create sequence " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.change, "Seqeunce: " + key + " does not exist in target database. The sequence must be created.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(sequence, Ddl.Dialect.generic) + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(sequence, Ddl.Dialect.generic) + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand create = new OdbcCommand(Ddl.ddl(sequence, target.dialect), target.connection);
                            create.CommandTimeout = 0;
                            create.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Sequence: " + key + " created successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to create sequence " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressSet(0, target.sequences.Count);

            foreach (Sequence sequence in target.sequences.Values)
            {
                //key = view.schema + "." + view.name;
                key = sequence.name;

                if (!source.sequences.ContainsKey(key))
                {
                    log.log(Logger.LogLevel.change, "Target sequence: " + key + " does not exist in the source data. The sequence must be deleted.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "DROP SEQUENCE " + key + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP SEQUENCE " + key + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand drop = new OdbcCommand();
                            drop.Connection = target.connection;
                            drop.CommandType = CommandType.Text;
                            drop.CommandText = "DROP SEQUENCE " + key;
                            drop.CommandTimeout = 0;
                            drop.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Sequence " + key + " dropped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to drop sequence " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);

            log.unindent();
            if (config.type != 3)
            {
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SEQUENCES SYNCHRONIZED.");
            }
        }

        private void DbDataSync()
        {
            if (config.type != 4)
            {
                if (config.type == 3)
                {
                    //Remove target Foreign Keys 
                    if (target.tables.Count > 0)
                    {
                        foreach (Table table in target.tables.Values)
                        {
                            tempTableSchema = table.schema;
                            break;
                        }
                        foreignKeyRefresh = new ForeignKeyRefresh(_target, tempTableSchema);
                        foreignKeyRefresh.load();
                        foreignKeyRefresh.dropForeignKey();
                    }
                }

                log.phaseUpdate("Data Sync");
                log.statusUpdate("Tables");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SYNCHRONIZING DATA...");
                log.indent();

                string sourceKey;
                string targetKey;
                Table targetTable;
                int rowCount;

                double hundredthDivisor;
                int progressValue;
                int hundredthValue;

                log.progressSet(0, source.tables.Count);

                foreach (Table table in source.tables.Values)
                {
                    //sourceKey = table.schema + "." + table.name;
                    sourceKey = table.name;
                    // TODO: May need to translate the target schema here!
                    //targetKey = table.schema + "." + table.name;
                    targetKey = table.name;

                    if (!ProcessData(table.name))
                    {
                        continue;
                    }

                    log.log(Logger.LogLevel.progress, "");
                    log.log(Logger.LogLevel.change, "Transfering data for tables: " + sourceKey + " -> " + targetKey);
                    log.log(Logger.LogLevel.progress, "");
                    log.indent();

                    targetTable = target.tables[targetKey];

                    log.statusUpdate(sourceKey);

                    // TODO: Status updates here

                    try
                    {
                        OdbcCommand countCommand = new OdbcCommand();
                        countCommand.Connection = source.connection;
                        countCommand.CommandType = CommandType.Text;
                        countCommand.CommandText = "select count(*) from " + sourceKey;

                        if (config.filterTables.ContainsKey(sourceKey))
                        {
                            TableFilter filter = (TableFilter)config.filterTables[sourceKey];
                            if (filter.active)
                            {
                                log.log(Logger.LogLevel.warning, "Data for table " + sourceKey + " is being filtered.  All data may not be transferred.");
                                countCommand.CommandText += " " + filter.filter;
                            }
                        }
                        rowCount = (int)countCommand.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to get row count from " + sourceKey + ".");
                        log.log(Logger.LogLevel.error, "Data synchronization for " + sourceKey + " skipped.");
                        log.log(Logger.LogLevel.error, ex.Message);
                        continue;
                    }

                    if (config.transferMaxRows > 0 && rowCount > config.transferMaxRows)
                    {
                        log.log(Logger.LogLevel.error, "Table " + sourceKey + " contains " + rowCount + " rows which exceeds the maximum rows of " + config.transferMaxRows + " in configuration " + config.title + ". NO DATA WILL BE TRANSFERRED.");
                        continue;
                    }

                    hundredthDivisor = (double)rowCount / 100.0;

                    log.progressSet(1);

                    log.log(Logger.LogLevel.change, "Transfering " + rowCount + " rows for table " + targetKey + ".");


                    try
                    {
                        OdbcCommand deleteCommand = new OdbcCommand();
                        deleteCommand.Connection = target.connection;
                        deleteCommand.CommandType = CommandType.Text;
                        //deleteCommand.CommandText = "delete from " + targetKey;

                        if (target.dialect == Ddl.Dialect.db2)
                            deleteCommand.CommandText = "TRUNCATE TABLE " + targetKey + " IMMEDIATE";
                        else
                            deleteCommand.CommandText = "TRUNCATE TABLE " + targetKey;

                        deleteCommand.CommandTimeout = 0;

                        //if (config.type == 3 && config.filterTables.ContainsKey(sourceKey))
                        if (config.filterTables.ContainsKey(sourceKey))
                        {
                            TableFilter filter = (TableFilter)config.filterTables[sourceKey];
                            if (filter.active)
                            {
                                log.log(Logger.LogLevel.warning, "Data for table " + sourceKey + " is being filtered on a data transer only deployment.  All data may not be deleted.");
                                deleteCommand.CommandText = "delete from " + targetKey;
                                deleteCommand.CommandText += " " + filter.filter;
                            }
                            log.log(Logger.LogLevel.change, "Successfully deleted " + deleteCommand.ExecuteNonQuery().ToString() + " from " + targetKey + ".");
                        }
                        else
                        {
                            deleteCommand.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Successfully deleted all records from " + targetKey + ".");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to delete all records from " + targetKey + ".");
                        log.log(Logger.LogLevel.error, "Data synchronization for " + targetKey + " skipped.");
                        log.log(Logger.LogLevel.error, ex.Message);
                        continue;
                    }


                    OdbcCommand identityCommand = new OdbcCommand();

                    try
                    {
                        // TODO: Status updates here

                        identityCommand.Connection = target.connection;
                        identityCommand.CommandType = CommandType.Text;

                        // TODO: Status updates here
                        if (table.columns.identity != null)
                        {
                            switch (target.dialect)
                            {
                                case Ddl.Dialect.db2:
                                    if (table.columns.identity.identity.generated.ToUpper() == "A")
                                    {
                                        identityCommand.CommandText = "ALTER TABLE " + targetKey + " " +
                                            "ALTER COLUMN " + table.columns.identity.name + " " +
                                            "SET GENERATED BY DEFAULT";
                                        identityCommand.CommandTimeout = 0;
                                        identityCommand.ExecuteNonQuery();
                                        log.log(Logger.LogLevel.change, "Succussfully set generated by default for table " + targetKey + ".");
                                    }
                                    break;

                                case Ddl.Dialect.sqlServer:
                                    identityCommand.CommandText = "set identity_insert " + targetKey + " on";
                                    identityCommand.CommandTimeout = 0;
                                    identityCommand.ExecuteNonQuery();
                                    log.log(Logger.LogLevel.change, "Succussfully set identity insert on for table " + targetKey + ".");
                                    break;

                                default:
                                    log.log(Logger.LogLevel.error, "Unknown database dialect detected in DbSync - identity 1.");
                                    log.log(Logger.LogLevel.error, "Data synchronization for " + targetKey + " skipped.");
                                    continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to setup identity column for data transfer for table " + targetKey + ".");
                        log.log(Logger.LogLevel.error, "Data synchronization for " + targetKey + " skipped.");
                        log.log(Logger.LogLevel.error, ex.Message);
                        continue;
                    }


                    int Count = 0;
                    OdbcDataReader reader = null;

                    try
                    {
                        // TODO: Status updates here
                        OdbcCommand readCommand = new OdbcCommand();

                        readCommand.Connection = source.connection;
                        readCommand.CommandType = CommandType.Text;
                        readCommand.CommandText = "select " + sourceKey + ".* from " + sourceKey;

                        if (config.filterTables.ContainsKey(sourceKey))
                        {
                            TableFilter filter = (TableFilter)config.filterTables[sourceKey];
                            if (filter.active)
                                readCommand.CommandText += " " + filter.filter;
                        }

                        if (target.dialect == Ddl.Dialect.db2)
                        {
                            //string PrimaryColumnName = findPrimaryKeyColumnName(table);
                            string PrimaryColumnName = findPrimaryKeyColumnName(targetTable);
                            if (PrimaryColumnName != "")
                            {
                                readCommand.CommandText += " Order by " + PrimaryColumnName;
                            }
                        }

                        reader = readCommand.ExecuteReader();

                        progressValue = 0;
                        int field;

                        // This is a code optimization for special data types (CLOBs)
                        if (table.columns.hasSpecialTypes)
                        {
                            // Same loop as the one below with special calls to make special data types work
                            while (reader.Read())
                            {
                                for (field = 0; field < reader.FieldCount; field++)
                                {
                                    //logText.AppendText(reader[field].ToString());
                                    ////if (reader.IsDBNull(field)) MessageBox.Show("tesT");
                                    //logText.AppendText(" - ");

                                    //if (reader.IsDBNull(field))
                                    //    targetTable.insert.Parameters["@" + reader.GetName(field)].Value = DBNull.Value;
                                    //else

                                    if (reader.GetDataTypeName(field) == "CLOB")
                                        reader.GetString(field);

                                    targetTable.insert.Parameters["@" + reader.GetName(field)].Value = reader[field];
                                }

                                if (targetTable.insert.ExecuteNonQuery() != 1)
                                    log.log(Logger.LogLevel.error, "Insert error occurred at record " + (Count + 1) + ".");

                                hundredthValue = (int)((float)(++Count) / hundredthDivisor);

                                if (progressValue != hundredthValue)
                                {
                                    progressValue = hundredthValue;
                                    log.progressUpdate(1, progressValue);
                                    //log.log("Insert record count: " + Count);
                                }

                                //log.progressIncrement();

                                if ((Count % 500) == 0)
                                    log.log(Logger.LogLevel.progress, "Insert record count: " + Count + " at " + DateTime.Now.ToLongTimeString());
                            }
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                for (field = 0; field < reader.FieldCount; field++)
                                    targetTable.insert.Parameters["@" + reader.GetName(field)].Value = reader[field];

                                if (targetTable.insert.ExecuteNonQuery() != 1)
                                    log.log(Logger.LogLevel.error, "Insert error occurred at record " + (Count + 1) + ".");

                                hundredthValue = (int)((float)(++Count) / hundredthDivisor);

                                if (progressValue != hundredthValue)
                                {
                                    progressValue = hundredthValue;
                                    log.progressUpdate(1, progressValue);
                                }

                                if ((Count % 500) == 0)
                                    log.log(Logger.LogLevel.progress, "Insert record count: " + Count + " at " + DateTime.Now.ToLongTimeString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while transfering data to " + targetKey + ".");
                        log.log(Logger.LogLevel.error, "Data synchronization for " + targetKey + " stopped.");
                        log.log(Logger.LogLevel.error, ex.Message);
                        continue;
                    }
                    finally
                    {
                        try { reader.Close(); }
                        catch { /*Do nothing*/ }
                    }

                    if (Count == rowCount)
                        log.log(Logger.LogLevel.change, "Successfully transfered " + Count + " of " + rowCount + " row(s) for table " + targetKey + ".");
                    else
                        log.log(Logger.LogLevel.error, "Only " + Count + " of " + rowCount + " row(s) were transfered for table " + targetKey + ".");

                    try
                    {
                        // TODO: Status updates here
                        if (table.columns.identity != null)
                        {
                            switch (target.dialect)
                            {
                                case Ddl.Dialect.db2:
                                    if (table.columns.identity.identity.generated.ToUpper() == "A")
                                    {
                                        identityCommand.CommandText = "ALTER TABLE " + targetKey + " " +
                                            "ALTER COLUMN " + table.columns.identity.name + " " +
                                            "SET GENERATED BY DEFAULT";
                                        identityCommand.CommandTimeout = 0;
                                        identityCommand.ExecuteNonQuery();
                                        log.log(Logger.LogLevel.change, "Successfully set generated always for table " + targetKey + ".");
                                    }

                                    //long maxIdentity = maxValue(table.columns.identity);
                                    long maxIdentity = maxValue(table.columns.identity, "");

                                    // RESET COUNTER
                                    identityCommand.CommandText = "ALTER TABLE " + targetKey + " " +
                                        "ALTER COLUMN " + table.columns.identity.name + " " +
                                        "RESTART WITH " + (maxIdentity + 1);
                                    identityCommand.CommandTimeout = 0;
                                    identityCommand.ExecuteNonQuery();
                                    log.log(Logger.LogLevel.change, "Successfully reset identity counter to " + (maxIdentity + 1) + ".");
                                    break;

                                case Ddl.Dialect.sqlServer:
                                    identityCommand.CommandText = "set identity_insert " + targetKey + " off";
                                    identityCommand.CommandTimeout = 0;
                                    identityCommand.ExecuteNonQuery();
                                    log.log(Logger.LogLevel.change, "Successfully set identity insert off for table " + targetKey + ".");
                                    break;

                                default:
                                    log.log(Logger.LogLevel.error, "Unknown database dialect detected in DbSync - identity 1.");
                                    log.log(Logger.LogLevel.error, "IDENTITY COLUMN FOR " + targetKey + " LEFT IN INAPPROPRIATE STATE. PLEASE FIX MANUALLY.");
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to reset identity insert for table " + targetKey + ".");
                        log.log(Logger.LogLevel.error, "IDENTITY COLUMN FOR " + targetKey + " LEFT IN INAPPROPRIATE STATE. PLEASE FIX MANUALLY.");
                        log.log(Logger.LogLevel.error, ex.Message);
                        continue;
                    }


                    // Check to see if a sequence is associated with this table and if so update it.
                    //if (table.name == "E_PPL_PEOPLE")
                    //{
                    //    bool testb = true;
                    //}
                    string sequenceColumnName = table.name;
                    /*
                    if (sequenceColumnName.ToUpper().EndsWith("_STATIC"))
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 8);
                    else
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 1);
                    */

                    if (sequenceColumnName.ToUpper().EndsWith("_STATIC"))
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 8);
                    else if (sequenceColumnName.ToUpper().EndsWith("_PEOPLE"))
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 7) + "_PERSON";
                    else if (sequenceColumnName.ToUpper().EndsWith("_STATUSES"))
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 2);
                    else if (sequenceColumnName.ToUpper().EndsWith("_ENTRIES"))
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 3) + "Y";
                    else if (sequenceColumnName.ToUpper().EndsWith("_CLASSES"))
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 2);
                    else if (sequenceColumnName.ToUpper().EndsWith("_CATEGORIES"))
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 3) + "Y";
                    if (sequenceColumnName.ToUpper().EndsWith("_CONFIG"))
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 8);
                    else
                        sequenceColumnName = sequenceColumnName.Substring(0, sequenceColumnName.Length - 1);

                    string sequenceName = "SEQ_" + sequenceColumnName;

                    if (sequenceColumnName == "E_HIR_NODE")
                    {
                        sequencesDifferent.Add("SEQ_E_HIR_NODE_STATIC");
                    }

                    if (!sequencesDifferent.Contains(sequenceName) && source.sequences.ContainsKey(sequenceName))
                    {
                        // Add the sequence to the different list to make sure its value gets reset.
                        sequencesDifferent.Add(sequenceName);
                    }

                    log.unindent();
                    log.progressIncrement(0);
                }

                log.progressHide(1);
                log.progressHide(0);

                log.statusUpdate("Completed");
                log.unindent();
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "DATA SYNCHRONIZED.");
                log.log(Logger.LogLevel.change, "");

                if (source.dialect == Ddl.Dialect.db2 && target.dialect == Ddl.Dialect.db2)
                    MQTRefresh(2);
            }

            if (config.FKCheckSync == "Y")
            {
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SYNCHRONIZING FOREIGN KEYS...");
                log.log(Logger.LogLevel.progress, "");
                log.indent();
                //Add target foreign keys from source
                foreach (Table table in source.tables.Values)
                {
                    tempTableSchema = table.schema;
                    break;
                }

                foreignKeyRefresh = new ForeignKeyRefresh(_source, tempTableSchema);
                foreignKeyRefresh.load();
                string tempFKvalues = foreignKeyRefresh.sqlFKAdd;

                OdbcCommand alter = new OdbcCommand();
                alter.CommandType = CommandType.Text;
                alter.Connection = target.connection;

                string[] FKString = new string[1000];
                string[] FKKeyValue = new string[10];

                if (tempFKvalues != "")
                {
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "-- Foreign key constraints add commands --");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));

                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, tempFKvalues);
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "-- Foreign key constraints add commands --");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));

                            FKString = tempFKvalues.Split(';');
                            for (int count = 0; count < FKString.Length; count = count + 1)
                            {
                                try
                                {
                                    if (FKString[count].Trim().Length > 0)
                                    {
                                        alter.CommandText = FKString[count].ToString();
                                        alter.CommandTimeout = 0;
                                        alter.ExecuteNonQuery();
                                        FKKeyValue = FKString[count].Split(' ');
                                        if (target.dialect == Ddl.Dialect.db2)
                                        {
                                            log.log(Logger.LogLevel.change, "Foreign Key " + FKKeyValue[5].ToString() + " created successfully.");
                                            log.log(Logger.LogLevel.ddlChange, FKString[count].ToString() + ";");
                                        }
                                        else if (target.dialect == Ddl.Dialect.sqlServer)
                                        {
                                            log.log(Logger.LogLevel.change, "Foreign Key " + FKKeyValue[6].ToString() + " created successfully.");
                                            log.log(Logger.LogLevel.ddlChange, FKString[count].ToString() + ";");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.log(Logger.LogLevel.error, ex.Message);
                                }
                            }
                            log.log(Logger.LogLevel.progress, "");
                            log.log(Logger.LogLevel.progress, "FOREIGN KEYS SYNCHRONIZED...");
                            log.log(Logger.LogLevel.progress, "");
                            log.unindent();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to add foreign key constraint " + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
            }
        }

        private void DbSequenceSync()
        {
            if (config.type != 4)
            {
                string columnName;
                string tableName;

                OdbcCommand command = new OdbcCommand();
                command.Connection = target.connection;
                command.CommandType = CommandType.Text;

                foreach (string sequenceName in sequencesDifferent)
                {
                    if (sequenceName == "SEQ_E_HIR_NODE_STATIC")
                    {
                        tableName = "E_HIR_NODES";
                    }
                    else
                    {

                        columnName = sequenceName.Substring(4);

                        /*
                        if (target.tables.ContainsKey(columnName + "S"))
                            tableName = columnName + "S";
                        else
                            tableName = columnName + "S_STATIC";
                        */
                        if (target.tables.ContainsKey(columnName + "S"))
                            tableName = columnName + "S";
                        else if (columnName.ToUpper().EndsWith("_PERSON"))
                            tableName = columnName.Substring(0, columnName.Length - 7) + "_PEOPLE";
                        else if (columnName.ToUpper().EndsWith("_STATUS"))
                            tableName = columnName + "ES";
                        else if (columnName.ToUpper().EndsWith("_ENTRY"))
                            tableName = columnName.Substring(0, columnName.Length - 1) + "IES";
                        else if (columnName.ToUpper().EndsWith("_CLASS"))
                            tableName = columnName + "ES";
                        else if (columnName.ToUpper().EndsWith("_CATEGORY"))
                            tableName = columnName.Substring(0, columnName.Length - 1) + "IES";
                        else if (columnName.ToUpper().EndsWith("_TARGET"))
                            tableName = columnName + "S_CONFIG";
                        else
                            tableName = columnName + "S_STATIC";
                    }
                    columnName = findPrimaryKeyColumnName(target.tables[tableName]);

                    if (columnName == "")
                        columnName = sequenceName.Substring(4);

                    try
                    {
                        //long maxIdentity = maxValue(target.tables[tableName].columns[columnName]);
                        long maxIdentity = maxValue(target.tables[tableName].columns[columnName], sequenceName);
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        try
                        {
                            // RESET COUNTER
                            command.CommandText = "ALTER SEQUENCE " + sequenceName + " " +
                                "RESTART WITH " + (maxIdentity + 1);
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Successfully reset sequence " + sequenceName + " to " + (maxIdentity + 1) + ".");
                            log.log(Logger.LogLevel.ddlChange, "ALTER SEQUENCE " + sequenceName + " RESTART WITH " + (maxIdentity + 1) + ";");
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to reset sequence value " + sequenceName + ".");
                            log.log(Logger.LogLevel.error, "SEQUENCE " + sequenceName + " LEFT IN INAPPROPRIATE STATE. PLEASE FIX MANUALLY.");
                            log.log(Logger.LogLevel.error, ex.Message);
                            continue;
                        }
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));

                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to determine max value to reset sequence " + sequenceName + ".");
                        log.log(Logger.LogLevel.error, "SEQUENCE " + sequenceName + " LEFT IN INAPPROPRIATE STATE. PLEASE FIX MANUALLY.");
                        log.log(Logger.LogLevel.error, ex.Message);
                        continue;
                    }
                }
            }
        }

        private string findPrimaryKeyColumnName(Table table)
        {
            OdbcCommand command = new OdbcCommand();
            command.Connection = target.connection;
            command.CommandType = CommandType.Text;

            // GET Primary Key Name 
            command.CommandText = "  SELECT K.COLNAME " +
                                  "  FROM SYSCAT.KEYCOLUSE K, SYSCAT.TABCONST T  " +
                                  "  WHERE K.CONSTNAME = T.CONSTNAME AND K.TABNAME = T.TABNAME  " +
                                  "  AND K.TABSCHEMA = '" + table.schema + "'" +
                                  "  AND K.TABNAME = '" + table.name + "' " +
                                  "  AND T.TYPE = 'P'";

            object value = command.ExecuteScalar();

            string pkName = "";
            //if (value == DBNull.Value)
            if (value == null)
                pkName = "";
            else
                pkName = (string)(value);

            return pkName;
        }

        private long maxValue(Column column, string sequencename)
        {
            // Exceptions for this DB command are caught at the level above.

            OdbcCommand command = new OdbcCommand();
            command.Connection = target.connection;
            command.CommandType = CommandType.Text;

            // GET MAX VALUE
            command.CommandText = "SELECT MAX(" + column.name + ") " +
                "FROM " + column.table.name;

            if (sequencename == "SEQ_E_HIR_NODE_STATIC")
            {
                command.CommandText += " WHERE E_HIR_NODE < 1000000 ";
            }

            object value = command.ExecuteScalar();

            long maxValue = 0;
            if (value == DBNull.Value)
                maxValue = 0;
            else
            {
                // TODO: 4. Should handle smallint and decimal here as well
                if (column.type == OdbcType.BigInt)
                    maxValue = (long)(value);
                else
                    maxValue = (int)(value);
            }

            return maxValue;
        }

        public bool ProcessSchema(string name)
        {
            bool process = false;

            if (config.tempExcludeSchema.ToUpper() == "Y" && (name.EndsWith("_TEMP", StringComparison.OrdinalIgnoreCase) || name.StartsWith("TEMP_", StringComparison.OrdinalIgnoreCase)))
                return false;

            if (config.tableSelectSchema == 1)
            {
                // Include by default - if its in the selected list then we should exclude 
                if (config.schemaTables.ContainsKey(name) && config.schemaTables[name].active)
                    process = false;
                else
                    process = true;
            }
            else
            {
                // Exclude by default - if its in the selected list then we should include
                if (config.schemaTables.ContainsKey(name) && config.schemaTables[name].active)
                    process = true;
                else
                    process = false;
            }

            return process;
        }

        public bool ProcessData(string name)
        {
            bool process = false;

            if (config.transferStaticData.ToUpper() == "Y" && name.EndsWith("_STATIC", StringComparison.OrdinalIgnoreCase))
                process = true;

            if (config.transferTypesData.ToUpper() == "Y" && name.EndsWith("_TYPES", StringComparison.OrdinalIgnoreCase))
                process = true;

            if (config.tempExcludeData.ToUpper() == "Y" && (name.EndsWith("_TEMP", StringComparison.OrdinalIgnoreCase) || name.StartsWith("TEMP_", StringComparison.OrdinalIgnoreCase)))
                return false;

            if (name.EndsWith("_CONFIG", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!process)
            {
                if (config.tableSelectData == 1)
                {
                    // Include by default - if its in the selected list then we should exclude 
                    if (config.dataTables.ContainsKey(name) && config.dataTables[name].active)
                        process = false;
                    else
                        process = true;
                }
                else
                {
                    // Exclude by default - if its in the selected list then we should include
                    if (config.dataTables.ContainsKey(name) && config.dataTables[name].active)
                        process = true;
                    else
                        process = false;
                }
            }

            return process;
        }

        private void MetaData(Database database)
        {

            DataTable tables;
            DataRow row;

            //tables = database.connection.GetSchema();
            //tables = database.connection.GetSchema("MetaDataCollections");
            //tables = database.connection.GetSchema("DataSourceInformation");
            //tables = database.connection.GetSchema("DataTypes");
            //tables = database.connection.GetSchema("Restrictions");
            //tables = database.connection.GetSchema("Tables");
            //tables = database.connection.GetSchema("Tables", new String[] { null, null, "ADM_BATCH_START_SEQUENCES" });
            //tables = database.connection.GetSchema("Columns", new String[] { null, null, "ADM_BATCH_START_SEQUENCES" });
            //tables = database.connection.GetSchema("Columns", new String[] { null, null, "ADM_COMPANY_CARD_CUSTOMERS" });
            //tables = database.connection.GetSchema("Indexes", new String[] { null, null, "ADM_BATCH_START_SEQUENCES" });
            tables = database.connection.GetSchema("Indexes", new String[] { null, null, "EXPLAIN_STATEMENT" });
            //tables = database.connection.GetSchema("Views");
            //tables = database.connection.GetSchema("Columns", new String[] { null, null, "MNU_NODES" });


            int rowIndex;
            int colIndex;

            for (colIndex = 0; colIndex < tables.Columns.Count; colIndex++)
                log.log(Logger.LogLevel.info, tables.Columns[colIndex].ColumnName);

            log.log(Logger.LogLevel.info, "");
            log.log(Logger.LogLevel.info, "");

            string text;

            for (rowIndex = 0; rowIndex < tables.Rows.Count; rowIndex++)
            {
                text = tables.Rows[rowIndex].ItemArray[0].ToString();

                for (colIndex = 1; colIndex < tables.Columns.Count; colIndex++)
                {
                    text += " - ";
                    text += tables.Rows[rowIndex].ItemArray[colIndex].ToString();
                }

                log.log(Logger.LogLevel.info, text);
            }
        }

        public TransactionScript(Source sourceSource, Target targetTarget, Configuration config, DoneDelegate doneHandler)// Button GoButton)
        {
            this.sourceSource = sourceSource;
            this.targetTarget = targetTarget;
            this.config = config;
            this.doneHandler = doneHandler;
            //this.goButton = GoButton;
        }

        //20091214

        private void DbSchemaStoredProceduresLoad()
        {
            log.phaseUpdate("Stored Procedures Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING Stored Procedures FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            //source.storedprocedures.load();
            string dial;
            dial = source.dialect.ToString();
            source.storedprocedures.load(this, this.config, 1, dial);
            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING Stored Procedures FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            //target.storedprocedures.load();
            dial = target.dialect.ToString();
            target.storedprocedures.load(this, this.config, 2, dial);
            log.unindent();

        }

        private void DbSchemaStoredProceduresSync()
        {
            log.statusUpdate("StoredProcedures");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "SYNCHRONIZING StoredProcedures...");
            log.log(Logger.LogLevel.progress, "");
            log.indent();

            string key;
            StoredProcedure targetStoredProcedure;

            log.progressSet(0, source.storedprocedures.Count);

            foreach (StoredProcedure storeprocedure in source.storedprocedures.Values)
            {
                key = storeprocedure.name;

                //if (!ProcessSchema(key))
                //{
                //    continue;
                //}

                if (target.storedprocedures.ContainsKey(key))
                {
                    targetStoredProcedure = target.storedprocedures[key];

                    if (config.ddlLogging == Configuration.DdlLogging.all)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(storeprocedure, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(targetStoredProcedure, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, "");
                    }

                    //TODO 0. Should we just drop and recreate all views as a matter of course to make sure any table changes are incorporated into the views?
                    //TODO 0. The standard should be not to use * in view definitions (we could have DB Deploy check that).
                    string storeprocedureDdl = Ddl.ddl(storeprocedure, Ddl.Dialect.generic);
                    //if (storeprocedureDdl.Contains("*"))
                    //    log.log(Logger.LogLevel.warning, "StoredProcedure " + storeprocedure.name + " contains a wildcard '*' to define returned columns.  This should be avoided because the StoredProcedure may need to be dropped and recreated when underlying tables change.");

                    if (Ddl.storedproceduresEqual(storeprocedure, targetStoredProcedure)) //  storeprocedureDdl == Ddl.ddl(storeprocedure, Ddl.Dialect.generic))
                    {
                        log.log(Logger.LogLevel.progress, "StoredProcedure: " + key + " schema is the same in the source and target databases. No Syncronization is necessary.");
                    }
                    else
                    {
                        log.log(Logger.LogLevel.change, "StoredProcedure: " + key + " schema differs in the source and target databases. Syncronization is required.");

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP PROCEDURE " + key);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP PROCEDURE " + key);
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand drop = new OdbcCommand();
                                drop.Connection = target.connection;
                                drop.CommandType = CommandType.Text;
                                drop.CommandText = "DROP PROCEDURE " + key;
                                drop.CommandTimeout = 0;
                                drop.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "StoredProcedure " + key + " dropped successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to drop StoredProcedure " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(storeprocedure, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(storeprocedure, Ddl.Dialect.generic));
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand create = new OdbcCommand(Ddl.ddl(storeprocedure, target.dialect), target.connection);
                                create.CommandTimeout = 0;
                                create.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "StoredProcedure " + key + " created successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to create StoredProcedure " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.change, "StoredProcedure: " + key + " does not exist in target database. The StoredProcedure must be created.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(storeprocedure, Ddl.Dialect.generic));
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(storeprocedure, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand create = new OdbcCommand(Ddl.ddl(storeprocedure, target.dialect), target.connection);
                            create.CommandTimeout = 0;
                            create.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "StoredProcedure " + key + " created successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to create StoredProcedure " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressSet(0, target.storedprocedures.Count);

            foreach (StoredProcedure storeprocedure in target.storedprocedures.Values)
            {
                //key = view.schema + "." + view.name;
                key = storeprocedure.name;

                if (!ProcessSchema(key))
                {
                    continue;
                }

                if (!source.storedprocedures.ContainsKey(key))
                {
                    log.log(Logger.LogLevel.change, "Target StoredProcedure: " + key + " does not exist in the source data. The StoredProcedure must be deleted.");

                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "DROP PROCEDURE " + key);
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP PROCEDURE " + key);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand drop = new OdbcCommand();
                            drop.Connection = target.connection;
                            drop.CommandType = CommandType.Text;
                            drop.CommandText = "DROP PROCEDURE " + key;
                            drop.CommandTimeout = 0;
                            drop.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "StoredProcedure " + key + " dropped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to drop StoredProcedure " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);
            log.unindent();
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "STORED PROCEDURES SYNCHRONIZED.");
        }

        private void DbSchemaFunctionsLoad()
        {
            log.phaseUpdate("Functions Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING Functions FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            //source.functions.load();
            string dial;
            dial = source.dialect.ToString();
            source.functions.load(this, this.config, 1, dial);
            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING Functions FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            //target.functions.load();
            dial = target.dialect.ToString();
            target.functions.load(this, this.config, 1, dial);
            log.unindent();
        }

        private void DbSchemaTriggersLoad()
        {
            log.phaseUpdate("Triggers Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING Triggers FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            foreach (Table table in source.tables.Values)
            {
                //log.log(Logger.LogLevel.progress, "LOADING TRIGGERS FOR Source Table: " + table.name);
                table.triggers.load();
            }
            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING Triggers FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            foreach (Table table in target.tables.Values)
            {
                //log.log(Logger.LogLevel.progress, "LOADING TRIGGERS FOR Target Table: " + table.name);
                table.triggers.load();
            }
            log.unindent();
        }

        private void DbSchemaFunctionsSync()
        {
            log.statusUpdate("Functions");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "SYNCHRONIZING Functions...");
            log.log(Logger.LogLevel.progress, "");
            log.indent();

            string key;
            Function targetFunction;

            log.progressSet(0, source.functions.Count);

            foreach (Function function in source.functions.Values)
            {
                key = function.name;

                //if (!ProcessSchema(key))
                //{
                //    continue;
                //}

                if (target.functions.ContainsKey(key))
                {
                    targetFunction = target.functions[key];

                    if (config.ddlLogging == Configuration.DdlLogging.all)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(function, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(targetFunction, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, "");
                    }

                    //TODO 0. Should we just drop and recreate all views as a matter of course to make sure any table changes are incorporated into the views?
                    //TODO 0. The standard should be not to use * in view definitions (we could have DB Deploy check that).
                    string functionDdl = Ddl.ddl(function, Ddl.Dialect.generic);
                    //if (storeprocedureDdl.Contains("*"))
                    //    log.log(Logger.LogLevel.warning, "StoredProcedure " + storeprocedure.name + " contains a wildcard '*' to define returned columns.  This should be avoided because the StoredProcedure may need to be dropped and recreated when underlying tables change.");

                    if (Ddl.functionsEqual(function, targetFunction))
                    {
                        log.log(Logger.LogLevel.progress, "Function: " + key + " schema is the same in the source and target databases. No Syncronization is necessary.");
                    }
                    else
                    {
                        log.log(Logger.LogLevel.change, "Function: " + key + " schema differs in the source and target databases. Syncronization is required.");

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP FUNCTION " + key);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP FUNCTION " + key);
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand drop = new OdbcCommand();
                                drop.Connection = target.connection;
                                drop.CommandType = CommandType.Text;
                                drop.CommandText = "DROP FUNCTION " + key;
                                drop.CommandTimeout = 0;
                                drop.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "Function " + key + " dropped successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to drop Function " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(function, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(function, Ddl.Dialect.generic));
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand create = new OdbcCommand(Ddl.ddl(function, target.dialect), target.connection);
                                create.CommandTimeout = 0;
                                create.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "Function " + key + " created successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to create Function " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.change, "Function: " + key + " does not exist in target database. The function must be created.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(function, Ddl.Dialect.generic));
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(function, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand create = new OdbcCommand(Ddl.ddl(function, target.dialect), target.connection);
                            create.CommandTimeout = 0;
                            create.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Function " + key + " created successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to create Function " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressSet(0, target.functions.Count);

            foreach (Function function in target.functions.Values)
            {
                //key = view.schema + "." + view.name;
                key = function.name;

                if (!ProcessSchema(key))
                {
                    continue;
                }

                if (!source.functions.ContainsKey(key))
                {
                    log.log(Logger.LogLevel.change, "Target function: " + key + " does not exist in the source data. The function must be deleted.");

                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "DROP FUNCTION " + key);
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP FUNCTION " + key);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand drop = new OdbcCommand();
                            drop.Connection = target.connection;
                            drop.CommandType = CommandType.Text;
                            drop.CommandText = "DROP FUNCTION " + key;
                            drop.CommandTimeout = 0;
                            drop.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Function " + key + " dropped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to drop Function " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);
            log.unindent();
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "FUNCTIONS SYNCHRONIZED.");
        }

        private void DbSchemaTriggersSync()
        {
            if (config.type != 3)
            {
                log.statusUpdate("Triggers");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SYNCHRONIZING TRIGGERS...");
                log.log(Logger.LogLevel.progress, "");
            }

            log.indent();
            string key;
            Table targetTable;
            Trigger targetTrigger;

            log.progressSet(0, source.tables.Count);

            foreach (Table table in source.tables.Values)
            {
                key = table.name;

                if (!ProcessSchema(key))
                {
                    continue;
                }

                if (target.tables.ContainsKey(key))
                {
                    targetTable = target.tables[key];

                    // Check what Triggers may need to be added or updated
                    foreach (Trigger trigger in table.triggers.Values)
                    {
                        if (targetTable.triggers.ContainsKey(trigger.name))
                        {
                            targetTrigger = targetTable.triggers[trigger.name];

                            if (config.ddlLogging == Configuration.DdlLogging.all)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, Ddl.ddl(trigger, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, Ddl.ddl(targetTrigger, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, "");
                            }

                            if (Ddl.ddl(trigger, Ddl.Dialect.generic) == Ddl.ddl(targetTrigger, Ddl.Dialect.generic))
                            {
                                log.log(Logger.LogLevel.progress, "Trigger: " + trigger.name + " schema is the same in the source and target databases. No Syncronization is necessary.");
                            }
                            else
                            {
                                log.log(Logger.LogLevel.change, "Trigger: " + trigger.name + " schema differs in the source and target databases. Syncronization is required.");

                                try
                                {
                                    if (config.type == 4)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, "DROP TRIGGER " + trigger.name.ToString() + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }
                                    else
                                    {
                                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                        {
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                            log.log(Logger.LogLevel.ddlChange, "DROP TRIGGER " + trigger.name.ToString() + ";");
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        }

                                        OdbcCommand drop = new OdbcCommand("DROP TRIGGER " + trigger.name, target.connection);
                                        drop.CommandTimeout = 0;
                                        drop.ExecuteNonQuery();
                                        log.log(Logger.LogLevel.change, "Trigger " + trigger.name + " dropped successfully.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.log(Logger.LogLevel.error, "Exception occurred while trying to drop trigger " + trigger.name + ".");
                                    log.log(Logger.LogLevel.error, ex.Message);
                                    continue;
                                }

                                try
                                {
                                    if (config.type == 4)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, Ddl.ddl(trigger, Ddl.Dialect.generic) + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }
                                    else
                                    {
                                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                        {
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(trigger, Ddl.Dialect.generic) + ";");
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        }

                                        OdbcCommand create = new OdbcCommand(Ddl.ddl(trigger, target.dialect), target.connection);
                                        create.CommandTimeout = 0;
                                        create.ExecuteNonQuery();
                                        log.log(Logger.LogLevel.change, "Trigger " + trigger.name + " created successfully.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.log(Logger.LogLevel.error, "Exception occurred while trying to create trigger " + trigger.name + ".");
                                    log.log(Logger.LogLevel.error, ex.Message);
                                }
                            }

                        }
                        else
                        {
                            try
                            {
                                if (config.type == 4)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(trigger, Ddl.Dialect.generic) + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }
                                else
                                {
                                    if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, Ddl.ddl(trigger, Ddl.Dialect.generic) + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }

                                    OdbcCommand create = new OdbcCommand(Ddl.ddl(trigger, target.dialect), target.connection);
                                    create.CommandTimeout = 0;
                                    create.ExecuteNonQuery();
                                    log.log(Logger.LogLevel.change, "Trigger " + trigger.name + " created successfully.");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.log(Logger.LogLevel.error, "Exception occurred while trying to create trigger " + trigger.name + ".");
                                log.log(Logger.LogLevel.error, ex.Message);
                            }
                        }
                    }

                    // check what triggers need to be deleted.
                    foreach (Trigger trigger in targetTable.triggers.Values)
                    {
                        if (!table.triggers.ContainsKey(trigger.name))
                        {
                            try
                            {
                                if (config.type == 4)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP TRIGGER " + trigger.name + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }
                                else
                                {
                                    if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, "DROP TRIGGER " + trigger.name + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }

                                    OdbcCommand drop = new OdbcCommand();
                                    drop.Connection = target.connection;
                                    drop.CommandType = CommandType.Text;
                                    if (target.dialect == Ddl.Dialect.db2)
                                    {
                                        drop.CommandText = "DROP TRIGGER " + trigger.name;
                                    }
                                    else
                                    {
                                        drop.CommandText = "DROP TRIGGER " + trigger.table.name + "." + trigger.name;
                                    }
                                    drop.CommandTimeout = 0;
                                    drop.ExecuteNonQuery();
                                    log.log(Logger.LogLevel.change, "Trigger: " + trigger.name + " dropped successfully.");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.log(Logger.LogLevel.error, "Exception occurred while trying to drop trigger " + trigger.name + ".");
                                log.log(Logger.LogLevel.error, ex.Message);
                            }

                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.error, "Source table " + key + " was not found in the target database when trying to synchronize triggers.");
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);

            log.unindent();
            if (config.type != 3)
            {
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "TRIGGERS SYNCHRONIZED.");
            }
        }
     /*   private void DbSchemaModulesLoad()
        {
            log.phaseUpdate("Modules Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING Modules FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            //source.storedprocedures.load();
            string dial;
            dial = source.dialect.ToString();
            source.Modules.load(this, this.config, 1, dial);
            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING Modules FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            //target.storedprocedures.load();
            dial = target.dialect.ToString();
            target.Modules.load(this, this.config, 2, dial);
            log.unindent();

        } */
       /* private void DbSchemaModulesLoad()
        {
            log.phaseUpdate("Modules Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING MODULES FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            source.modules.load(this, this.config, 1);
            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING MODULES FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            target.modules.load(this, this.config, 2);
            log.unindent();

            //source.sequences.list();

        } */
        private void DbSchemaModulesLoad()
        {
            log.phaseUpdate("Modules Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING MODULES FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            //source.functions.load();
            string dial;
            dial = source.dialect.ToString();
            source.modules.load(this, this.config, 1, dial);
            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING Functions FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            //target.functions.load();
            dial = target.dialect.ToString();
            target.modules.load(this, this.config, 1, dial);
            log.unindent();
        }

      /* private void DbSchemaModulesSync()
        {
            log.statusUpdate("Modules");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "SYNCHRONIZING Modules...");
            log.log(Logger.LogLevel.progress, "");
            log.indent();

            string key;
            Module targetModule;

            log.progressSet(0, source.Modules.Count);

            foreach (Module Module in source.Modules.Values)
            {
                key = Module.name;

                //if (!ProcessSchema(key))
                //{
                //    continue;
                //}

                if (target.Modules.ContainsKey(key))
                {
                    targetModule = target.Modules[key];

                    if (config.ddlLogging == Configuration.DdlLogging.all)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(Module, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(targetModule, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, "");
                    }

                    //TODO 0. Should we just drop and recreate all views as a matter of course to make sure any table changes are incorporated into the views?
                    //TODO 0. The standard should be not to use * in view definitions (we could have DB Deploy check that).
                    string ModuleDdl = Ddl.ddl(Module, Ddl.Dialect.generic);
                    //if (storeprocedureDdl.Contains("*"))
                    //    log.log(Logger.LogLevel.warning, "StoredProcedure " + storeprocedure.name + " contains a wildcard '*' to define returned columns.  This should be avoided because the StoredProcedure may need to be dropped and recreated when underlying tables change.");

                    if (Ddl.ModulesEqual(Module, targetModule)) //  storeprocedureDdl == Ddl.ddl(storeprocedure, Ddl.Dialect.generic))
                    {
                        log.log(Logger.LogLevel.progress, "Module: " + key + " schema is the same in the source and target databases. No Syncronization is necessary.");
                    }
                    else
                    {
                        log.log(Logger.LogLevel.change, "Module: " + key + " schema differs in the source and target databases. Syncronization is required.");

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP Mod " + key);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP Mod " + key);
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand drop = new OdbcCommand();
                                drop.Connection = target.connection;
                                drop.CommandType = CommandType.Text;
                                drop.CommandText = "DROP Mod " + key;
                                drop.CommandTimeout = 0;
                                drop.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "Module " + key + " dropped successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to drop Module " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(Module, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(Module, Ddl.Dialect.generic));
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand create = new OdbcCommand(Ddl.ddl(Module, target.dialect), target.connection);
                                create.CommandTimeout = 0;
                                create.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "Module " + key + " created successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to create Module " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.change, "Module: " + key + " does not exist in target database. The Module must be created.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(Module, Ddl.Dialect.generic));
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(Module, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand create = new OdbcCommand(Ddl.ddl(Module, target.dialect), target.connection);
                            create.CommandTimeout = 0;
                            create.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Module " + key + " created successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to create Module " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressSet(0, target.Modules.Count);

            foreach (Module Module in target.Modules.Values)
            {
                //key = view.schema + "." + view.name;
                key = Module.name;

                if (!ProcessSchema(key))
                {
                    continue;
                }

                if (!source.Modules.ContainsKey(key))
                {
                    log.log(Logger.LogLevel.change, "Target Module: " + key + " does not exist in the source data. The Module must be deleted.");

                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "DROP Mod " + key);
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP Mod " + key);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand drop = new OdbcCommand();
                            drop.Connection = target.connection;
                            drop.CommandType = CommandType.Text;
                            drop.CommandText = "DROP Mod " + key;
                            drop.CommandTimeout = 0;
                            drop.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Module " + key + " dropped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to drop Module " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);
            log.unindent();
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "Modules SYNCHRONIZED.");
        }
        */

        //20091214 code added by Munwar in accordance to sequence code

        //20100720
     /*  private void DbSchemaModulesSync()
        {
            if (config.type != 3)
            {
                log.statusUpdate("Modules");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SYNCHRONIZING MODULES...");
                log.log(Logger.LogLevel.progress, "");
            }
            log.indent();

            string key;
            Module targetModule;

            log.progressSet(0, source.modules.Count);

            foreach (Module Module in source.modules.Values)
            {
                //key = view.schema + "." + view.name;
                key = Module.name;

                if (target.modules.ContainsKey(key))
                {
                    targetModule = target.modules[key];

                    if (config.ddlLogging == Configuration.DdlLogging.all)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(Module, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(targetModule, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, "");
                    }

                    if (Ddl.ddl(Module, Ddl.Dialect.generic) == Ddl.ddl(targetModule, Ddl.Dialect.generic))
                    {
                        log.log(Logger.LogLevel.progress, "Module: " + key + " schema is the same in the source and target databases. No Syncronization is necessary.");
                    }
                    else
                    {
                        log.log(Logger.LogLevel.change, "Module: " + key + " schema differs in the source and target databases. Syncronization is required.");
                        modulesDifferent.Add(key);

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP MODULE " + key + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP MODULE " + key + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand drop = new OdbcCommand();
                                drop.Connection = target.connection;
                                drop.CommandType = CommandType.Text;
                                drop.CommandText = "DROP MODULE " + key;
                                drop.CommandTimeout = 0;
                                drop.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "MODULE: " + key + " dropped successfully: ");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to drop Module " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(Module, Ddl.Dialect.generic) + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(Module, Ddl.Dialect.generic) + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand create = new OdbcCommand(Ddl.ddl(Module, target.dialect), target.connection);
                                create.CommandTimeout = 0;
                                create.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "Module: " + key + " created successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to create Module " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.change, "Module: " + key + " does not exist in target database. The Module must be created.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(Module, Ddl.Dialect.generic) + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(Module, Ddl.Dialect.generic) + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand create = new OdbcCommand(Ddl.ddl(Module, target.dialect), target.connection);
                            create.CommandTimeout = 0;
                            create.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Module: " + key + " created successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to create Module " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressSet(0, target.modules.Count);

            foreach (Module module in target.modules.Values)
            {
                //key = view.schema + "." + view.name;
                key = module.name;

                if (!source.modules.ContainsKey(key))
                {
                    log.log(Logger.LogLevel.change, "Target Module: " + key + " does not exist in the source data. The Module must be deleted.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "DROP MODULE " + key + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP MODULE " + key + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand drop = new OdbcCommand();
                            drop.Connection = target.connection;
                            drop.CommandType = CommandType.Text;
                            drop.CommandText = "DROP MODULE " + key;
                            drop.CommandTimeout = 0;
                            drop.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Module " + key + " dropped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to drop Module " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);

            log.unindent();
            if (config.type != 3)
            {
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "MODULES SYNCHRONIZED.");
            }
        }
    */
        private void DbSchemaModulesSync()
        {
            log.statusUpdate("Modules");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "SYNCHRONIZING Modules...");
            log.log(Logger.LogLevel.progress, "");
            log.indent();

            string key;
            Module targetModule;

            log.progressSet(0, source.modules.Count);

            foreach (Module module in source.modules.Values)
            {
                key = module.name;

                //if (!ProcessSchema(key))
                //{
                //    continue;
                //}

                if (target.modules.ContainsKey(key))
                {
                    targetModule = target.modules[key];

                    if (config.ddlLogging == Configuration.DdlLogging.all)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(module, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(targetModule, Ddl.Dialect.generic));
                        log.log(Logger.LogLevel.ddl, "");
                    }

                    //TODO 0. Should we just drop and recreate all views as a matter of course to make sure any table changes are incorporated into the views?
                    //TODO 0. The standard should be not to use * in view definitions (we could have DB Deploy check that).
                    string moduleDdl = Ddl.ddl(module, Ddl.Dialect.generic);
                    //if (storeprocedureDdl.Contains("*"))
                    //    log.log(Logger.LogLevel.warning, "StoredProcedure " + storeprocedure.name + " contains a wildcard '*' to define returned columns.  This should be avoided because the StoredProcedure may need to be dropped and recreated when underlying tables change.");

                    if (Ddl.moduleEqual(module, targetModule))
                    {
                        log.log(Logger.LogLevel.progress, "Module: " + key + " schema is the same in the source and target databases. No Syncronization is necessary.");
                    }
                    else
                    {
                        log.log(Logger.LogLevel.change, "Module: " + key + " schema differs in the source and target databases. Syncronization is required.");

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP MODULE " + key);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP MODULE " + key);
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand drop = new OdbcCommand();
                                drop.Connection = target.connection;
                                drop.CommandType = CommandType.Text;
                                drop.CommandText = "DROP MODULE " + key;
                                drop.CommandTimeout = 0;
                                drop.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "Module " + key + " dropped successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to drop Module " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(module, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(module, Ddl.Dialect.generic));
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                OdbcCommand create = new OdbcCommand(Ddl.ddl(module, target.dialect), target.connection);
                                create.CommandTimeout = 0;
                                create.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "Module " + key + " created successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to create Module " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.change, "Module: " + key + " does not exist in target database. The Module must be created.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(module, Ddl.Dialect.generic));
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(module, Ddl.Dialect.generic));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand create = new OdbcCommand(Ddl.ddl(module, target.dialect), target.connection);
                            create.CommandTimeout = 0;
                            create.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Module " + key + " created successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to create Module " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressSet(0, target.modules.Count);

            foreach (Module module in target.modules.Values)
            {
                //key = view.schema + "." + view.name;
                key = module.name;

                if (!ProcessSchema(key))
                {
                    continue;
                }

                if (!source.modules.ContainsKey(key))
                {
                    log.log(Logger.LogLevel.change, "Target module: " + key + " does not exist in the source data. The module must be deleted.");

                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "DROP MODULE " + key);
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP MODULE " + key);
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand drop = new OdbcCommand();
                            drop.Connection = target.connection;
                            drop.CommandType = CommandType.Text;
                            drop.CommandText = "DROP MODULE " + key;
                            drop.CommandTimeout = 0;
                            drop.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "Module " + key + " dropped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to drop Module " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);
            log.unindent();
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "MOUDLES SYNCHRONIZED.");
        }

        private void DbSchemaMQTSync()
        {
            if (config.type != 3)
            {
                log.statusUpdate("Materialized Query Tables");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SYNCHRONIZING MQTS...");
                log.log(Logger.LogLevel.progress, "");
            }
            log.indent();

            string key;
            MQT targetMQT;

            log.progressSet(0, source.mqts.Count);

            foreach (MQT mqt in source.mqts.Values)
            {
                key = mqt.name;

                //if (!ProcessSchema(key))
                //{
                //    continue;
                //}

                if (target.mqts.ContainsKey(key))
                {
                    string targetMQTsql = "";
                    string sourceMQTsql = "";
                    targetMQT = target.mqts[key];

                    if (config.ddlLogging == Configuration.DdlLogging.all)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(mqt, Ddl.Dialect.db2));
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddl, Ddl.ddl(targetMQT, Ddl.Dialect.db2));
                        log.log(Logger.LogLevel.ddl, "");
                    }

                    //TODO 0. Should we just drop and recreate all views as a matter of course to make sure any table changes are incorporated into the views?
                    //TODO 0. The standard should be not to use * in view definitions (we could have DB Deploy check that).

                    if (Ddl.mqtsEqual(mqt, targetMQT))
                    {
                        log.log(Logger.LogLevel.progress, "MQT: " + key + " schema is the same in the source and target databases. No Syncronization is necessary.");
                    }
                    else
                    {
                        log.log(Logger.LogLevel.change, "MQT: " + key + " schema differs in the source and target databases. Syncronization is required.");

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP TABLE " + key + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP TABLE " + key + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                // If tablespace has added in Target and not in Source
                                targetMQTsql = Ddl.ddl(targetMQT, target.dialect);
                                string[] targetMQTsqlValues = Regex.Split(targetMQTsql.ToUpper(), "DATA INITIALLY DEFERRED REFRESH DEFERRED");
                                if (targetMQTsqlValues[1] != "")
                                    targetMQTsql = " " + targetMQTsqlValues[1];
                                else
                                    targetMQTsql = " ";

                                // If tablespace has added in Source and not in Target
                                sourceMQTsql = Ddl.ddl(mqt, target.dialect);
                                string[] sourceMQTsqlValues = Regex.Split(sourceMQTsql.ToUpper(), "DATA INITIALLY DEFERRED REFRESH DEFERRED");
                                sourceMQTsql = " " + sourceMQTsqlValues[0] + "DATA INITIALLY DEFERRED REFRESH DEFERRED";

                                OdbcCommand drop = new OdbcCommand();
                                drop.Connection = target.connection;
                                drop.CommandType = CommandType.Text;
                                drop.CommandText = "DROP TABLE " + key;
                                drop.CommandTimeout = 0;
                                drop.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "MQT " + key + " dropped successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to drop MQT " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }

                        try
                        {
                            if (config.type == 4)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                //log.log(Logger.LogLevel.ddlChange, Ddl.ddl(mqt, Ddl.Dialect.db2) + ";");
                                log.log(Logger.LogLevel.ddlChange, sourceMQTsql + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }
                            else
                            {
                                if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    //log.log(Logger.LogLevel.ddlChange, Ddl.ddl(mqt, Ddl.Dialect.db2) + targetMQTsql + ";");
                                    log.log(Logger.LogLevel.ddlChange, sourceMQTsql + targetMQTsql + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }

                                //OdbcCommand create = new OdbcCommand(Ddl.ddl(mqt, target.dialect) + targetMQTsql, target.connection);
                                OdbcCommand create = new OdbcCommand(sourceMQTsql + targetMQTsql, target.connection);
                                create.CommandTimeout = 0;
                                create.ExecuteNonQuery();
                                log.log(Logger.LogLevel.change, "MQT " + key + " created successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.log(Logger.LogLevel.error, "Exception occurred while trying to create MQT " + key + ".");
                            log.log(Logger.LogLevel.error, ex.Message);
                        }
                    }
                }
                else
                {
                    try
                    {
                        findMQTTableSpaceSQL(key);

                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(mqt, Ddl.Dialect.db2) + TableSpaceSQL + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, Ddl.ddl(mqt, Ddl.Dialect.db2) + TableSpaceSQL + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand create = new OdbcCommand(Ddl.ddl(mqt, target.dialect), target.connection);
                            create.CommandTimeout = 0;
                            create.CommandText += TableSpaceSQL;
                            create.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "MQT " + key + " created successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to create MQT " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressSet(0, target.mqts.Count);

            foreach (MQT mqt in target.mqts.Values)
            {
                key = mqt.name;

                //if (!ProcessSchema(key))
                //{
                //    continue;
                //}

                if (!source.mqts.ContainsKey(key))
                {
                    log.log(Logger.LogLevel.change, "Target MQT: " + key + " does not exist in the source data. The MQT must be deleted.");
                    try
                    {
                        if (config.type == 4)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, "DROP TABLE " + key + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        else
                        {
                            if (config.ddlLogging >= Configuration.DdlLogging.changes)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddlChange, "DROP TABLE " + key + ";");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            }

                            OdbcCommand drop = new OdbcCommand();
                            drop.Connection = target.connection;
                            drop.CommandType = CommandType.Text;
                            drop.CommandText = "DROP TABLE " + key;
                            drop.CommandTimeout = 0;
                            drop.ExecuteNonQuery();
                            log.log(Logger.LogLevel.change, "MQT " + key + " dropped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(Logger.LogLevel.error, "Exception occurred while trying to drop MQT " + key + ".");
                        log.log(Logger.LogLevel.error, ex.Message);
                    }
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);

            log.unindent();
            if (config.type != 3)
            {
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "MQTS SYNCHRONIZED.");
            }
        }

        private void DbSchemaMQTSLoad()
        {
            log.phaseUpdate("MQT Load");
            log.statusUpdate("Source");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING MQT'S FOR SOURCE: " + source.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            source.mqts.load(this, this.config, 1);
            log.unindent();

            log.statusUpdate("Target");
            log.log(Logger.LogLevel.progress, "");
            log.log(Logger.LogLevel.progress, "LOADING MQT'S FOR TARGET: " + target.name);
            log.log(Logger.LogLevel.progress, "");

            log.indent();
            target.mqts.load(this, this.config, 2);
            log.unindent();
        }

        private void MQTRefresh(int i)
        {
            if (config.type != 3)
            {
                log.statusUpdate("Target");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "Loading MQT for refresh: " + target.name);
                log.log(Logger.LogLevel.progress, "");
            }
            log.indent();

            if (config.type != 4 && i == 1)
            {
                target.mqts.Clear();
                target.mqts.load(this, this.config, 2);
                log.log(Logger.LogLevel.change, "");
                //log.unindent();
            }

            string key;
            log.progressSet(0, target.mqts.Count);
            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
            foreach (MQT mqt in target.mqts.Values)
            {
                key = mqt.name;
                try
                {
                    if (config.type == 4)
                    {
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        log.log(Logger.LogLevel.ddlChange, " REFRESH TABLE " + key + ";");
                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                    }
                    else
                    {
                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                        {
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                            log.log(Logger.LogLevel.ddlChange, " REFRESH TABLE " + key + ";");
                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                        }
                        OdbcCommand Refresh = new OdbcCommand();
                        Refresh.Connection = target.connection;
                        Refresh.CommandType = CommandType.Text;
                        /* 
                        Refresh.CommandText = " ALTER TABLE " + key + 
                                              " ACTIVATE NOT LOGGED INITIALLY; RRFRESH TABLE " + key + 
                                              " ; COMMIT WORK;";
                        */
                        Refresh.CommandText = " REFRESH TABLE " + key;
                        Refresh.CommandTimeout = 0;
                        Refresh.ExecuteNonQuery();
                        log.log(Logger.LogLevel.change, "MQT " + key + " refreshed successfully.");
                        /* 
                        log.log(Logger.LogLevel.ddlChange, " ALTER TABLE " + key + 
                                        " ACTIVATE NOT LOGGED INITIALLY; RRFRESH TABLE " + key + 
                                        " ; COMMIT WORK;");
                        */
                    }
                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.error, "Exception occurred while trying to refresh MQT " + key + ".");
                    log.log(Logger.LogLevel.error, ex.Message);
                }
                log.progressIncrement(0);
            }
            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
            log.unindent();
            log.progressHide(0);
        }

        //20100720

        //20110204

        private void DbSchemaLoadMQTIndexes()
        {
            if (config.type != 3)
            {
                log.phaseUpdate("MQT Index Load");
                log.statusUpdate("Source");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "LOADING MQT INDEXES FOR SOURCE: " + source.name);
                log.log(Logger.LogLevel.progress, "");
            }

            log.indent();
            foreach (MQT mqt in source.mqts.Values)
                mqt.indexes.load();
            log.unindent();

            if (config.type != 3)
            {
                log.statusUpdate("Target");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "LOADING MQT INDEXES FOR TARGET: " + target.name);
                log.log(Logger.LogLevel.progress, "");
            }

            log.indent();
            foreach (MQT mqt in target.mqts.Values)
                mqt.indexes.load();
            log.unindent();
        }

        private void DbSchemaMQTIndexSync()
        {
            // Load indexes for each MQT table here...
            // This is seperated from DbSchemaLoad() because the syncing of the table schemas,
            // in particular constraints (primary keys), can cause indexes to be generated.
            DbSchemaLoadMQTIndexes();

            if (config.type != 3)
            {
                log.statusUpdate("MQT Indexes");
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "SYNCHRONIZING MQT INDEXES...");
                log.log(Logger.LogLevel.progress, "");
            }
            log.indent();

            string key;
            MQT targetMQT;
            MQTIndex targetMQTIndex;

            log.progressSet(0, source.mqts.Count);

            foreach (MQT mqt in source.mqts.Values)
            {
                key = mqt.name;

                //if (!ProcessSchema(key))
                //{
                //    continue;
                //}

                if (target.mqts.ContainsKey(key))
                {
                    targetMQT = target.mqts[key];

                    // Check what indexes may need to be added or updated
                    foreach (MQTIndex mqtindex in mqt.indexes.Values)
                    {
                        if (targetMQT.indexes.ContainsKey(mqtindex.name))
                        {
                            targetMQTIndex = targetMQT.indexes[mqtindex.name];

                            //Find MQT INDEX Column name

                            string sqlColumn = "  SELECT COLNAME " +
                                                "  FROM SYSCAT.INDEXCOLUSE " +
                                                "  WHERE INDSCHEMA = '" + mqtindex.mqt.schema + "'" +
                                                "  AND INDNAME = '" + mqtindex.name + "' ";

                            OdbcCommand colCommand = new OdbcCommand(sqlColumn, mqtindex.mqt.database.connection);
                            OdbcDataReader colReader = null;

                            string colName = "";
                            colReader = colCommand.ExecuteReader();
                            while (colReader.Read())
                            {
                                colName = (string)colReader["COLNAME"];
                            }

                            if (config.ddlLogging == Configuration.DdlLogging.all)
                            {
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, "SOURCE DDL:");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, Ddl.ddl(mqtindex, Ddl.Dialect.generic, colName));
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, "TARGET DDL:");
                                log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                log.log(Logger.LogLevel.ddl, Ddl.ddl(targetMQTIndex, Ddl.Dialect.generic, colName));
                                log.log(Logger.LogLevel.ddl, "");
                            }

                            if (Ddl.ddl(mqtindex, Ddl.Dialect.generic, colName) == Ddl.ddl(targetMQTIndex, Ddl.Dialect.generic, colName))
                            {
                                log.log(Logger.LogLevel.progress, "MQT Index: " + mqtindex.name + " schema is the same in the source and target databases. No Syncronization is necessary.");
                            }
                            else
                            {
                                log.log(Logger.LogLevel.change, "MQT Index: " + mqtindex.name + " schema differs in the source and target databases. Syncronization is required.");

                                try
                                {
                                    if (config.type == 4)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, "DROP INDEX " + mqtindex.name.ToString() + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }
                                    else
                                    {
                                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                        {
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                            log.log(Logger.LogLevel.ddlChange, "DROP INDEX " + mqtindex.name.ToString() + ";");
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        }

                                        OdbcCommand drop = new OdbcCommand("DROP INDEX " + mqtindex.name, target.connection);
                                        drop.CommandTimeout = 0;
                                        drop.ExecuteNonQuery();
                                        log.log(Logger.LogLevel.change, "MQT Index " + mqtindex.name + " dropped successfully.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.log(Logger.LogLevel.error, "Exception occurred while trying to drop MQT index " + mqtindex.name + ".");
                                    log.log(Logger.LogLevel.error, ex.Message);
                                    continue;
                                }

                                try
                                {
                                    if (config.type == 4)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, Ddl.ddl(mqtindex, Ddl.Dialect.generic, colName) + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }
                                    else
                                    {
                                        if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                        {
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                            log.log(Logger.LogLevel.ddlChange, Ddl.ddl(mqtindex, Ddl.Dialect.generic, colName) + ";");
                                            log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        }

                                        OdbcCommand create = new OdbcCommand(Ddl.ddl(mqtindex, target.dialect, colName), target.connection);
                                        create.CommandTimeout = 0;
                                        create.ExecuteNonQuery();
                                        log.log(Logger.LogLevel.change, "MQT Index " + mqtindex.name + " created successfully.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.log(Logger.LogLevel.error, "Exception occurred while trying to create MQT index " + mqtindex.name + ".");
                                    log.log(Logger.LogLevel.error, ex.Message);
                                }
                            }

                        }
                        else
                        {
                            try
                            {
                                //Find MQT INDEX Column name

                                string sqlColumn = "  SELECT COLNAME " +
                                                    "  FROM SYSCAT.INDEXCOLUSE " +
                                                    "  WHERE INDSCHEMA = '" + mqtindex.mqt.schema + "'" +
                                                    "  AND INDNAME = '" + mqtindex.name + "' ";

                                OdbcCommand colCommand = new OdbcCommand(sqlColumn, mqtindex.mqt.database.connection);
                                OdbcDataReader colReader = null;

                                string colName = "";
                                colReader = colCommand.ExecuteReader();
                                while (colReader.Read())
                                {
                                    colName = (string)colReader["COLNAME"];
                                }

                                if (config.type == 4)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, Ddl.ddl(mqtindex, Ddl.Dialect.generic, colName) + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }
                                else
                                {
                                    if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, Ddl.ddl(mqtindex, Ddl.Dialect.generic, colName) + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }

                                    OdbcCommand create = new OdbcCommand(Ddl.ddl(mqtindex, target.dialect, colName), target.connection);
                                    create.CommandTimeout = 0;
                                    create.ExecuteNonQuery();
                                    log.log(Logger.LogLevel.change, "MQT Index " + mqtindex.name + " created successfully.");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.log(Logger.LogLevel.error, "Exception occurred while trying to create MQT index " + mqtindex.name + ".");
                                log.log(Logger.LogLevel.error, ex.Message);
                            }
                        }
                    }

                    // check what MQT indexes need to be deleted.
                    foreach (MQTIndex mqtindex in targetMQT.indexes.Values)
                    {
                        if (!mqt.indexes.ContainsKey(mqtindex.name))
                        {
                            try
                            {
                                if (config.type == 4)
                                {
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    log.log(Logger.LogLevel.ddlChange, "DROP MQT INDEX " + mqtindex.name + ";");
                                    log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                }
                                else
                                {
                                    if (config.ddlLogging >= Configuration.DdlLogging.changes)
                                    {
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                        log.log(Logger.LogLevel.ddlChange, "DROP MQT INDEX " + mqtindex.name + ";");
                                        log.log(Logger.LogLevel.ddl, string.Concat(System.Collections.ArrayList.Repeat('-', 75).ToArray()));
                                    }

                                    OdbcCommand drop = new OdbcCommand();
                                    drop.Connection = target.connection;
                                    drop.CommandType = CommandType.Text;
                                    drop.CommandText = "DROP INDEX " + mqtindex.name;
                                    drop.CommandTimeout = 0;
                                    drop.ExecuteNonQuery();
                                    log.log(Logger.LogLevel.change, "MQT Index: " + mqtindex.name + " dropped successfully.");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.log(Logger.LogLevel.error, "Exception occurred while trying to drop MQT index " + mqtindex.name + ".");
                                log.log(Logger.LogLevel.error, ex.Message);
                            }
                        }
                    }
                }
                else
                {
                    log.log(Logger.LogLevel.error, "Source table " + key + " was not found in the target database when trying to synchronize MQT indexes.");
                }
                log.progressIncrement(0);
            }

            log.progressHide(0);

            log.unindent();
            if (config.type != 3)
            {
                log.log(Logger.LogLevel.progress, "");
                log.log(Logger.LogLevel.progress, "MQT INDEXES SYNCHRONIZED.");
            }
        }

        public bool findTablespaceFileExists(string name)
        {
            FileXMLStatus = false;
            if (System.IO.File.Exists(name))
                FileXMLStatus = true;

            return FileXMLStatus;
        }

        private string findTableSpaceSQL(string TN)
        {
            TableSpaceSQL = "";
            if (FileXMLStatus)
            {
                DataSet qq = new DataSet();
                qq.ReadXml(fileXMLname);

                if (qq.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < qq.Tables[0].Rows.Count; i++)
                    {
                        if (qq.Tables[0].Rows[i]["TableName"].ToString() == TN)
                        {
                            if (qq.Tables[0].Rows[i]["TableSpaceName"].ToString() != "")
                            {
                                TableSpaceSQL += " IN \"" + qq.Tables[0].Rows[i]["TableSpaceName"].ToString() + "\" ";
                            }
                            if ((qq.Tables[0].Rows[i]["IndexSpaceName"].ToString() != "") && (qq.Tables[0].Rows[i]["IndexSpaceName"].ToString() != "NV"))
                            {
                                TableSpaceSQL += " INDEX IN \"" + qq.Tables[0].Rows[i]["IndexSpaceName"].ToString() + "\" ";
                            }
                            break;
                        }
                    }
                }
            }
            return TableSpaceSQL;
        }

        private string findMQTTableSpaceSQL(string TN)
        {
            TableSpaceSQL = "";
            if (FileXMLStatus)
            {
                DataSet qq = new DataSet();
                qq.ReadXml(fileXMLname);

                if (qq.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < qq.Tables[0].Rows.Count; i++)
                    {
                        if (qq.Tables[0].Rows[i]["TableName"].ToString() == TN)
                        {
                            if (qq.Tables[0].Rows[i]["TableSpaceName"].ToString() != "")
                            {
                                TableSpaceSQL += " ENABLE QUERY OPTIMIZATION IN  \"" + qq.Tables[0].Rows[i]["TableSpaceName"].ToString() + "\" ";
                            }
                            if ((qq.Tables[0].Rows[i]["IndexSpaceName"].ToString() != "") && (qq.Tables[0].Rows[i]["IndexSpaceName"].ToString() != "NV"))
                            {
                                TableSpaceSQL += " INDEX IN \"" + qq.Tables[0].Rows[i]["IndexSpaceName"].ToString() + "\" ";
                            }
                            break;
                        }
                    }
                }
            }
            return TableSpaceSQL;
        }

        private string findPrimaryKeyColumnNameSqlServer(Table table)
        {
            OdbcCommand command = new OdbcCommand();
            command.Connection = target.connection;
            command.CommandType = CommandType.Text;

            // GET Primary Key Name 
            command.CommandText = " SELECT i.name AS IndexName FROM sys.indexes AS i INNER JOIN sys.index_columns AS ic " +
                                  " ON i.OBJECT_ID = ic.OBJECT_ID AND i.index_id = ic.index_id WHERE i.is_primary_key = 1 " +
                                  " and OBJECT_NAME(ic.OBJECT_ID) = '" + table.name + "' ";

            object value = command.ExecuteScalar();

            string pkName = "";
            //if (value == DBNull.Value)
            if (value == null)
                pkName = "";
            else
                pkName = (string)(value);

            return pkName;
        }
    }
}
