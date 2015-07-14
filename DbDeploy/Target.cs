using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class Target
    {
        public readonly Int64 id;
        public string server;
        public string name;
        public string connectionString;

        public override string ToString()
        {
            return this.server + "." + this.name;
        }

        public Target(Int64 id, string server, string name, string connectionString)
        {
            this.id = id;
            this.server = server;
            this.name = name;
            this.connectionString = connectionString;
        }

    }
}
