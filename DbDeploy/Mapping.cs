using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class Mapping
    {
        public readonly Column fromColumn;
        public readonly Column toColumn;

        public override string ToString()
        {
            return fromColumn.name + " --> " + toColumn.name;
        }

        public Mapping(Column fromColumn, Column toColumn)
        {
            this.fromColumn = fromColumn;
            this.toColumn = toColumn;
        }
    }
}
