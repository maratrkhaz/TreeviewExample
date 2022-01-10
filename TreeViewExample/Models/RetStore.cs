using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeViewExample.Persistence;

namespace TreeViewExample.Models
{
    public class RetStore : OrgUnitBase
    {
        private RetStore(string name) : base(name) { } //For EF Core

        private RetStore(string name, OrgUnitBase parent) : base(name, parent)
        {
        }

        public override string CodeInForm { get { return "ret"; } }

        public static RetStore AddUnitToDatabase(string name, OrgUnitBase parent, OrgUnitDbContext context)
        {
            var newUnit = new RetStore(name, parent);
            OrgUnitBase.AddUnitToDatabase(newUnit, context);
            return newUnit;
        }
    }
}
