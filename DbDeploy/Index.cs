using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class Index
    {
        public readonly Table table;
        public readonly string name;
        public readonly bool nonUnique;
        public readonly short type;
        //public readonly SortedList<string, IndexColumn> columns = new SortedList<string, IndexColumn>();
        public readonly SortedList<short, IndexColumn> columns = new SortedList<short, IndexColumn>();

        public Index(Table table, string name, bool nonUnique, short type)
        {
            this.table = table;
            this.name = name;
            this.nonUnique = nonUnique;
            this.type = type;
        }
    }
}
