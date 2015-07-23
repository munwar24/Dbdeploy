using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Text.RegularExpressions;

namespace OmniDbDeploy
{
    class Ddl
    {
        public enum Dialect
        {
            generic,
            sqlServer,
            db2
        }

        public static Dialect dialectFromPlatform(string platform)
        {
            if (platform.StartsWith("DB2"))
                return Dialect.db2;
            else if (platform.StartsWith("Microsoft"))
                return Dialect.sqlServer;

            return Dialect.generic;
        }

        public static string ddl(Table table, Dialect dialect)
        {
            //string sql = "CREATE TABLE " + table.schema + "." + table.name + "\r\n(\r\n";
            string sql = "CREATE TABLE " + table.name + "\r\n(\r\n";

            int ordinal = 0;

            foreach (Column column in table.columns.Values)
            {
                if (ordinal++ != 0)
                    sql += ",\r\n";

                sql += columnDefinition(column, dialect);
            }

            if (table.primaryKey.columnNames.Count > 0)
                sql += ",\r\n" + primaryKeyConstraint(table,dialect);

            //if (table.foreignKey.columnNames.Count > 0)
            //    sql += ",\r\n" + foreignKeyConstraint(table, dialect); sql += "\r\n)\r\n";
            sql += "\r\n)\r\n";

            if (dialect != Dialect.db2)
            {
                sql = sql.Replace("VARCHAR(0)", "VARCHAR(max)");
                sql = sql.Replace("VARBINARY(0)", "VARBINARY(max)");
            }

            return sql;
        }

        public static string primaryKeyConstraint(Table table, Dialect dialect)
        {
            string sql = "";

            if (table.primaryKey.columnNames.Count > 0)
            {
                if (dialect == Dialect.db2 || dialect == Dialect.generic)
                {
                    sql += " CONSTRAINT " + table.primaryKey.constraintName + " PRIMARY KEY (";
                }
                else
                {
                    sql += "    CONSTRAINT [" + table.primaryKey.constraintName + "] PRIMARY KEY (";
                }
                //sql += "    PRIMARY KEY (";
                int count = 0;
                foreach (string columnName in table.primaryKey.columnNames)
                {
                    if (count++ > 0)
                        sql += ",";
                    sql += "\"" + columnName + "\"";
                }
                sql += ")";
            }

            return sql;
        }

        public static string foreignKeyConstraint(Table table, Dialect dialect)
        {
            string sql = "";
            if (table.foreignKey.columnNames.Count > 0)
            {
                if (dialect == Dialect.db2 || dialect == Dialect.generic)
                {
                    sql += " CONSTRAINT " + table.foreignKey.columnNames[0];
                }
                else
                {
                    sql += "    CONSTRAINT [" + table.foreignKey.columnNames[0] + "]";
                }                
                sql += "    FOREIGN KEY (";
                string columnName = "";
                int countForeignKeyTotal = 1;
                int countForeignKey = 1;
                for (int count = 1; count < table.foreignKey.columnNames.Count; count++)
                {
                    columnName = table.foreignKey.columnNames[count];
                    if (countForeignKey == 2)
                    {
                        sql += ") REFERENCES ";
                        sql += "\"" + columnName.Trim() + "\"";
                        countForeignKey++;
                        countForeignKeyTotal++;
                    }
                    else if (countForeignKey == 3)
                    {
                        sql += " (";
                        sql += "\"" + columnName.Trim() + "\"";
                        countForeignKey++;
                        countForeignKeyTotal++;
                    }
                    else if (countForeignKey == 4)
                    {
                        if (countForeignKeyTotal < table.foreignKey.columnNames.Count)
                        {
                            if (dialect == Dialect.db2 || dialect == Dialect.generic)
                            {
                                sql += "), CONSTRAINT " + table.foreignKey.columnNames[countForeignKeyTotal];
                            }
                            else
                            {
                                sql += "), CONSTRAINT [" + table.foreignKey.columnNames[countForeignKeyTotal] + "]";
                            }                            
                            sql += " FOREIGN KEY (";
                        }
                        countForeignKey = 1;
                        countForeignKeyTotal++;
                    }
                    else
                    {
                        sql += "\"" + columnName.Trim() + "\"";
                        countForeignKey++;
                        countForeignKeyTotal++;
                    }
                }
            }
            sql += ") ";
            return sql;
        }

