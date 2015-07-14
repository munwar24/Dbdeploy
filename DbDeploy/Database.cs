using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Database
    {
        private Logger log = Logger.Singleton;
        private string _platform;
        private Ddl.Dialect _dialect;
        private string _version;
        private Tables _tables;
        private Views _views;
        private Sequences _sequences;
        private StoredProcedures _storedprocedures;
        private Functions _functions;
        private Modules _Modules;
        private MQTS _mqts;

        public readonly string typeName;
        public readonly string server;
        public readonly string name;
        public readonly string connectionString;
        public readonly OdbcConnection connection;

        public string platform
        {
            get
            {
                return _platform;
            }
        }

        public Ddl.Dialect dialect
        {
            get
            {
                return _dialect;
            }
        }

        public string version
        {
            get
            {
                return _version;
            }
        }

        public Tables tables { get { return _tables; } }
        public Views views { get { return _views; } }
        public Sequences sequences { get { return _sequences; } }
        public StoredProcedures storedprocedures { get { return _storedprocedures; } }
        public Functions functions { get { return _functions; } }
        public Modules Modules { get { return _Modules; } }
        public MQTS mqts { get { return _mqts; } }

        public void reset()
        {
            _tables = new Tables(this);
            _views = new Views(this);
            _sequences = new Sequences(this);
            _storedprocedures = new StoredProcedures(this);
            _functions = new Functions(this);
            _Modules = new Modules(this);
            _mqts = new MQTS(this);
        }

        public void open()
        {
            log.statusUpdate(this.typeName + ": " + this.name);
            connection.Open();

            DataTable info = connection.GetSchema("DataSourceInformation");

            _platform = (string)info.Rows[0].ItemArray[1];
            _dialect = Ddl.dialectFromPlatform(platform);
            _version = (string)info.Rows[0].ItemArray[2];

            log.log(Logger.LogLevel.progress, 
                "Database open: " + 
                this.typeName + 
                ": " + 
                this.name + 
                " ON " + 
                server +
                " (" +
                platform + 
                " " + 
                version +
                ")"
                );
        }

        public void close()
        {
            connection.Close();

            log.log(Logger.LogLevel.progress,
                "Database closed: " +
                this.typeName +
                ": " +
                this.name +
                " ON " +
                server +
                " (" +
                platform +
                " " +
                version +
                ")"
                );
        }

        public Database(string typeName, string server, string name, string connectionString)
        {
            this.typeName = typeName;
            this.server = server;
            this.name = name;
            this.connectionString = connectionString;
            connection = new OdbcConnection(this.connectionString);

            _tables = new Tables(this);
            _views = new Views(this);
            _sequences = new Sequences(this);
            _storedprocedures = new StoredProcedures(this);
            _functions = new Functions(this);
            _Modules = new Modules(this);
            _mqts = new MQTS(this);
        }
    }
}
