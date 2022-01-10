using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeViewExample.Dto;
using TreeViewExample.Models;
using TreeViewExample.Persistence;

namespace TreeViewExample.Services
{
    public class UnitRepository : IUnitRepository
    {
        private readonly OrgUnitDbContext _context;
        private Dictionary<string, Func<string, OrgUnitBase, OrgUnitBase>> _addUnits;
        private readonly IMapper _mapper;

        public UnitRepository(OrgUnitDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<UnitTreeDto> GetAllUnits()
        {
            var nodes = new List<UnitTreeDto>();

            foreach (var item in _context.OrgUnits.IgnoreQueryFilters())
            {
                var parent_checked = item.ParentUnitId is null ? "#" : item.ParentUnitId.ToString();

                var nodetype = "";

                nodes.Add(new UnitTreeDto(item.UnitId.ToString(), nodetype, parent_checked, item.Name.ToString()));
            }

            return nodes;
        }

        public IEnumerable<SelectListItem> GetAllUnitTypes()
        {
            List<SelectListItem> companytypes = _context.OrgUnitTypes.IgnoreQueryFilters().AsNoTracking()
                   .OrderBy(n => n.Name)
                   .Select(n =>
                       new SelectListItem
                       {
                           Value = n.Code.ToString(),
                           Text = n.Name.ToString()
                       }).ToList();
            var tip = new SelectListItem()
            {
                Value = null,
                Text = "--- Choose type ---"
            };
            companytypes.Insert(0, tip);
            return new SelectList(companytypes, "Value", "Text");
        }

        public IEnumerable<SelectListItem> GetUnitTypeByCode(string typeCode)
        {
            List<SelectListItem> companytypes = _context.OrgUnitTypes.IgnoreQueryFilters().AsNoTracking()
                   .Where(c=>c.Code== typeCode)
                   .OrderBy(n => n.Name)
                   .Select(n =>
                       new SelectListItem
                       {
                           Value = n.Code.ToString(),
                           Text = n.Name.ToString()
                       }).ToList();
            var tip = new SelectListItem()
            {
                Value = null,
                Text = "--- Choose type ---"
            };
            companytypes.Insert(0, tip);
            return new SelectList(companytypes, "Value", "Text");
        }

        public List<UnitTreeDto> GetUnitTree()
        {
            var nodes = new List<UnitTreeDto>();

            var cnt = _context.OrgUnits.Count();

            foreach (var item in _context.OrgUnits)
            {
                var parent_name = item.ParentUnitId is null ? "#" : item.ParentUnitId.ToString();
                if (cnt == 1)
                    parent_name = "#";

                var nodetype = "";

                nodes.Add(new UnitTreeDto(item.UnitId.ToString(), nodetype, parent_name, item.Name.ToString()));
            }

            return nodes;
        }

        public UnitsDto GetUnitById(int id)
        {
           var company = _context.OrgUnits.SingleOrDefault(p => p.UnitId == id);
           var companyDto = _mapper.Map<UnitsDto>(company);
           return companyDto;
        }

        private Company AddCompany(string name, OrgUnitBase parent)
        {
            Company unit = Company.AddUnitToDatabase(name, _context);
            return unit;
        }

        private SubUnit AddSubUnit(string name, OrgUnitBase parent)
        {
            SubUnit unit = SubUnit.AddUnitToDatabase(name, parent, _context);
            return unit;
        }

        private RetStore AddRetailStore(string name, OrgUnitBase parent)
        {
            RetStore unit = RetStore.AddUnitToDatabase(name, parent, _context);
            return unit;
        }

        private OrgUnitBase PerformAddUnit(string op, string name, OrgUnitBase parent)
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            _addUnits = new Dictionary<string, Func<string, OrgUnitBase, OrgUnitBase>>
            {
                {"cmp", this.AddCompany },
                {"sub", this.AddSubUnit  },
                { "ret", this.AddRetailStore }
            };

            if (!_addUnits.ContainsKey(op))
                throw new ArgumentException(string.Format($"Operation {op} is invalid"), nameof(op));

            return _addUnits[op](name, parent);
        }

        public void CreateUnit(UnitsDto dto)
        {
            var parent = _context.OrgUnits.SingleOrDefault(p => p.UnitId == dto.Parent);
            PerformAddUnit(dto.UnitTypeCode, dto.Name, parent);
        }

        public void MoveUnitToNewParent(int id, int parentid)
        {

            var org = _context.Find<OrgUnitBase>(id);
            var oldparent = _context.Find<OrgUnitBase>(org.UnitId);

            if (org != null)
            {
                org.MoveUnitToNewParent(parentid, _context);
            }
        }

        public UnitsDto EditUnitName(int id, string newname)
        {
            var company = _context.Find<OrgUnitBase>(id);

            if (company != null)
            {
                company.ChangeName(newname);
                _context.SaveChanges();
            }

            var companyDto = _mapper.Map<UnitsDto>(company);
            return companyDto;
        }

        public void DeleteUnit(int id)
        {
            var org = _context.Find<OrgUnitBase>(id);

            if (org != null)
            {
                org.DeleteUnit(_context);
            }
        }
    }
}
