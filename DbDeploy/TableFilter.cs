using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class TableFilter : TableSelect
    {
        public string filter;

        public TableFilter(bool active, string name, string filter) : base(active, name)
        {
            this.filter = filter;
        }
    }
}
