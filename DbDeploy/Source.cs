using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class Source
    {
        public string server;
        public string name;
        public string connectionString;

        public override string ToString()
        {
            return this.server + "." + this.name;
        }

        public Source(string server, string name, string connectionString)
        {
            this.server = server;
            this.name = name;
            this.connectionString = connectionString;
        }
    }
}
