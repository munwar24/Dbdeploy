using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class ColumnAlterCandidate
    {
        public readonly Column masterColumn;
        public readonly Column targetColumn;

        public ColumnAlterCandidate(Column masterColumn, Column targetColumn)
        {
            this.masterColumn = masterColumn;
            this.targetColumn = targetColumn;
        }
    }
}
