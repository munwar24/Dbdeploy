using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class MQTIndex
    {
        public readonly MQT mqt;
        public readonly string name;
        public readonly bool nonUnique;
        public readonly short type;
        public readonly SortedList<string, MQTIndexColumn> columns = new SortedList<string, MQTIndexColumn>();

        public MQTIndex(MQT mqt, string name, bool nonUnique, short type)
        {
            this.mqt = mqt;
            this.name = name;
            this.nonUnique = nonUnique;
            this.type = type;
        }
    }
}
