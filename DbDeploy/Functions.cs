using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Functions : SortedList<string, Function>
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

                 SPString = "SELECT FUNCSCHEMA, FUNCNAME, BODY FROM SYSCAT.FUNCTIONS WHERE ORIGIN = 'Q' AND FUNCSCHEMA = '" + restrictions[1].ToUpper() + "' " + "  ORDER BY FUNCNAME ";

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
                     name = ((string)reader["FUNCNAME"]);
                     schema = restrictions[1].ToUpper();
                     if (script.ProcessSchema(name) || script.ProcessData(name))
                     {
                         log.log(Logger.LogLevel.progress, "Function loaded: " + name);
                         log.statusUpdate(database.typeName + ": " + name);
                         this.Add(name, new Function(database, catalog, schema, name));
                     }
                 }
             }
             else
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
        }

        public void list()
        {
            string text;

            foreach (Function Function in this.Values)
            {
                text = Function.catalog +
                    " - " +
                    Function.schema +
                    " - " +
                    Function.name; //+ " - " + Function.type + " - " + Function.remarks;

                log.log(Logger.LogLevel.info, text);
            }
        }

        public Functions(Database database)
        {
            this.database = database;
        }
    }
}
