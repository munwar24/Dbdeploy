using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class ForeignKeyRefresh
    {
        public readonly Database database;
        private Logger log = Logger.Singleton;
        public string sqlFKRemove = "";
        public string sqlFKAdd = "";
        private string TableSchema;

        public void load()
        {
            switch (database.dialect)
            {
                case Ddl.Dialect.db2:
                    alterDb2();
                    break;

                case Ddl.Dialect.sqlServer:
                    alterSqlServer();
                    break;

                default:
                    throw new Exception("Unable to determine foreign key for undetermined database dialect.");
                    break;
            }
        }

        private void alterDb2()
        {
            string sql =
                "  SELECT * " +
                "  FROM SYSIBM.SYSRELS " +
                "  WHERE CREATOR = '" + TableSchema.ToUpper() + "' " +
                "  AND TBNAME NOT LIKE " + "'EXPLAIN_%'" + 
                "  AND TBNAME NOT LIKE " + "'ADVISE_%'" +
                "  AND TBNAME NOT LIKE " + "'IBMSNAP_%'" +
                "  ORDER BY RELNAME ";

            OdbcCommand command = new OdbcCommand(sql, database.connection);
            OdbcDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    /*
                    sqlFKRemove += " ALTER TABLE " + "\r\n" +((string)reader["TBNAME"]) + "\r\n";
                    sqlFKRemove += " DROP FOREIGN KEY " + "\r\n\"" + ((string)reader["RELNAME"]) + "\"\r\n";
                    sqlFKRemove += ";" + "\r\n";

                    sqlFKAdd += " ALTER TABLE " + "\r\n" +((string)reader["TBNAME"]) + "\r\n";
                    sqlFKAdd += " ADD CONSTRAINT " + "\r\n" + ((string)reader["RELNAME"]) + "\r\n";
                    sqlFKAdd += " FOREIGN KEY " + "\r\n(" + ((string)reader["FKCOLNAMES"]).Trim() + ")\r\n";
                    sqlFKAdd += " REFERENCES " + "\r\n" + ((string)reader["REFTBNAME"]) + "\r\n(";
                    sqlFKAdd += ((string)reader["PKCOLNAMES"]).Trim() + ")\r\n";
                    sqlFKAdd += ";" + "\r\n";
                    */
                    sqlFKRemove += " ALTER TABLE " + ((string)reader["TBNAME"]) + " " ;
                    sqlFKRemove += " DROP FOREIGN KEY " + ((string)reader["RELNAME"]) + " ";
                    sqlFKRemove += ";" + "\r\n";

                    sqlFKAdd += "ALTER TABLE " +  ((string)reader["TBNAME"]) + " ";
                    sqlFKAdd += "ADD CONSTRAINT " +  ((string)reader["RELNAME"]) + " ";
                    sqlFKAdd += "FOREIGN KEY (" +  ((string)reader["FKCOLNAMES"]).Trim() + ") ";
                    sqlFKAdd += "REFERENCES " +  ((string)reader["REFTBNAME"]) + " (";
                    sqlFKAdd += ((string)reader["PKCOLNAMES"]).Trim() + ")";
                    sqlFKAdd += ";" + "\r\n"; ;

                }
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Exception occurred while trying to get foreign key for " + database.name + ".");
                log.log(Logger.LogLevel.error, ex.Message);
            }
            finally
            {
                try { reader.Close(); }
                catch {/* Do Nothing */}
            }
        }

        public void dropForeignKey()
        {
            OdbcCommand alter = new OdbcCommand();
            alter.CommandType = CommandType.Text;
            alter.Connection = database.connection;

            if (sqlFKRemove != "")
            {
                try
                {
                    alter.CommandText = sqlFKRemove;
                    alter.CommandTimeout = 0;
                    alter.ExecuteNonQuery();
                    log.log(Logger.LogLevel.change, " ");
                    log.log(Logger.LogLevel.change, "Dropped foreign key constraints in target database. ");
                    log.log(Logger.LogLevel.change, " ");
                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.error, "Exception occurred while trying to drop foreign key constraint. " );
                    log.log(Logger.LogLevel.error, ex.Message);
                }
            }
        }

        public void addForeignKey()
        {
            OdbcCommand alter = new OdbcCommand();
            alter.CommandType = CommandType.Text;
            alter.Connection = database.connection;

            if (sqlFKAdd != "")
            {
                try
                {
                    alter.CommandText = sqlFKAdd;
                    alter.CommandTimeout = 0;
                    alter.ExecuteNonQuery();
                    log.log(Logger.LogLevel.change, " ");
                    log.log(Logger.LogLevel.change, "Added foreign key constraints in target database. ");
                    log.log(Logger.LogLevel.change, " ");
                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.error, "Exception occurred while trying to drop foreign key constraint " + ".");
                    log.log(Logger.LogLevel.error, ex.Message);
                }
            }

        }

        private void alterSqlServer()
        {
            string sql =
                            "  SELECT f.name AS ForeignKey, " +
                            "  OBJECT_NAME(f.parent_object_id) AS TableName, " +
                            "  COL_NAME(fc.parent_object_id, " +
                            "  fc.parent_column_id) AS ColumnName, " +
                            "  OBJECT_NAME (f.referenced_object_id) AS ReferenceTableName, " +
                            "  COL_NAME(fc.referenced_object_id, " +
                            "  fc.referenced_column_id) AS ReferenceColumnName " +
                            "  FROM sys.foreign_keys AS f " +
                            "  INNER JOIN sys.foreign_key_columns AS fc " +
                            "  ON f.OBJECT_ID = fc.constraint_object_id " +
                            "  Order by ForeignKey ";
 
            OdbcCommand command = new OdbcCommand(sql, database.connection);
            OdbcDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sqlFKRemove += " ALTER TABLE " + ((string)reader["TableName"]) ;
                    sqlFKRemove += " DROP CONSTRAINT " + ((string)reader["ForeignKey"]) ;
                    sqlFKRemove += " ;" + "\r\n";

                    sqlFKAdd += " ALTER TABLE " + ((string)reader["TableName"]) ;
                    sqlFKAdd += " ADD CONSTRAINT " + ((string)reader["ForeignKey"]) ;
                    sqlFKAdd += " FOREIGN KEY " + "([" + ((string)reader["ColumnName"]).Trim() + "])";
                    sqlFKAdd += " REFERENCES " + ((string)reader["ReferenceTableName"]) + " ([";
                    sqlFKAdd += ((string)reader["ReferenceColumnName"]).Trim() + "])";
                    sqlFKAdd += " ;" + "\r\n";
                }
            }
            catch (Exception ex)
            {
                log.log(Logger.LogLevel.error, "Exception occurred while trying to get foreign key for " + database.name + ".");
                log.log(Logger.LogLevel.error, ex.Message);
            }
            finally
            {
                try { reader.Close(); }
                catch {/* Do Nothing */}
            }
        }

        public ForeignKeyRefresh(Database database,string tempTableSchema)
        {
            this.database = database;
            this.TableSchema = tempTableSchema;
        }
    }
}
