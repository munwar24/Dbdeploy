using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Triggers : SortedList<string, Trigger>
    {
        private Logger log = Logger.Singleton;
        public Table table;

        public void load()
        {
            string SPString = "SELECT USER_NAME([so2].[uid]) AS [table_schema], ";
            SPString += "[so].[name] AS [trigger_name], ";
            SPString += "OBJECT_NAME([so].[parent_obj]) AS [table_name] ";
            SPString += "FROM sysobjects AS [so] INNER JOIN sysobjects AS so2 ON so.parent_obj = so2.Id ";
            SPString += "WHERE [so].[type] = 'TR' ";
            SPString += "AND OBJECT_NAME([so].[parent_obj]) = '" + table.name + "'";

            DataTable data = new DataTable();
            OdbcConnection cn = table.database.connection;
            OdbcCommand cmd = new OdbcCommand(SPString, cn);
            IDataReader dr = cmd.ExecuteReader();
            CustomAdapter da = new CustomAdapter();
            da.FillFromReader(data, dr); //converts a datareader into a datatable

            DataRow row;

            string schema;
            string name;
            string TriggerTable;
            Trigger newTrigger = null;

            if (data.Rows.Count > 0)
            {
                string currentName = "";
                int dataIndex;

                log.indent();

                for (dataIndex = 0; dataIndex < data.Rows.Count; dataIndex++)
                {
                    row = data.Rows[dataIndex];

                    schema = row.ItemArray[0].ToString();
                    name = row.ItemArray[1].ToString();
                    TriggerTable = row.ItemArray[2].ToString();

                    if (currentName == "" || currentName != name)
                    {
                        currentName = name;
                        newTrigger = new Trigger(table, name, TriggerTable);
                        this.Add(name, newTrigger);
                        log.log(Logger.LogLevel.progress, "Trigger Loaded: " + name + " On " + TriggerTable);
                    }
                }
                log.unindent();
            }
        }
        public Triggers(Table table)
        {
            this.table = table;
        }
    }
}
