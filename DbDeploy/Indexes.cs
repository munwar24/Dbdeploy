using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Indexes : SortedList<string, Index>
    {
        private Logger log = Logger.Singleton;

        public Table table;

        public void load()
        {
            // Step through each column
            
            DataTable data = table.database.connection.GetSchema(
                "Indexes", 
                new string[]{ null, table.schema, table.name } 
                );
            /*
            DataTable data = table.database.connection.GetSchema(
                "Indexes",
                new string[] { null, null, table.name }
                );
            */
            DataRow row;

            bool nonUnique;
            string schema; // Qualifer
            string name;
            short type;
            short ordinalPosition;
            string columnName;
            string ascOrDesc;
            Index newIndex = null;

            string sql = "";

            if (data.Rows.Count > 0)
            {
                string currentName = "";
                int dataIndex;

                for (dataIndex = 0; dataIndex < data.Rows.Count; dataIndex++)
                {
                    row = data.Rows[dataIndex];

                    if (((short)row.ItemArray[3]) == 0)
                        nonUnique = false;
                    else
                        nonUnique = true;

                    schema = (string)row.ItemArray[4];
                    name = (string)row.ItemArray[5];
                    type = (short)row.ItemArray[6];
                    ordinalPosition = (short)row.ItemArray[7];
                    columnName = (string)row.ItemArray[8];
                    //ascOrDesc = (String)row.ItemArray[9];
                    if (row.ItemArray[9] is System.DBNull)
                        ascOrDesc = "A";
                    else
                        ascOrDesc = (String)row.ItemArray[9];

                    if (currentName == "" || currentName != name)
                        {
                            currentName = name;
                            newIndex = new Index(table, name, nonUnique, type);
                            this.Add(name, newIndex);
                        }

                    //newIndex.columns.Add(columnName, new IndexColumn(columnName, ordinalPosition, ascOrDesc));
                    newIndex.columns.Add(ordinalPosition, new IndexColumn(columnName, ordinalPosition, ascOrDesc));
                }

                switch(table.database.dialect)
                {
                    case Ddl.Dialect.db2:
                        sql =
                            "  SELECT INDNAME IndexName " +
                            "    FROM SYSCAT.INDEXES " +
                            "   WHERE TABSCHEMA = '" + table.schema + "'" +
                            "     AND TABNAME = '" + table.name + "' " +
                            "     AND UNIQUERULE = 'P'";
                        break;

                    case Ddl.Dialect.sqlServer:
                        sql =
                            "  select obj1.name IndexName" +
                            "    from sys.objects obj1 " +
                            "    join sys.objects obj2 on obj2.object_id = obj1.parent_object_id " +
                            "   where obj1.type = 'PK' " +
                            "     and obj2.name = '" + table.name + "'";
                        break;
                }
                OdbcCommand command = new OdbcCommand(sql, table.database.connection);

                OdbcDataReader reader = null;

                string indexName;

                try
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        indexName = (string)reader["IndexName"];
                        if (this.ContainsKey(indexName))
                            this.Remove(indexName);
                    }
                }
                catch (Exception ex)
                {
                    log.log(Logger.LogLevel.error, "Exception occurred while trying to remvoe primary key from index list for " + table.name + ".");
                    log.log(Logger.LogLevel.error, ex.Message);
                }
                finally
                {
                    try { reader.Close(); }
                    catch {/* Do Nothing */}
                }
            }
        }

        /*
        public void list(RichTextBox logText)
        {
            string text; 

            foreach (Index index in this.Values)
            {
                text = index.name;
            }
        }
        */

        public Indexes(Table table)
        {
            this.table = table;
        }

    }
}
