using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class TableSelect
    {
        public bool active;
        public string name;

        public override string ToString()
        {
            return this.name;
        }

        public TableSelect(bool active, string name)
        {
            this.active = active;
            this.name = name;
        }
    }
}
