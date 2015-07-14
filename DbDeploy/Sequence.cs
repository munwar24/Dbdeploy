using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class Sequence
    {
        public readonly string schema;
        public readonly string name;
        public readonly int dataTypeId;
        public readonly decimal startWith;
        public readonly decimal increment;
        public readonly decimal minValue;
        public readonly decimal maxValue;
        public readonly char cycle;
        public readonly int cache;
        public readonly char order;

        public Sequence(
            string schema, 
            string name, 
            int dataTypeId, 
            decimal startWith, 
            decimal increment, 
            decimal minValue, 
            decimal maxValue, 
            char cycle, 
            int cache, 
            char order
        )
        {
            this.schema = schema;
            this.name = name;
            this.dataTypeId = dataTypeId;
            this.startWith = startWith;
            this.increment = increment;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.cycle = cycle;
            this.cache = cache;
            this.order = order;
        }
    }

}
