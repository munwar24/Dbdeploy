using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class MQTS: SortedList<string, MQT>
    {
        public readonly Database database;
        private Logger log = Logger.Singleton;

        public void load(TransactionScript script, Configuration config, int check)
        {
            OdbcCommand command;
            OdbcDataReader reader;
            string[] restrictions = new string[3];

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
                        restrictions[1] = ConStrValues[count].Replace(ConStrValues[count].Substring(0, 4), "").ToUpper();
                        //break;
                    }
                    if (ConStrValues[count].Substring(0, 5).ToUpper() == "CURRE")
                    {
                        restrictions[1] = ConStrValues[count].Replace(ConStrValues[count].Substring(0, 14), "").ToUpper();
                        //break;
                    }
                }
            }
            string sql =
            "  SELECT * " +
            "  FROM SYSCAT.TABLES " +
            "  WHERE TABSCHEMA = '" + restrictions[1].ToUpper() + "' " +
            "  AND TYPE = 'S' " +
            "  ORDER BY TABNAME ";

            command = new OdbcCommand(sql, database.connection);
            reader = null;
            reader = command.ExecuteReader();

            string schema;
            string name;
            //int dataIndex=0;
            log.log(Logger.LogLevel.progress, "");
            while (reader.Read())
            {
                name = ((string)reader["TABNAME"]);
                schema = restrictions[1].ToUpper();
                if (script.ProcessSchema(name) || script.ProcessData(name))
                {
                    log.log(Logger.LogLevel.progress, "MQT loaded: " + name);
                    log.statusUpdate(database.typeName + ": " + name);
                    this.Add(name, new MQT(database, schema, name));
                }
            }
            log.progressHide(0);
        }

        public void list()
        {
            string text;

            foreach (MQT mqt in this.Values)
            {
                text = mqt.schema + " - " + mqt.name;
                log.log(Logger.LogLevel.info, text);
            }
        }

        public MQTS(Database database)
        {
            this.database = database;
        }
    }
}
