using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeViewExample.Dto;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TreeViewExample.Services
{
    public interface IUnitRepository
    {
        List<UnitTreeDto> GetAllUnits();
        List<UnitTreeDto> GetUnitTree();
        UnitsDto GetUnitById(int id);

        UnitsDto EditUnitName(int id, string newname);

        void MoveUnitToNewParent(int id, int parentid);
        void DeleteUnit(int id);

        void CreateUnit(UnitsDto dto);

        IEnumerable<SelectListItem> GetAllUnitTypes();
        IEnumerable<SelectListItem> GetUnitTypeByCode(string typeCode);
    }
}
