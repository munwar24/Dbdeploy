using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Sequences : SortedList<string, Sequence>
    {
        public readonly Database database;
        private Logger log = Logger.Singleton;

        public void load(TransactionScript script, Configuration config, int check)
        {
            string name;
            string sql = "";
            OdbcCommand query;
            OdbcDataReader reader;
            string SeqSchema = "";

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
                        SeqSchema = ConStrValues[count].Replace(ConStrValues[count].Substring(0, 4), "").ToUpper();
                        //break;
                    }
                    if (ConStrValues[count].Substring(0, 5).ToUpper() == "CURRE")
                    {
                        SeqSchema = ConStrValues[count].Replace(ConStrValues[count].Substring(0, 14), "").ToUpper();
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

            sql = "SELECT * FROM SYSCAT.SEQUENCES WHERE ORIGIN = 'U' AND SEQSCHEMA = '" + SeqSchema + "';";

            //sql = "select * from syscat.sequences where origin = 'U';";

            query = new OdbcCommand(sql, database.connection);

            log.statusUpdate("Loading sequences...");

            reader = query.ExecuteReader();

            while (reader.Read())
            {
                name = (string)reader["SEQNAME"];
                log.statusUpdate(database.typeName + ": " + name);
                log.log(Logger.LogLevel.progress, "Sequence loaded: " + name);
                this.Add(name, new Sequence(
                    (string)reader["SEQSCHEMA"],
                    name,
                    (int)reader["DATATYPEID"],
                    (decimal)reader["START"],
                    (decimal)reader["INCREMENT"],
                    (decimal)reader["MINVALUE"],
                    (decimal)reader["MAXVALUE"],
                    ((string)reader["CYCLE"]).ToCharArray()[0],
                    (int)reader["CACHE"],
                    ((string)reader["ORDER"]).ToCharArray()[0]
                    )
                );
            }

            log.statusUpdate("Sequences loaded.");

            reader.Close();
        }

        public void list()
        {
            string text;

            foreach (Sequence sequence in this.Values)
            {
                text = sequence.schema +
                    " - " +
                    sequence.name;

                log.log(Logger.LogLevel.info, text);
            }
        }

        public Sequences(Database database)
        {
            this.database = database;
        }
    }
}
