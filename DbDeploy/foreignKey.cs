using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class ForeignKey
    {

        public readonly Table table;

        public string constraintName = "";
        public readonly List<string> columnNames = new List<string>();

        private Logger log = Logger.Singleton;

        public void load()
        {
            switch (table.database.dialect)
            {
                case Ddl.Dialect.db2:
                    loadDb2();
                    break;

                case Ddl.Dialect.sqlServer:
                    loadSqlServer();
                    break;

                default:
                    throw new Exception("Unable to determine foreign key for undetermined database dialect.");
                    break;
            }
        }

        private void loadDb2()
        {
            string sql =
                "  SELECT * " +
                "  FROM SYSIBM.SYSRELS " +
                "  WHERE TBNAME = '" + table.name + "' " +
                "  ORDER BY RELNAME ";
                
            OdbcCommand command = new OdbcCommand(sql, table.database.connection);

            OdbcDataReader reader = null;

            /*
             Columns Heading
             * CREATOR, TBNAME, RELNAME, REFTBNAME, REFTBCREATOR, COLCOUNT, 
             * DELETERULE, UPDATERULE, TIMESTAMP, FKCOLNAMES, PKCOLNAMES, 
             * REFKEYNAME, DEFINER, DEFINERTYPE
             */
            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //constraintName = (string)reader["RELNAME"];
                    columnNames.Add((string)reader["RELNAME"]);
                    columnNames.Add((string)reader["FKCOLNAMES"]);
                    columnNames.Add((string)reader["REFTBNAME"]);
                    columnNames.Add((string)reader["PKCOLNAMES"]);
                }
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Exception occurred while trying to get foreign key for " + table.name + ".");
                log.log(Logger.LogLevel.error, ex.Message);
            }
            finally
            {
                try { reader.Close(); }
                catch {/* Do Nothing */}
            }
        }

        private void loadSqlServer()
        {
            string sql =
                "  SELECT f.name AS RELNAME," +
                "    OBJECT_NAME(f.parent_object_id) AS TableName, " +
                "    COL_NAME(fc.parent_object_id, " +
                "    fc.parent_column_id) AS FKCOLNAMES, " +
                "    OBJECT_NAME (f.referenced_object_id) AS REFTBNAME, " +
                "    COL_NAME(fc.referenced_object_id, " +
                "    fc.referenced_column_id) AS PKCOLNAMES " +
                "    FROM sys.foreign_keys AS f " +
                "    INNER JOIN sys.foreign_key_columns AS fc " +
                "    ON f.OBJECT_ID = fc.constraint_object_id " +
                "     WHERE OBJECT_NAME(f.parent_object_id) = '" + table.name + "' ";

            OdbcCommand command = new OdbcCommand(sql, table.database.connection);

            OdbcDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    constraintName = (string)reader["RELNAME"];
                    columnNames.Add((string)reader["FKCOLNAMES"]);
                    columnNames.Add((string)reader["REFTBNAME"]);
                    columnNames.Add((string)reader["PKCOLNAMES"]);
                }
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Exception occurred while trying to get foreign key for " + table.name + ".");
                log.log(Logger.LogLevel.error, ex.Message);
            }
            finally
            {
                try { reader.Close(); }
                catch {/* Do Nothing */}
            }
        }

        public ForeignKey(Table table)
        {
            this.table = table;
        }
    }
}
