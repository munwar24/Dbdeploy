using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class Alteration
    {
        public Table masterTable;
        public Table targetTable;
        public Ddl.Dialect dialect;
        public string ddl;
        public string ddlPrimaryKeyDrop = "";
        public string ddlPrimaryKeyAdd = "";
        public string ddlForeignKeyDrop = "";
        public string ddlForeignKeyAdd = "";
        public Dictionary<string, Column> dropColumns = new Dictionary<string, Column>();
        public Dictionary<string, Column> addColumns = new Dictionary<string, Column>();
        public Dictionary<string, Column> commonColumns = new Dictionary<string, Column>();
        public List<ColumnAlterCandidate> alterColumns = new List<ColumnAlterCandidate>();
        public List<Mapping> mappings = new List<Mapping>();

        public Alteration(Table masterTable, Table targetTable, Ddl.Dialect dialect)
        {
            this.masterTable = masterTable;
            this.targetTable = targetTable;
            this.dialect = dialect;
        }
    }
}
