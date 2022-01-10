using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TreeViewExample.Dto;
using TreeViewExample.Models;
using TreeViewExample.Services;

namespace TreeViewExample.Controllers
{
    public class OrgstructController : Controller
    {
        private readonly IMapper _mapper;
        private IUnitRepository _repo;

        //some fix
        public OrgstructController(IUnitRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Nodes()
        {
            return Json(_repo.GetUnitTree());
        }

        public IActionResult Create(int id)
        {
            var parentCompany = _repo.GetUnitById(id);
            var viewModel = new CompanyEditViewModel();

            if (parentCompany != null)
            {
                viewModel.Parent = parentCompany.Id;
                viewModel.ParentName = parentCompany.Name;
                viewModel.UnitTypes = _repo.GetAllUnitTypes();
            }
            else
            {
                viewModel.Parent = 0;
                viewModel.ParentName = "";
                viewModel.UnitTypes = _repo.GetUnitTypeByCode("cmp");
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(CompanyEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<UnitsDto>(model);
                _repo.CreateUnit(dto);
                return View("Index");
            }
            else
            {
                if (model.Parent == 0)
                {
                    model.UnitTypes = _repo.GetUnitTypeByCode("cmp");
                }
                else
                {
                    model.UnitTypes = _repo.GetAllUnitTypes();
                }
                                    
                return View(model);
            }
        }
        
        [HttpDelete]
        public IActionResult DeleteNode( int id)
        {
           _repo.DeleteUnit(id);
            return View("Index");
        }

        [HttpPost]
        public IActionResult Move(int curid, int newparentid)
        {
            _repo.MoveUnitToNewParent(curid, newparentid);
            return View("Index");
        }

        public IActionResult Edit(int id)
        {
            var unit = _repo.GetUnitById(id);
            var model = _mapper.Map<CompanyEditViewModel>(unit);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CompanyEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<UnitsDto>(model);
                var companyEditedDto = _repo.EditUnitName(dto.Id, dto.Name);
                return View("Index");
            }
            else
            {
                return View(model);
            }
        }
        
    }
}
