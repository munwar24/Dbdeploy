using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class MQT
    {
        public readonly Database database;
        public readonly string schema;
        public readonly string name;
        public string platformDdl;

        public readonly Columns columns;
        public readonly MQTIndexes indexes;

        public void load()
        {
            string sql = "";
            OdbcCommand query;
            OdbcDataReader reader;

            switch (database.dialect)
            {
                case Ddl.Dialect.db2:
                    sql = "SELECT TEXT " +
                          "FROM SYSCAT.VIEWS " +
                          "WHERE VIEWSCHEMA = '" +
                          schema + "' " +
                          "AND VIEWNAME = '" +
                          name + "'";

                    query = new OdbcCommand(sql, database.connection);

                    reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        platformDdl = reader.GetString(0);
                    }
                    else
                    {
                        //TOOD: 2. Log error here
                    }

                    reader.Close();
                    break;

                case Ddl.Dialect.sqlServer:
                    //TODO: 2. Should log an error here.
                default:
                    //TODO: 2. Should log an error here.
                    break;
            }
        }

        public MQT(Database database, string schema, string name)
        {
            this.database = database;
            this.schema = schema;
            this.name = name;

            indexes = new MQTIndexes(this);
            
            load();
        }
    }
}