        public static string ddl(Index index, Dialect dialect)
        {
            string sql = "CREATE ";

            if (!index.nonUnique)
                sql += "UNIQUE ";

            sql += "INDEX " + index.name + " ON " + index.table.name + " (";

            int count = 0;
            foreach (IndexColumn column in index.columns.Values)
            {
                if (count++ > 0)
                    sql += ",";

                sql += "\"" + column.name + "\"";

                if (column.ascOrDesc.ToUpper() != "A")
                    sql += " DESC";
            }

            sql += ")";

            if (dialect == Dialect.db2 || dialect == Dialect.generic)
            {
                sql += "  ALLOW REVERSE SCANS   COMPRESS YES;";
            }

            return sql;
        }

        public static string ddl(View view, Dialect dialect)
        {
            string sql = "";

            if (dialect == Dialect.sqlServer)
            {
                sql = view.platformDdl;
            }
            else
            {
                // sql = view.platformDdl.Replace(view.schema + ".", "");
                sql = view.platformDdl;
                sql = System.Text.RegularExpressions.Regex.Replace(sql, @"""", "");
                sql = sql.Replace(view.schema + ".", "");
            }

            //sql = view.platformDdl.Replace(view.schema + ".","");

            return sql;
        }

        public static bool viewsEqual(View view1, View view2)
        {
            string ddl1 = Regex.Replace(ddl(view1, Dialect.generic),"\\s","");
            string ddl2 = Regex.Replace(ddl(view2, Dialect.generic),"\\s","");

            return ddl1 == ddl2;
        }

        public static string ddl(Sequence sequence, Dialect dialect)
        {
            string sql = "CREATE SEQUENCE " + sequence.name + "\r\n";

            sql += "    AS ";

            switch (sequence.dataTypeId)
            {
                case 16:
                    sql += "DECIMAL";
                    break;

                case 20:
                    sql += "BIGINT";
                    break;

                case 24:
                    sql += "INTEGER";
                    break;

                case 28:
                    sql += "SMALLINT";
                    break;

                default:
                    Logger.Singleton.log(Logger.LogLevel.error, "Unknown sequence datatype " + sequence.dataTypeId + " detected.");
                    break;
            }

            sql += "\r\n";

            sql += "    INCREMENT BY " + sequence.increment.ToString() + "\r\n";

            sql += "    ";
            if (sequence.minValue == sequence.startWith)
            {
                sql += "NO MINVALUE";
            }
            else
            {
                sql += "MINVALUE " + sequence.minValue;
            }
            sql += "\r\n";

            sql += "    ";
            // TODO: 4. Should check for other values besides BIGINT
            if (sequence.maxValue == 9223372036854775807)
            {
                sql += "NO MAXVALUE";
            }
            else
            {
                sql += "MAXVALUE " + sequence.maxValue;
            }
            sql += "\r\n";

            sql += "    ";
            if (sequence.cycle == 'N')
            {
                sql += "NO ";
            }
            sql += "CYCLE\r\n";

            sql += "    ";
            if (sequence.cache < 2)
            {
                sql += "NO CACHE";
            }
            else
            {
                sql += "CACHE " + sequence.cache;
            }
            sql += "\r\n";

            sql += "    ";
            if (sequence.order == 'N')
            {
                sql += "NO ";
            }
            sql += "ORDER\r\n";

            return sql;
        }
        public static string ddl(Module module, Dialect dialect)
        {
            string sql = "CREATE MODULE " + module.name + "\r\n";

            /*   //  sql += "    AS ";

                switch (module.moduleId)
                {
                    case 16:
                        sql += "DECIMAL";
                        break;

                    case 20:
                        sql += "BIGINT";
                        break;

                    case 24:
                        sql += "INTEGER";
                        break;

                    case 28:
                        sql += "SMALLINT";
                        break;

                    default:
                        Logger.Singleton.log(Logger.LogLevel.error, "Unknown module datatype " + module.moduleId + " detected.");
                        break;
                }

                sql += "\r\n";

                 sql += "    INCREMENT BY " + module.increment.ToString() + "\r\n";

                 sql += "    ";
                 if (module.minValue == module.startWith)
                 {
                     sql += "NO MINVALUE";
                 }
                 else
                 {
                     sql += "MINVALUE " + module.minValue;
                 }
                 sql += "\r\n";

                 sql += "    ";
                 // TODO: 4. Should check for other values besides BIGINT
                 if (module.maxValue == 9223372036854775807)
                 {
                     sql += "NO MAXVALUE";
                 }
                 else
                 {
                     sql += "MAXVALUE " + module.maxValue;
                 }
                 sql += "\r\n";
             
                sql += "    ";
                if (module.ownertype == 'U')
                {
                    sql += "NO ";
                }
                sql += "OWNERTYPE\r\n";

                sql += "    ";
                if (module.cache < 2)
                {
                    sql += "NO CACHE";
                }
                else
                {
                    sql += "CACHE " + module.cache;
                }
                sql += "\r\n"; 

            sql += "    ";
            if (module.moduletype == 'M')
            {
                sql += "NO ";
            }
            sql += "MODULETYPE\r\n"; */

            return sql;
        } 

        public static string ddl(Alteration alteration, string tempTableName)
        {
            string masterFieldsClause = "";
            string targetFieldsClause = "";

            int count = 0;
            foreach(Column column in alteration.commonColumns.Values)
            {
               if( count++ > 0 )
               {
                   masterFieldsClause += ",";
                   targetFieldsClause += ",";
               }

                masterFieldsClause += column.name;
                targetFieldsClause += column.name;
            }

            foreach(Mapping mapping in alteration.mappings)
            {
               if( count++ > 0)
               {
                   masterFieldsClause += ",";
                   targetFieldsClause += ",";
               }
                //masterFieldsClause += mapping.fromColumn.name;
                //targetFieldsClause += mapping.toColumn.name;
               masterFieldsClause += mapping.toColumn.name;
               targetFieldsClause += mapping.fromColumn.name;
            }

            string sql = "INSERT INTO " + tempTableName + "\r\n";
            sql += "(" + masterFieldsClause + ")\r\n" ;
            sql += "SELECT " + targetFieldsClause + "\r\n FROM " + alteration.targetTable.name + "\r\n";

            return sql;
        }

        public static Alteration alter(Table masterTable, Table targetTable, Dialect dialect)
        {
            Alteration alteration = new Alteration(masterTable, targetTable, dialect);

            //string sql = "CREATE TABLE " + table.schema + "." + table.name + "\r\n(\r\n";

            string sqlCmn = "";
            string sql = "";
            if (dialect == Dialect.db2)
            {
                sql = "ALTER TABLE " + targetTable.name + "\r\n";
            }
            else
            {
                sqlCmn = "ALTER TABLE " + targetTable.name + "\r\n";
                sql = "";
            }

            string noChangeSql = sql;

            //int ordinal = 0;
            
            // Drop any "extra" columns in the target table
            foreach (Column column in targetTable.columns.Values)
            {
                if (masterTable.columns.ContainsKey(column.name))
                {
                    alteration.commonColumns.Add(column.name, column);
                }
                else
                {
                    if (dialect == Dialect.db2)
                    {
                        sql += "    DROP COLUMN " + column.name + "\r\n";
                    }
                    else
                    {
                        sql += sqlCmn + "    DROP COLUMN " + column.name + "\r\n";
                    }
                    alteration.dropColumns.Add(column.name,column);
                }
            }

            // Add any "new" columns in from the master table.
            // Also update any changes between to column data types
            string colDef;
            foreach (Column column in masterTable.columns.Values)
            {
                if (targetTable.columns.ContainsKey(column.name))
                {
                    colDef = columnDefinition(targetTable.columns[column.name], dialect);
                    if (colDef != columnDefinition(column, dialect))
                    {
                        if (dialect == Dialect.db2)
                        {
                            sql += "    ALTER COLUMN " + columnDefinition(column, dialect) + "\r\n";
                        }
                        else
                        {
                            sql += sqlCmn + "    ALTER COLUMN " + columnDefinition(column, dialect) + "\r\n";
                        }

                        alteration.alterColumns.Add(new ColumnAlterCandidate(column, targetTable.columns[column.name]));
                    }
                }
                else
                {
                    if (dialect == Dialect.db2)
                        sql += "    ADD COLUMN " + columnDefinition(column, dialect) + "\r\n";
                    else
                    {
                        //sql += "    ADD " + columnDefinition(column, dialect) + "\r\n";
                        sql += sqlCmn + "    ADD " + columnDefinition(column, dialect) + "\r\n";
                    }
                    alteration.addColumns.Add(column.name,column);
                }
            }

            // Check to see if primary keys are the same
            bool primaryKeySame = false;

            //if (masterTable.primaryKey.constraintName == targetTable.primaryKey.constraintName)
            //{
            if (masterTable.primaryKey.columnNames.Count == targetTable.primaryKey.columnNames.Count)
            {
                primaryKeySame = true;
                for (int index = 0; index < masterTable.primaryKey.columnNames.Count; index++)
                {
                    if (masterTable.primaryKey.columnNames[index] != targetTable.primaryKey.columnNames[index])
                    {
                        primaryKeySame = false;
                        break;
                    }
                    if (masterTable.primaryKey.constraintName != targetTable.primaryKey.constraintName)
                    {
                        primaryKeySame = false;
                        break;
                    }
                }
            }
            //}

            alteration.ddlPrimaryKeyDrop = "";
            alteration.ddlPrimaryKeyAdd = "";

            if (!primaryKeySame)
            {
                string sqlDrop = "";
                string sqlAdd = "";
                if (dialect == Dialect.db2)
                {
                    sqlDrop = "ALTER TABLE " + targetTable.name + "\r\n" + "    DROP PRIMARY KEY\r\n";
                    sqlAdd = "ALTER TABLE " + targetTable.name + "\r\n" + "    ADD " + primaryKeyConstraint(masterTable, dialect);
                }
                else
                {
                    if (masterTable.primaryKey.constraintName != "")
                    {
                        sqlDrop = "ALTER TABLE " + targetTable.name + "\r\n" + "    DROP CONSTRAINT " + targetTable.primaryKey.constraintName + "\r\n";
                        sqlAdd = "ALTER TABLE " + targetTable.name + "\r\n" + "    ADD " + primaryKeyConstraint(masterTable, dialect);
                    }
                    if (masterTable.primaryKey.constraintName == "")
                    {
                        sqlDrop = "ALTER TABLE " + targetTable.name + "\r\n" + "    DROP CONSTRAINT " + targetTable.primaryKey.constraintName + "\r\n";
                    }
                }

                //if (masterTable.primaryKey.constraintName == targetTable.primaryKey.constraintName)
                //{
                if (masterTable.primaryKey.columnNames.Count > 0)
                {
                    if (targetTable.primaryKey.columnNames.Count > 0)
                    {
                        alteration.ddlPrimaryKeyDrop = sqlDrop;
                        alteration.ddlPrimaryKeyAdd = sqlAdd;
                    }
                    else
                    {
                        alteration.ddlPrimaryKeyAdd = sqlAdd;
                    }
                }
                else
                {
                    if (targetTable.primaryKey.columnNames.Count > 0)
                    {
                        alteration.ddlPrimaryKeyDrop = sqlDrop;
                    }
                }
                //}
                //else
                //{
                //    alteration.ddlPrimaryKeyDrop = sqlDrop;
                //    alteration.ddlPrimaryKeyAdd = sqlAdd;
                //}
            }

            /*
            // Check to see if foreign keys are the same
            bool foreignKeySame = false;

            if (masterTable.foreignKey.columnNames.Count == targetTable.foreignKey.columnNames.Count)
            {
                foreignKeySame = true;
                for (int index = 0; index < masterTable.foreignKey.columnNames.Count; index++)
                {
                    if (masterTable.foreignKey.columnNames[index] != targetTable.foreignKey.columnNames[index])
                    {
                        foreignKeySame = false;
                        break;
                    }
                }
            }

            alteration.ddlForeignKeyDrop = "";
            alteration.ddlForeignKeyAdd = "";

            if (!foreignKeySame)
            {
                string sqlDrop = "";
                string sqlAdd = "";
                for (int count = 0; count < masterTable.foreignKey.columnNames.Count; count = count + 4)
                {
                    sqlDrop += "ALTER TABLE " + targetTable.name + "\r\n" + "    DROP FOREIGN KEY\r\n" + "\r\n" + masterTable.foreignKey.columnNames[count] + ";";
                    sqlAdd += "ALTER TABLE " + targetTable.name + "\r\n" + "    ADD " + foreignKeyConstraint(masterTable, dialect);
                }

                if (masterTable.foreignKey.columnNames.Count > 0)
                {
                    if (targetTable.foreignKey.columnNames.Count > 0)
                    {
                        alteration.ddlForeignKeyDrop = sqlDrop;
                        alteration.ddlForeignKeyAdd = sqlAdd;
                    }
                    else
                    {
                        alteration.ddlForeignKeyAdd = sqlAdd;
                    }
                }
                else
                {
                    if (targetTable.foreignKey.columnNames.Count > 0)
                    {
                        for (int count = 0; count < targetTable.foreignKey.columnNames.Count; count = count + 4)
                        {
                            sqlDrop += "ALTER TABLE " + targetTable.name + "\r\n" + "    DROP FOREIGN KEY\r\n" + "\r\n" + targetTable.foreignKey.columnNames[count] + ";";
                        }
                        alteration.ddlForeignKeyDrop = sqlDrop;
                    }
                }
            }
            */


            if (sql == noChangeSql)
                sql = "";

            alteration.ddl = sql;

            return alteration;
        }

        public static string columnDefinition(Column column, Dialect dialect)
        {
            string sql = "    \"" + column.name + "\"";

            if (column.typeName.ToUpper() == "TEXT" && dialect == Dialect.db2)
                sql += " CLOB"; // Convert CLOB for DB2 platform
            else if (column.typeName.ToUpper() == "BINARY" && dialect == Dialect.db2)
                sql += " BLOB"; // Convert BLOB for DB2 platform
            else if (column.typeName.ToUpper() == "DATETIME" && dialect == Dialect.db2)
                sql += " TIMESTAMP"; // Convert TIMESTAMP for DB2 platform
            else if (column.typeName.ToUpper() == "DATE" && (dialect == Dialect.generic || dialect == Dialect.sqlServer))
                sql += " DATE"; // Convert DATE for SQL Server platform
            else if (column.typeName.ToUpper() == "DATETIME" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && (!column.typedateTime2) && (!column.typedateTimeoffset))
                sql += " DATETIME"; // Convert DATETIME for SQL Server platform
            else if (column.typeName.ToUpper() == "TIME" && (dialect == Dialect.generic || dialect == Dialect.sqlServer))
                sql += " TIME"; // Convert TIME for SQL Server platform
            else if (column.typeName.ToUpper() == "DOUBLE" && dialect == Dialect.sqlServer && (!column.typesmallmoney) && (!column.typemoney))
                sql += " FLOAT"; // Convert FLOAT for SQL Server platform

            else if (column.typeName.ToUpper() == "TEXT" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && column.typexml)
                sql += " XML"; // Convert XML for SQL Server platform

            else if (column.typeName.ToUpper() == "DATETIME" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && column.typedateTime2)
                sql += " DATETIME2"; // Convert DATETIME2 for SQL Server platform

            else if (column.typeName.ToUpper() == "DATETIME" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && column.typedateTimeoffset)
                sql += " DATETIMEOFFSET"; // Convert DATETIMEOFFSET for SQL Server platform

            else if (column.typeName.ToUpper() == "TEXT" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && column.typegeography)
                sql += " GEOGRAPHY"; // Convert GEOGRAPHY for SQL Server platform

            else if (column.typeName.ToUpper() == "TEXT" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && column.typegeometry)
                sql += " GEOMETRY"; // Convert GEOMETRY for SQL Server platform

            else if (column.typeName.ToUpper() == "TEXT" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && column.typehierarchyid)
                sql += " HIERARCHYID"; // Convert HIERARCHYID for SQL Server platform

            else if (column.typeName.ToUpper() == "DOUBLE" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && column.typesmallmoney)
                sql += " SMALLMONEY"; // Convert SMALLMONEY for SQL Server platform

            else if (column.typeName.ToUpper() == "DOUBLE" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && column.typemoney)
                sql += " MONEY"; // Convert MONEY for SQL Server platform

            else if (column.typeName.ToUpper() == "TEXT" && (dialect == Dialect.generic || dialect == Dialect.sqlServer) && column.typesql_variant)
                sql += " SQL_VARIANT"; // Convert SQL_VARIANT for SQL Server platform

            else
                sql += " " + column.typeName.ToUpper();

            if (column.typeName.ToUpper() == "CHAR" || column.typeName.ToUpper() == "VARCHAR" || column.typeName.ToUpper() == "NVARCHAR" )
            {
                sql += "(" + column.columnSize + ")";
                if (dialect == Dialect.db2)
                {
                    sql += " " + column.specialTypeExtender;
                }
            }
            ////Added the else if condition to check the datatype varbinary
            //else  if (column.typeName.ToUpper() ==  "VARBINARY")
            //{
            //    if (column.columnSize != 0) 
            //    {
            //        sql += "(" + column.columnSize + ")";
            //    }
            //    if (column.columnSize == 0) //If column size is max then taking as columnsize 0 to overcome that issue added this condition
            //    {
            //        sql += "(" + "Max" + ")";
            //    }
            //    if (dialect == Dialect.db2)
            //    {
            //        sql += " " + column.specialTypeExtender;
            //    }
            //}
            //else if (column.typeName.ToUpper() == "DECIMAL")
            //    sql += "(" + column.columnSize + "," + column.decimalDigits + ")";
            else if (column.typeName.ToUpper() == "DECIMAL")
                sql += "(" + column.columnSize + "," + column.decimalDigits + ")";
            else if (column.typeName.ToUpper() == "NUMERIC")
                sql += "(" + column.columnSize + "," + column.decimalDigits + ")";
            else if (column.typeName.ToUpper() == "BINARY")
                sql += "(" + column.columnSize + ")";
            else if (column.typeName.ToUpper() == "NCHAR")
                sql += "(" + column.columnSize + ")";
            else if (column.typeName.ToUpper() == "VARBINARY")
                sql += "(" + column.columnSize + ")";

            switch (dialect)
            {
                case Dialect.generic:
                case Dialect.db2:
                    if (!column.nullable)
                        sql += " NOT NULL";

                    if (column.defaultValue != null)
                        sql += defaultDefinition(column);

                    if (column.identity != null)
                    {
                        if (dialect == Dialect.db2)
                        {
                            if (column.identity.generated == "A")
                                sql += " GENERATED ALWAYS";
                            else if (column.identity.generated == "D")
                                sql += " GENERATED BY DEFAULT";
                        }

                        sql += " AS IDENTITY";

                        sql += "(START WITH " + column.identity.start;
                        sql += " INCREMENT BY " + column.identity.increment;

                        if (dialect == Dialect.db2)
                        {
                            //TODO: Complete by adding in minvalue, maxvalue, no cycle, cache, order
                        }

                        sql += ")";
                    }
                    break;

                case Dialect.sqlServer:
                    if (column.identity != null)
                    {
                        sql += " IDENTITY";
                        sql += " (" + column.identity.start + "," + column.identity.increment + ")";
                    }

                    if (!column.nullable)
                    {
                        sql += " NOT NULL";
                    }
                    else
                    {
                        sql += " NULL";
                    }

                    if (column.defaultValue != null)
                        sql += defaultDefinition(column);

                    break;
            }
           
            if (dialect != Dialect.db2)
            {
                sql = sql.Replace("VARCHAR(0)", "VARCHAR(max)");
                sql = sql.Replace("VARBINARY(0)", "VARBINARY(max)");
            }

            return sql;
        }

        private static string defaultDefinition(Column column)
        {
            string sql = " DEFAULT ";

            switch( column.type )
            {
                    /*
                case OdbcType.Char:
                case OdbcType.Date:
                case OdbcType.DateTime:
                case OdbcType.NChar:
                case OdbcType.NText:
                case OdbcType.NVarChar:
                case OdbcType.SmallDateTime:
                case OdbcType.Text:
                case OdbcType.Time:
                case OdbcType.Timestamp:
                case OdbcType.VarChar:
                    sql += "'" + column.defaultValue + "'";
                    break;
                     */

                default:
                    sql += column.defaultValue.Replace("(","").Replace(")","");
                    break;
            }

            return sql;
        }

        public static string ddl(StoredProcedure storedprocedure, Dialect dialect)
        {
            string sql = "";
            if (dialect == Dialect.db2)
                sql = storedprocedure.platformDdl.Replace(storedprocedure.schema + ".", "");
            else
                sql = storedprocedure.platformDdl;

            return sql;
        }

        public static bool storedproceduresEqual(StoredProcedure storedprocedure1, StoredProcedure storedprocedure2)
        {
            string ddl1 = Regex.Replace(ddl(storedprocedure1, Dialect.generic), "\\s", "");
            string ddl2 = Regex.Replace(ddl(storedprocedure2, Dialect.generic), "\\s", "");

            return ddl1 == ddl2;
        }

        public static string ddl(Function function, Dialect dialect)
        {
            string sql = "";
            if (dialect == Dialect.db2)
                sql = function.platformDdl.Replace(function.schema + ".", "");
            else
                sql = function.platformDdl;

            return sql;
        }

        public static bool functionsEqual(Function function1, Function function2)
        {
            string ddl1 = Regex.Replace(ddl(function1, Dialect.generic), "\\s", "");
            string ddl2 = Regex.Replace(ddl(function2, Dialect.generic), "\\s", "");

            return ddl1 == ddl2;
        }
        public static string ddl(MQT mqt, Dialect dialect)
        {
            string sql = "";
            if (dialect == Dialect.db2)
            {
                //sql = mqt.platformDdl.Replace(mqt.schema + ".", "");
                sql = mqt.platformDdl;
                sql = System.Text.RegularExpressions.Regex.Replace(sql, @"""", "");
                sql = sql.Replace(mqt.schema + ".", "");
            }
            else
                sql = mqt.platformDdl;

            sql = System.Text.RegularExpressions.Regex.Replace(sql, @"\s{2,}", " ");
            return sql;
        }

        public static bool mqtsEqual(MQT mqt1, MQT mqt2)
        {
            string ddl1 = Regex.Replace(ddl(mqt1, Dialect.db2), "\\s", "");
            string[] ddl1Value = Regex.Split(ddl1.ToUpper(), "datainitiallydeferred".ToUpper());
            ddl1 = ddl1Value[0];
            
            string ddl2 = Regex.Replace(ddl(mqt2, Dialect.db2), "\\s", "");
            string[] ddl2Value = Regex.Split(ddl2.ToUpper(), "datainitiallydeferred".ToUpper());
            ddl2 = ddl2Value[0];

            return ddl1 == ddl2;
        }

        public static string ddl(MQTIndex mqtindex, Dialect dialect, string ColumnName)
        {
            string sql = "CREATE ";

            if (!mqtindex.nonUnique)
                sql += "UNIQUE ";

            sql += "INDEX " + mqtindex.name + " ON " + mqtindex.mqt.name + " (";
            //sql += "\"" + ColumnName + "\"";
            //sql += ")";
            int count = 0;
            foreach (MQTIndexColumn column in mqtindex.columns.Values)
            {
                if (count++ > 0)
                    sql += ",";

                sql += "\"" + column.name + "\"";

                if (column.ascOrDesc.ToUpper() != "A")
                    sql += " DESC";
            }

            sql += ")";

            if (dialect == Dialect.db2 || dialect == Dialect.generic)
            {
                sql += "  ALLOW REVERSE SCANS   COMPRESS YES;";
            }

            return sql;
        }

        public static string ddl(Trigger trigger, Dialect dialect)
        {
            string sql = "";
            OdbcCommand query;
            OdbcDataReader reader;
            string platformDdl = "";

            sql = "sp_helptext '" + trigger.name + "'";
            query = new OdbcCommand(sql,trigger.table.database.connection);
            reader = query.ExecuteReader();
            
            while (reader.Read())
            {
                platformDdl += (string)reader[0];
            }
            reader.Close();
            return platformDdl;
        }

        public static bool triggersEqual(Trigger trigger1, Trigger trigger2)
        {
            string ddl1 = Regex.Replace(ddl(trigger1, Dialect.generic), "\\s", "");
            string ddl2 = Regex.Replace(ddl(trigger2, Dialect.generic), "\\s", "");

            return ddl1 == ddl2;
        }

        public static string ddlTemp(Table table, Dialect dialect)
        {
            //string sql = "CREATE TABLE " + table.schema + "." + table.name + "\r\n(\r\n";
            string sql = "CREATE TABLE " + table.name + "\r\n(\r\n";

            int ordinal = 0;

            foreach (Column column in table.columns.Values)
            {
                if (ordinal++ != 0)
                    sql += ",\r\n";

                sql += columnDefinition(column, dialect);
            }

            return sql;
        }
    }
}
