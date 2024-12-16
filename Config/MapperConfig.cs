using AutoMapper;
using dplo.Domain;
using dplo.Domain.Entities;
using Dplo.ViewModels;

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
            cfg.CreateMap<Brand, BrandViewModel>();
            cfg.CreateMap<Country, CountryViewModel>();
            cfg.CreateMap<Category, CategoryModel>();
            //Any Other Mapping Configuration ....
        });
        ////Create an Instance of Mapper and return that Instance
        var mapper = new AutoMapper.Mapper(config);
        return mapper;
    }
}

