using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class DbIdentity
    {
        //public readonly Column column;
        public readonly decimal start;
        public readonly decimal increment;

        //TODO: Should add DB2 specific attributes here (like minvalue, maxvalue, cycle, etc)
        public readonly string generated;       // "A" = Always, "D" = Default

        //TODO: Should add SQL Server specific attribute here (last_value)

        public static DbIdentity check(Column column)
        {
            DbIdentity returnValue = null;

            Table table = column.table;
            string platform = table.database.platform;

            OdbcCommand command;
            OdbcDataReader reader;

            switch (table.database.dialect)
            {
                case Ddl.Dialect.db2:
                    command = new OdbcCommand(
                        "select cid.*, generated" +
                        " from syscat.columns col, syscat.colidentattributes cid" +
                        " where col.tabschema = '" +
                        table.schema +
                        "' and col.tabname = '" +
                        table.name +
                        "' and col.colname = '" +
                        column.name +
                        "'" +
                        " and cid.tabschema = col.tabschema" +
                        " and cid.tabname = col.tabname" +
                        " and cid.colname = col.colname",
                        table.database.connection);

                    reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        returnValue = new DbIdentity(
                            (decimal)reader["START"],
                            (decimal)reader["INCREMENT"],
                            (string)reader["GENERATED"]
                            );
                    }

                    reader.Close();
                    break;


                case Ddl.Dialect.sqlServer:
                    //TODO: Complete
                    //TODO: Check for version (pre-2005) can't get identity metrics
                    command = new OdbcCommand(
                        "select scm.name schemaName, " +
                            "tbl.name tableName, " +
                            "idc.name columnName, " +
                            "seed_value seedValue, " +
                            "increment_value incrementValue " +
                        "from sys.schemas scm, " +
                            "sys.tables tbl, " +
                            "sys.identity_columns idc " +
                        "where tbl.schema_id = scm.schema_id " +
                            "and idc.object_id = tbl.object_id " +
                            "and scm.name = '" + table.schema + "' " +
                            "and tbl.name = '" + table.name + "' " +
                            "and idc.name = '" + column.name + "' ",
                        table.database.connection);

                    reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        if( column.type == OdbcType.BigInt )
                            returnValue = new DbIdentity(
                                (decimal)(long)reader["seedValue"],
                                (decimal)(long)reader["incrementValue"],
                                ""
                                );
                        // MS SQL 2008 R2
                        else if (column.type == OdbcType.SmallInt)
                            returnValue = new DbIdentity(
                                (decimal)(Int16)reader["seedValue"],
                                (decimal)(Int16)reader["incrementValue"],
                                ""
                                );
                        // MS SQL 2008 R2
                        else if (column.type == OdbcType.TinyInt)
                            returnValue = new DbIdentity(
                                (decimal)(byte)reader["seedValue"],
                                (decimal)(byte)reader["incrementValue"],
                                ""
                                );
                        else
                            returnValue = new DbIdentity(
                                (decimal)(int)reader["seedValue"],
                                (decimal)(int)reader["incrementValue"],
                                ""
                                );
                    }

                    reader.Close();
                    break;
            }

            return returnValue;
        }

        public DbIdentity(decimal start, decimal increment, string generated)
        {
            this.start = start;
            this.increment = increment;
            this.generated = generated;
        }
    }
}
