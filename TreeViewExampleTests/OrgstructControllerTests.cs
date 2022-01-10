using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeViewExample.Mapping;
using TreeViewExample.Models;
using TreeViewExample.Persistence;
using Xunit;
using Moq;
using TreeViewExample.Services;
using TreeViewExample.Dto;
using TreeViewExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TreeViewExampleTests
{
    public class OrgstructControllerTests
    {
        private OrgUnitDbContext _context;
        private readonly IMapper _mapper;

        public OrgstructControllerTests()
        {
            _context = GetDbContext();
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfiles());
            });
            _mapper = mockMapper.CreateMapper();
        }

        public OrgUnitDbContext GetDbContext()
        {
            var builder = new DbContextOptionsBuilder<OrgUnitDbContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            var dbContext = new OrgUnitDbContext(builder.Options);

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            return dbContext;
        }

        [Fact]
        public void Nodes_Contains_All_Units()
        {
            // Arrange - create the mock repository
            Mock<IUnitRepository> mock = new Mock<IUnitRepository>();

            mock.Setup(m => m.GetUnitTree()).Returns(new List<UnitTreeDto>
            {
                new UnitTreeDto("1","cmp","#","Company"),
                new UnitTreeDto("2","sub","1","SubUnit"),
                new UnitTreeDto("3","ret","2","Store"),
            });

            OrgstructController controller = new OrgstructController(mock.Object, _mapper);

            // Action
            var result = GetJsonValue<IEnumerable<UnitTreeDto>>(controller.Nodes());

            // Assert
            Assert.Equal(3, result.Count());

            // Assert
            var companyName = result.Where(t => t.Id == "1").Select(t => t.Text).FirstOrDefault();
            var subUnitName = result.Where(t => t.Id == "2").Select(t => t.Text).FirstOrDefault();
            var storeName = result.Where(t => t.Id == "3").Select(t => t.Text).FirstOrDefault();
            Assert.Equal("Company", companyName);
            Assert.Equal("SubUnit", subUnitName);
            Assert.Equal("Store", storeName);
        }

        [Fact]
        public void Node_Can_Create_Unit()
        {
            // Arrange - create the mock repository
            Mock<IUnitRepository> mock = new Mock<IUnitRepository>();

            mock.Setup(m => m.GetUnitTree()).Returns(new List<UnitTreeDto>
            {
                new UnitTreeDto("1","cmp","#","Company"),
                //new UnitTreeDto("2","sub","1","SubUnit"),
                //new UnitTreeDto("3","ret","2","Store"),
            });

            OrgstructController controller = new OrgstructController(mock.Object, _mapper);

            CompanyEditViewModel model = new CompanyEditViewModel
            {
                Parent = 1,
                Name = "SubUnit",
                UnitTypeCode = "sub"
            };

            // Action
            IActionResult result = controller.Create(model);

            // Assert - check the result type is a redirection
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", (result as ViewResult).ViewName);

        }

        [Theory]
        [InlineData(null, "sub", "Name")]
        [InlineData("SubUnit", null, "UnitTypeCode")]
        public void Node_Can_Not_Create_WithEmptyRequired(string name, string unittypecode, string expected)
        {
            // Arrange - create the mock repository
            Mock<IUnitRepository> mock = new Mock<IUnitRepository>();

            mock.Setup(m => m.GetUnitTree()).Returns(new List<UnitTreeDto>
            {
                new UnitTreeDto("1","cmp","#","Company"),
            });

            OrgstructController controller = new OrgstructController(mock.Object, _mapper);

            CompanyEditViewModel model = new CompanyEditViewModel
            {
                Name = name,
                Parent = 1,
                UnitTypeCode = unittypecode
            };

            // Action
            var errors = ValidateModel(model);
            
            Assert.Contains(errors,
                v => v.MemberNames.FirstOrDefault().ToLower().Contains(expected.ToLower()) &&
                v.ErrorMessage.Contains("required"));
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }

        private T GetJsonValue<T>(IActionResult result) where T : class
        {
            return (result as JsonResult)?.Value as T;
        }


    }
}
