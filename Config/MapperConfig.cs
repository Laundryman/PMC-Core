using AutoMapper;
using CoreSystem2024.Models;
using diam_planogram.Models.Shop;
using PMApplication.Dtos;
//using dplo.Domain;
//using dplo.Domain.Entities;
//using dplo.Service.MSGraphUtils;
//using Dplo.ViewModels;
using PMApplication.Entities;
using PMApplication.Entities.CountriesAggregate;
using PMApplication.Entities.PlanogramAggregate;
using Umbraco.Cms.Core.Models.Membership;

namespace CoreSystemII.Config;

public class MapperConfig
{
    public static AutoMapper.Mapper InitializeAutomapper()
    {
        ////Provide all the Mapping Configuration
        var config = new MapperConfiguration(cfg =>
        {
            //Configuring Employee and EmployeeDTO
            //cfg.AddProfile<UserViewModelProfile.MapBrandsToUserViewModelProfile>();
            //cfg.AddProfile<UserViewModelProfile>();
            cfg.CreateMap<Brand, BrandDto>();
            cfg.CreateMap<Country, CountryDto>();
            cfg.CreateMap<Category, CategoryModel>();
            cfg.CreateMap<Category, CategoryDto>();
            cfg.CreateMap<PlanogramNote, PNotesViewModel>();
            //Any Other Mapping Configuration ....
        });
        ////Create an Instance of Mapper and return that Instance
        var mapper = new AutoMapper.Mapper(config);
        return mapper;
    }
}

