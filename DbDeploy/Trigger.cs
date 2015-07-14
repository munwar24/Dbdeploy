using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    class Trigger
    {
        public readonly Table table;
        public readonly string name;
        public readonly string TriggerTable;

        public Trigger(Table table, string name, string TriggerTable)
        {
            this.table = table;
            this.name = name;
            this.TriggerTable = TriggerTable;
        }
    }
}
