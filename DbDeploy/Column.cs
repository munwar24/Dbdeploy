using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Column
    {
        public readonly Table table;
        public readonly string name;
        public readonly OdbcType type;
        public readonly string typeName;
        public readonly string specialTypeExtender = "";
        public readonly int columnSize;
        public readonly short decimalDigits;
        public readonly bool nullable;
        public readonly string defaultValue;
        public readonly DbIdentity identity;

        public readonly bool typexml = false;
        public readonly bool typedateTime2 = false;
        public readonly bool typedateTimeoffset = false;
        public readonly bool typegeography = false;
        public readonly bool typegeometry = false;
        public readonly bool typehierarchyid = false;
        public readonly bool typesmallmoney = false;
        public readonly bool typemoney = false;
        public readonly bool typesql_variant = false;

        public readonly OdbcParameter parameter;

        public override string ToString()
        {
            return this.name;
        }

        public Column(
            Table table, 
            string name, 
            //OdbcType type, 
            string typeName, 
            int columnSize,
            short decimalDigits,
            bool nullable,
            string defaultValue
            )
        {
            this.table = table;
            this.name = name;

            this.typeName = typeName;

            if (this.typeName == "CHAR () FOR BIT DATA") // DB2
            {
                this.typeName = "CHAR";
                this.specialTypeExtender = "FOR BIT DATA";
            }
            if (this.typeName == "VARCHAR () FOR BIT DATA") // DB2
            {
                this.typeName = "VARCHAR";
                this.specialTypeExtender = "FOR BIT DATA";
            }
            if (this.typeName == "INTEGER") // DB2
                this.typeName = "INT";
            else if (this.typeName == "CLOB") // DB2
                this.typeName = "TEXT";
            else if (this.typeName == "BLOB") // DB2
                this.typeName = "BINARY";
            else if (this.typeName == "TIMESTAMP" && table.database.dialect == Ddl.Dialect.db2)
                this.typeName = "DATETIME";
            else if (this.typeName == "int identity")  // MS SQL 2005
                this.typeName = "int";
            else if (this.typeName == "bigint identity")  // MS SQL 2005
                this.typeName = "bigint";
            else if (this.typeName == "float")  // MS SQL 2005
                this.typeName = "double";
            //else if (this.typeName == "money")  // MS SQL 2005
            //    this.typeName = "decimal";
            else if (this.typeName == "smallint identity")  // MS SQL 2008 R2
                this.typeName = "smallint";
            else if (this.typeName == "tinyint identity")  // MS SQL 2008 R2
                this.typeName = "tinyint";

            else if (this.typeName == "datetime2")  // MS SQL 2008 R2
            {
                this.typedateTime2 = true;
                this.typeName = "datetime";
            }

            else if (this.typeName == "datetimeoffset")  // MS SQL 2008 R2
            {
                this.typedateTimeoffset = true;
                this.typeName = "datetime";
            }

            else if (this.typeName == "geography")  // MS SQL 2008 R2
            {
                this.typegeography = true;
                this.typeName = "Text";
            }

            else if (this.typeName == "geometry")  // MS SQL 2008 R2
            {
                this.typegeometry = true;
                this.typeName = "Text";
            }

            else if (this.typeName == "hierarchyid")  // MS SQL 2008 R2
            {
                this.typehierarchyid = true;
                this.typeName = "Text";
            }

            else if (this.typeName == "smallmoney")  // MS SQL 2008 R2
            {
                this.typesmallmoney = true;
                this.typeName = "Double";
            }

            else if (this.typeName == "money")  // MS SQL 2008 R2
            {
                this.typemoney = true;
                this.typeName = "Double";
            }

            else if (this.typeName == "sql_variant")  // MS SQL 2008 R2
            {
                this.typesql_variant = true;
                this.typeName = "Text";
            }

            else if (this.typeName == "xml")  // MS SQL 2008 R2
            {
                this.typexml = true;
                this.typeName = "Text";
            }

            this.type = (OdbcType)Enum.Parse(typeof(OdbcType), this.typeName, true);

            this.columnSize = columnSize;
            this.decimalDigits = decimalDigits;

            this.nullable = nullable;
            this.defaultValue = defaultValue;

            parameter = new OdbcParameter("@" + this.name, this.type);
            if(this.type == OdbcType.Timestamp)
                parameter.DbType = DbType.DateTime;

            identity = DbIdentity.check(this);
        }
    }
}
