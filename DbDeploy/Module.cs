/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class Module
    {
        public readonly string schema;
        public readonly string name;
        public readonly int moduleId;
        public readonly char ownertype;
        public readonly char moduletype;
       // public readonly decimal startWith;
       // public readonly decimal increment;
      //  public readonly decimal minValue;
       // public readonly decimal maxValue;
      //  public readonly char cycle;
      ///  public readonly int cache;
       // public readonly char order;

        public Module(
            string schema,
            string name,
            int moduleId,
           // decimal startWith,
          //  decimal increment,
         //   decimal minValue,
          //  decimal maxValue,
         //   char cycle,
          //  int cache,
         //   char order
            char ownertype,
                char moduletype
        )
        {
            this.schema = schema;
            this.name = name;
            this.moduleId = moduleId;
            this.ownertype = ownertype;
            this.moduletype = moduletype;
          //  this.startWith = startWith;
          //  this.increment = increment;
         //   this.minValue = minValue;
          //  this.maxValue = maxValue;
         //   this.cycle = cycle;
          //  this.cache = cache;
         //   this.order = order;
        }
    }

} */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Module
    {
        public readonly Database database;
        public readonly string catalog;
        public readonly string schema;
        public readonly string name;
       // public readonly string type;
       // public readonly string remarks;
        public string platformDdl;

        public void load()
        {
        string sql = "";
            OdbcCommand query;
            OdbcDataReader reader;

            switch (database.dialect)
            {
                case Ddl.Dialect.db2:

                    sql = "SELECT SOURCEBODY " +
                          "FROM SYSIBM.SYSMODULES " +
                          "WHERE OWNERTYPE = 'U' AND MODULESCHEMA = '" +
                          schema + "' " +
                          "AND MODULENAME = '" +
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
        }

        //public StoredProcedure(Database database, string catalog, string schema, string name, string type, string remarks)
        public Module(Database database, string catalog, string schema, string name)
        {
            this.database = database;
            this.catalog = catalog;
            this.schema = schema;
            this.name = name;
            //this.type = type;
            //this.remarks = remarks;
            load();
        }
    }
}

