using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Odbc;


namespace OmniDbDeploy
{
    class Tables : SortedList<string, Table>
    {
        public readonly Database database;
        private Logger log = Logger.Singleton;

        public void load(TransactionScript script, Configuration config, int check, string dial)
        {
            // Step through each table
            // script.source.dialect
            DataTable data;
            DataRow row;

            if (dial.ToUpper() == "DB2")
            //if (script.source.dialect == Ddl.Dialect.db2)
            {
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
                data = database.connection.GetSchema("Tables", restrictions);
                // data = database.connection.GetSchema("Tables");
            }
            else
            {
                data = database.connection.GetSchema("Tables");
            }

            // DataTable data = database.connection.GetSchema("Tables");
            // DataRow row;

            string catalog;
            string schema;
            string name;
            string type;
            string remarks;

            int dataIndex;

            log.progressSet(0, data.Rows.Count);

            for (dataIndex = 0; dataIndex < data.Rows.Count; dataIndex++)
            {
                row = data.Rows[dataIndex];

                catalog = row.ItemArray[0].ToString();
                schema = row.ItemArray[1].ToString();
                name = row.ItemArray[2].ToString();
                type = row.ItemArray[3].ToString();
                remarks = row.ItemArray[4].ToString();

                //log.statusUpdate(database.typeName + ": " + schema + "." + name);
                log.statusUpdate(database.typeName + ": " + name);

                if (schema != "SYSIBM"
                    && schema != "SYSTOOLS"
                    && schema != "TOOLS_CATALOG"
                    && schema != "ASN"
                    && !name.StartsWith("EXPLAIN_")
                    && !name.StartsWith("ADVISE_")
                    && !name.StartsWith("IBMSNAP_")
                    && !name.ToUpper().StartsWith("SYSDIAGRAMS")
                    //&& (config.type != 3 || script.ProcessData(name))
                    && (script.ProcessSchema(name) || script.ProcessData(name))
                    )
                {
                    //this.Add(schema + "." + name, new Table(database, catalog, schema, name, type, remarks));
                    //log.log("Table loaded: " + schema + "." + name);
                    this.Add(name, new Table(database, catalog, schema, name, type, remarks, dial));
                    log.log(Logger.LogLevel.progress, "Table loaded: " + name);
                }

                log.progressUpdate(0, dataIndex);
            }
            log.progressHide(0);
        }

        public void list()
        {
            string text;

            foreach (Table table in this.Values)
            {
                text = table.catalog +
                    " - " +
                    table.schema +
                    " - " +
                    table.name +
                    " - " +
                    table.type +
                    " - " +
                    table.remarks;

                log.log(Logger.LogLevel.info, text);
            }
        }

        public Tables(Database database)
        {
            this.database = database;
        }

    }
}
