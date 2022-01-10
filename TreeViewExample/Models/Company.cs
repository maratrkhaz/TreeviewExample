using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeViewExample.Persistence;

namespace TreeViewExample.Models
{
    public class Company : OrgUnitBase
    {
        private Company(string name) : base(name) { } //For EF Core

        public override string CodeInForm { get { return "cmp"; } }

        public static Company AddUnitToDatabase(string name, OrgUnitDbContext context)
        {
            var newUnit = new Company(name);
            OrgUnitBase.AddUnitToDatabase(newUnit, context);
            return newUnit;
        }
    }
}
