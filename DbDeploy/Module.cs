using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class Module
    {
        public readonly string schema;
        public readonly string name;
        public readonly int moduleId;
        public readonly char ownertype;
        public readonly char moduletype;
       // public readonly decimal startWith;
       // public readonly decimal increment;
      //  public readonly decimal minValue;
       // public readonly decimal maxValue;
      //  public readonly char cycle;
      ///  public readonly int cache;
       // public readonly char order;

        public Module(
            string schema,
            string name,
            int moduleId,
           // decimal startWith,
          //  decimal increment,
         //   decimal minValue,
          //  decimal maxValue,
         //   char cycle,
          //  int cache,
         //   char order
            char ownertype,
                char moduletype
        )
        {
            this.schema = schema;
            this.name = name;
            this.moduleId = moduleId;
            this.ownertype = ownertype;
            this.moduletype = moduletype;
          //  this.startWith = startWith;
          //  this.increment = increment;
         //   this.minValue = minValue;
          //  this.maxValue = maxValue;
         //   this.cycle = cycle;
          //  this.cache = cache;
         //   this.order = order;
        }
    }

}
