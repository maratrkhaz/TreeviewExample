using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeViewExample.Dto
{
    public class UnitsDto 
    {
        public int Id { get; set; }
        public int? Parent { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string UnitTypeCode { get; set; }

    }
}
