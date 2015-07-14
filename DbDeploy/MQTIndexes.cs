using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class MQTIndexes : SortedList<string, MQTIndex>
    {
        private Logger log = Logger.Singleton;

        public MQT mqt;

        public void load()
        {
            // Step through each column

            DataTable data = mqt.database.connection.GetSchema(
                "Indexes",
                new string[] { null, mqt.schema, mqt.name }
                );

            DataRow row;

            bool nonUnique;
            string schema; // Qualifer
            string name;
            short type;
            short ordinalPosition;
            string columnName;
            string ascOrDesc;
            MQTIndex newIndex = null;

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
                    ascOrDesc = (String)row.ItemArray[9];

                    if (currentName == "" || currentName != name)
                    {
                        currentName = name;
                        newIndex = new MQTIndex(mqt, name, nonUnique, type);
                        this.Add(name, newIndex);
                    }

                    newIndex.columns.Add(columnName, new MQTIndexColumn(columnName, ordinalPosition, ascOrDesc));
                }
            }
        }

        public MQTIndexes(MQT mqt)
        {
            this.mqt = mqt;
        }

    }
}
