using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{

    class Table
    {
        public readonly Database database;
        public readonly string catalog;
        public readonly string schema;
        public readonly string name;
        public readonly string type;
        public readonly string remarks;

        public readonly Columns columns;
        public readonly PrimaryKey primaryKey;
        public readonly ForeignKey foreignKey;
        public readonly Indexes indexes;
        public readonly Triggers triggers;

        public readonly OdbcCommand insert;

        public Table(Database database, string catalog, string schema, string name, string type, string remarks, string dial)
        {
            this.database = database;
            this.catalog = catalog;
            this.schema = schema;
            this.name = name;
            this.type = type;
            this.remarks = remarks;

            columns = new Columns(this);
            columns.load();

            primaryKey = new PrimaryKey(this);
            primaryKey.load();

            //foreignKey = new ForeignKey(this);
            //foreignKey.load();

            indexes = new Indexes(this);

            triggers = new Triggers(this);

            insert = new OdbcCommand();

            //insert.CommandText = "insert into " + schema + "." + name;
            insert.CommandText = "insert into " + name;

            insert.CommandText += "(";

            int ordinal = 0;

            if (dial.ToUpper() == "DB2")
            {
                foreach (Column column in columns.Values)
                {
                    if (ordinal != 0)
                        insert.CommandText += ",";
                    insert.CommandText += column.name;
                    ordinal++;
                }
            }
            else
            {
                foreach (Column column in columns.Values)
                {
                    if (ordinal != 0)
                        insert.CommandText += ",";
                    insert.CommandText += "[" + column.name + "]";
                    ordinal++;
                }
            }

            insert.CommandText += ") values (";

            insert.CommandType = CommandType.Text;
            insert.Connection = database.connection;

            ordinal = 0;

            foreach (Column column in columns.Values)
            {
                if (ordinal != 0)
                    insert.CommandText += ",";
                // ODBC does not support named parameters
                //insert.CommandText += "@" + column.name;
                insert.CommandText += "?";
                ordinal++;

                insert.Parameters.Add(column.parameter);
            }

            insert.CommandText += ")";
        }
    }
}
