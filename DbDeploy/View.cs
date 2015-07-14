using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class View
    {
        public readonly Database database;
        public readonly string catalog;
        public readonly string schema;
        public readonly string name;
        public readonly string type;
        public readonly string remarks;
        public string platformDdl;

        public void load()
        {
            string sql = "";
            OdbcCommand query;
            OdbcDataReader reader;

            switch (database.dialect)
            {
                case Ddl.Dialect.db2:
                    /*
                    string sql = "select * " +
                                "from syscat.views " +
                                "where viewschema = '" +
                                schema + "' " +
                                "and viewname = '" +
                                name + "'";
                    */

                    sql = "select * " +
                          "from sysibm.views " +
                          "where table_schema = '" +
                          schema + "' " +
                          "and table_name = '" +
                          name + "'";

                    query = new OdbcCommand(sql, database.connection);

                    reader = query.ExecuteReader();

                    if (reader.Read())
                    {
                        platformDdl = reader.GetString(3);
                    }
                    else
                    {
                        //TOOD: 2. Log error here
                    }

                    reader.Close();
                    break;

                case Ddl.Dialect.sqlServer:
                    sql = "sp_helptext '" + schema + "." + name + "'";

                    query = new OdbcCommand(sql, database.connection);

                    reader = query.ExecuteReader();

                    platformDdl = "";

                    while (reader.Read())
                    {
                        platformDdl += (string)reader[0];
                    }

                    if (platformDdl.Length < 1)
                    {
                        //TOOD: 2. Log error here
                    }

                    reader.Close();
                    break;

                default:
                    //TODO: 2. Should log an error here.
                    break;
            }

            //log.log(platformDdl);
        }

        public View(Database database, string catalog, string schema, string name, string type, string remarks)
        {
            this.database = database;
            this.catalog = catalog;
            this.schema = schema;
            this.name = name;
            this.type = type;
            this.remarks = remarks;

            load();
        }
    }
}
