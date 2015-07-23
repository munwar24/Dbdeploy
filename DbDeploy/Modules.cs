using System;
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
                    */
                }
            }

            sql = "SELECT * FROM SYSCAT.MODULES WHERE OWNERTYPE = 'U' AND MODULESCHEMA = '" + ModuleSchema + "';";

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
