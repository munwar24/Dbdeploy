using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class PrimaryKey
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
                    throw new Exception("Unable to determine primary key for undetermined database dialect.");
                    break;
            }
        }

        private void loadDb2()
        {
            /*
              string sql = 
               "  SELECT * " +
               "    FROM SYSCAT.KEYCOLUSE " +
               "   WHERE TABSCHEMA = '" + table.schema + "'" +
               "     AND TABNAME = '" + table.name + "' " +
               "ORDER BY COLSEQ";

            string sql =
               "  SELECT COLNAME " +
               "    FROM SYSCAT.COLUMNS " +
               "   WHERE TABSCHEMA = '" + table.schema + "'" +
               "     AND TABNAME = '" + table.name + "' " +
               "     AND KEYSEQ = 1";

            */
            string sql =
               "  SELECT K.CONSTNAME, K.COLNAME " +
               "  FROM SYSCAT.KEYCOLUSE K, SYSCAT.TABCONST T  " +
               "  WHERE K.CONSTNAME = T.CONSTNAME AND K.TABNAME = T.TABNAME  " +
               "  AND K.TABSCHEMA = T.TABSCHEMA " +
               "  AND K.TABSCHEMA = '" + table.schema + "'" +
               "  AND K.TABNAME = '" + table.name + "' " +
               "  AND T.TYPE = 'P'";

            OdbcCommand command = new OdbcCommand(sql,table.database.connection);

            OdbcDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    constraintName = (string)reader["CONSTNAME"];
                    columnNames.Add((string)reader["COLNAME"]);
                }
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Exception occurred while trying to get primary key for " + table.name + ".");
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
            /*
            string sql =
                "  SELECT * " +
                "    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE " +
                "   WHERE TABLE_SCHEMA = '" + table.schema + "'" +
                "     AND TABLE_NAME = '" + table.name + "' " +
                "ORDER BY ORDINAL_POSITION";
            */

            string sql =
                "  SELECT i.name AS CONSTRAINT_NAME," +
                "    OBJECT_NAME(ic.OBJECT_ID) AS TABLE_NAME, " +
                "    COL_NAME(ic.OBJECT_ID,ic.column_id) AS COLUMN_NAME " +
                "    FROM sys.indexes AS i INNER JOIN sys.index_columns AS ic " +
                "    ON i.OBJECT_ID = ic.OBJECT_ID " +
                "    AND i.index_id = ic.index_id " +
                "    WHERE i.is_primary_key = 1 " +
                "    AND OBJECT_NAME(ic.OBJECT_ID) = '" + table.name + "' ";
            
            OdbcCommand command = new OdbcCommand(sql, table.database.connection);

            OdbcDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    constraintName = (string)reader["CONSTRAINT_NAME"];
                    columnNames.Add((string)reader["COLUMN_NAME"]);
                }
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Exception occurred while trying to get primary key for " + table.name + ".");
                log.log(Logger.LogLevel.error, ex.Message);
            }
            finally
            {
                try { reader.Close(); }
                catch {/* Do Nothing */}
            }
        }

        public PrimaryKey(Table table)
        {
            this.table = table;
        }
    }
}
