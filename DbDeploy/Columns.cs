using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    //class Columns : SortedList<string, Column>
    class Columns : Dictionary<string, Column>
    {
        private bool _hasSpecialTypes = false;

        public Table table;
        public Column identity;

        public bool hasSpecialTypes
        {
            get { return _hasSpecialTypes; }
        }

        public void load()
        {
            // Step through each column
            
            DataTable data = table.database.connection.GetSchema(
                "Columns", 
                new string[]{ null, table.schema, table.name } 
                );
            /*
            DataTable data = table.database.connection.GetSchema(
                "Columns",
                new string[] { null, null, table.name }
                );
            */
            DataRow row;

            string name;
            OdbcType type;
            string typeName;
            int columnSize;
            string bufferLength;
            short decimalDigits;
            string numPrecRadix;
            bool nullable;
            string remarks;
            string columnDef;
            string sqlDataType;
            string sqlDatetimeSub;
            string charOctetLength;
            string ordinalPosition;
            string isNullable;
            Column newColumn;

            int dataIndex;

            for (dataIndex = 0; dataIndex < data.Rows.Count; dataIndex++)
            {
                row = data.Rows[dataIndex];

                name = (string)row.ItemArray[3];
                //type = (OdbcType)((short)(row.ItemArray[4]));
                typeName = (string)row.ItemArray[5];

                if (typeName == "CLOB")
                    _hasSpecialTypes = true;

                columnSize = (int)row.ItemArray[6];

                if (row.IsNull(8))
                    decimalDigits = 0;
                else
                    decimalDigits = (short)row.ItemArray[8];

                if ((short)(row.ItemArray[10]) != 0)
                    nullable = true;
                else
                    nullable = false;

                if (row.ItemArray[12] == System.DBNull.Value )
                    columnDef = null;
                else
                    columnDef = ((string)(row.ItemArray[12]));

                newColumn = new Column(
                    table, 
                    name, 
                    //type, 
                    typeName, 
                    columnSize,
                    decimalDigits,
                    nullable,
                    columnDef
                    );

                if (newColumn.identity != null)
                    identity = newColumn;

                this.Add(name, newColumn);
            }
        }

        public void list(RichTextBox logText)
        {
            string text; 

            foreach (Column column in this.Values)
            {
                text = column.name +
                    " - " +
                    column.type.ToString() +
                    " - " +
                    column.typeName +
                    " - " +
                    column.nullable.ToString();
            }
        }

        public Columns(Table table)
        {
            this.table = table;
        }
    }
}
