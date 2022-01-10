using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeViewExample.Persistence;

namespace TreeViewExample.Models
{
    public class SubUnit : OrgUnitBase
    {
        private SubUnit(string name) : base(name) { } //For EF Core

        private SubUnit(string name, OrgUnitBase parent) : base(name, parent)
        {
        }

        public override string CodeInForm { get { return "sub"; } }

        public static SubUnit AddUnitToDatabase(string name, OrgUnitBase parent, OrgUnitDbContext context)
        {
            var newUnit = new SubUnit(name, parent);
            OrgUnitBase.AddUnitToDatabase(newUnit, context);
            return newUnit;
        }
    }
}
