using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class IndexColumn
    {
        public readonly string name;
        public readonly short ordinalPosition;
        public readonly string ascOrDesc;

        public IndexColumn(string name, short ordinalPosition, string ascOrDesc)
        {
            this.name = name;
            this.ordinalPosition = ordinalPosition;
            this.ascOrDesc = ascOrDesc;
        }
    }
}
