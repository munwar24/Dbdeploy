/* using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Modules : SortedList<string, Module>
    {
        public readonly Database database;
        private Logger log = Logger.Singleton;

        public void load(TransactionScript script, Configuration config, int check)
        {
            string name;
            string sql = "";
            OdbcCommand query;
            OdbcDataReader reader;
            string ModuleSchema = "";

            if (script.source.dialect == Ddl.Dialect.db2)
            {
                string ConStr;
                if (check == 1) //Check = 1 is for Source and 2 is for Target
                    ConStr = script.source.connectionString;
                else
                    ConStr = script.target.connectionString;

                string[] ConStrValues = new string[10];
                ConStrValues = ConStr.Split(';');
                for (int count = 0; count < ConStrValues.Length; count = count + 1)
                {
                    if (ConStrValues[count].Trim() == "")
                    {
                        break;
                    }
                    if (ConStrValues[count].Substring(0, 3).ToUpper() == "UID")
                    {
                        ModuleSchema = ConStrValues[count].Replace(ConStrValues[count].Substring(0, 4), "").ToUpper();
                        //break;
                    }
                    if (ConStrValues[count].Substring(0, 5).ToUpper() == "CURRE")
                    {
                        ModuleSchema = ConStrValues[count].Replace(ConStrValues[count].Substring(0, 14), "").ToUpper();
                        //break;
                    }
                    /*
                    if (ConStrValues[count].Substring(0, 3).ToUpper() == "UID")
                    {
                        SeqSchema = ConStrValues[count].Replace(ConStrValues[count].Substring(0, 4), "").ToUpper();
                        break;
                    }
                    
                }
            }

            sql = "SELECT moduleschema,modulename,sourcebody from SYSIBM.SYSMODULES WHERE OWNERTYPE = 'U' AND MODULESCHEMA = '" + ModuleSchema + "';";

            //sql = "select * from syscat.sequences where origin = 'U';";

            query = new OdbcCommand(sql, database.connection);

            log.statusUpdate("Loading modules...");

            reader = query.ExecuteReader();

            while (reader.Read())
            {
                name = (string)reader["MODULENAME"];
                log.statusUpdate(database.typeName + ": " + name);
                log.log(Logger.LogLevel.progress, "Module loaded: " + name);
                this.Add(name, new Module(
                    (string)reader["MODULESCHEMA"],
                    name,
                    (int)reader["MODULEID"],
                   // (decimal)reader["START"],
                  //  (decimal)reader["INCREMENT"],
                    //(decimal)reader["MINVALUE"],
                  //  (decimal)reader["MAXVALUE"],
                   // ((string)reader["CYCLE"]).ToCharArray()[0],
                  //  (int)reader["CACHE"],
                  //  ((string)reader["ORDER"]).ToCharArray()[0]
                  ((string)reader["OWNERTYPE"]).ToCharArray()[0],
                  ((string)reader["MODULETYPE"]).ToCharArray()[0]
                    )
                );
            }

            log.statusUpdate("Module loaded.");

            reader.Close();
        }

        public void list()
        {
            string text;

            foreach (Module module in this.Values)
            {
                text = module.schema +
                    " - " +
                    module.name;

                log.log(Logger.LogLevel.info, text);
            }
        }

        public Modules(Database database)
        {
            this.database = database;
        }
    }
}
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Modules : SortedList<string, Module>
    {
        public readonly Database database;
        private Logger log = Logger.Singleton;

        //public void load()
        public void load(TransactionScript script, Configuration config, int check, string dial)
        {
            // Step through each function
            string SPString = "";
            if (dial.ToUpper() == "DB2")
            {
                OdbcCommand command;
                OdbcDataReader reader;
                string ConStr;
                if (check == 1) //Check = 1 is for Source and 2 is for Target
                    ConStr = script.source.connectionString;
                else
                    ConStr = script.target.connectionString;

                string[] restrictions = new string[3];

                string[] ConStrValues = new string[10];
                ConStrValues = ConStr.Split(';');
                for (int count = 0; count < ConStrValues.Length; count = count + 1)
                {
                    if (ConStrValues[count].Trim() == "")
                    {
                        break;
                    }
                    if (ConStrValues[count].Substring(0, 3).ToUpper() == "UID")
                    {
                        restrictions[1] = ConStrValues[count].Replace(ConStrValues[count].Substring(0, 4), "").ToUpper();
                        //break;
                    }
                    if (ConStrValues[count].Substring(0, 5).ToUpper() == "CURRE")
                    {
                        restrictions[1] = ConStrValues[count].Replace(ConStrValues[count].Substring(0, 14), "").ToUpper();
                        //break;
                    }
                }

                SPString = "SELECT MODULESCHEMA,MODULENAME,SOURCEBODY FROM SYSIBM.SYSMODULES WHERE OWNERTYPE = 'U' AND MODULESCHEMA = '" + restrictions[1].ToUpper() + "' " + "  ORDER BY MODULENAME ";

                command = new OdbcCommand(SPString, database.connection);
                reader = null;
                reader = command.ExecuteReader();

                string schema;
                string name;
                string catalog = "";
                //int dataIndex=0;
                log.log(Logger.LogLevel.progress, "");
                while (reader.Read())
                {
                    name = ((string)reader["MODULENAME"]);
                    schema = restrictions[1].ToUpper();
                    if (script.ProcessSchema(name) || script.ProcessData(name))
                    {
                        log.log(Logger.LogLevel.progress, "Module loaded: " + name);
                        log.statusUpdate(database.typeName + ": " + name);
                        this.Add(name, new Module(database, catalog, schema, name));
                    }
                }
            }
        }
           
      
        /*    else
            {
                SPString = "SELECT SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME FROM INFORMATION_SCHEMA.ROUTINES ";
                SPString += " WHERE ROUTINE_TYPE = 'FUNCTION' ORDER BY SPECIFIC_NAME";

                DataTable data = new DataTable();
                OdbcConnection cn = database.connection;
                OdbcCommand cmd = new OdbcCommand(SPString, cn);
                IDataReader dr = cmd.ExecuteReader();
                CustomAdapter da = new CustomAdapter();
                da.FillFromReader(data, dr); //converts a datareader into a datatable

                //DataTable data = database.connection.GetSchema("Functions");
                DataRow row;

                string catalog;
                string schema;
                string name;
                string type;
                string remarks;
                string[] namearray;

                int dataIndex;

                log.progressSet(0, data.Rows.Count);

                for (dataIndex = 0; dataIndex < data.Rows.Count; dataIndex++)
                {
                    row = data.Rows[dataIndex];

                    catalog = row.ItemArray[0].ToString();
                    schema = row.ItemArray[1].ToString();
                    name = row.ItemArray[2].ToString();
                    namearray = name.Split(';');
                    name = namearray[0];
                    //type = row.ItemArray[3].ToString();
                    //remarks = row.ItemArray[4].ToString();

                    //log.statusUpdate(database.typeName + ": " + schema + "." + name);
                    log.statusUpdate(database.typeName + ": " + name);

                    if (schema != "SYSIBM"
                        && schema != "SYSIBMADM"
                        && schema != "SYSCAT"
                        && schema != "SYSSTAT"
                        && schema != "SYSTOOLS"
                        && schema != "TOOLS_CATALOG"
                        && schema != "INFORMATION_SCHEMA"
                        && schema != "sys"
                        )
                    {
                        //log.log("View loaded: " + schema + "." + name);
                        //this.Add(schema + "." + name, new View(database, catalog, schema, name, type, remarks));
                        //this.Add(name, new StoredProcedure(database, catalog, schema, name, type, remarks));

                        log.log(Logger.LogLevel.progress, "Functions loaded: " + name);
                        this.Add(name, new Function(database, catalog, schema, name));
                    }

                    log.progressUpdate(0, dataIndex);
                }
            }
            log.progressHide(0);
        } */

        public void list()
        {
            string text;

            foreach (Module Module in this.Values)
            {
                text = Module.catalog +
                    " - " +
                    Module.schema +
                    " - " +
                    Module.name; //+ " - " + Function.type + " - " + Function.remarks;

                log.log(Logger.LogLevel.info, text);
            }
        }

        public Modules(Database database)
        {
            this.database = database;
        }
    }
}
