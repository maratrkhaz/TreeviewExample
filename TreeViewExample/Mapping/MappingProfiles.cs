using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeViewExample.Dto;
using TreeViewExample.Models;

namespace TreeViewExample.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<OrgUnitBase, UnitsDto>()
                .ForMember(dest=>dest.Id, act=>act.MapFrom(src=>src.UnitId))
                .ForMember(dest=>dest.Parent, act => act.MapFrom(src => src.ParentUnitId))
                .ForMember(dest=>dest.Name, act=>act.MapFrom(src=>src.Name))
                .ForMember(dest=>dest.ParentName, act=>act.Ignore())
                .ForMember(dest=>dest.UnitTypeCode, act=>act.Ignore())
             ;

            CreateMap<CompanyEditViewModel, UnitsDto>().ReverseMap(); 

            CreateMap<OrgUnitBase, UnitTreeDto>().ReverseMap(); 

        }
    }
}
