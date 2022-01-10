using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TreeViewExample.Models
{
    public class CompanyEditViewModel
    {
        public int Id { get; set; }
        public int? Parent { get; set; }
        [Required]
        public string Name { get; set; }
        public string ParentName { get; set; }
        [Required]
        [Display(Name = "Unit Type Code")]
        public string UnitTypeCode { get; set; }
        public IEnumerable<SelectListItem> UnitTypes { get; set; }

    }
}
