using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CoreSystem2024.Models;
using diam_planogram.Models.Shop;
using PMApplication.Dtos;
using PMApplication.Entities;
using PMApplication.Entities.CountriesAggregate;
using PMApplication.Entities.PlanogramAggregate;

namespace CoreSystem2024.AutoMapper
{
    public class DiamClientProfile : Profile
    {
        public DiamClientProfile()
        {
            CreateMap<PlanogramNote, PNotesViewModel>();
            CreateMap<Brand, BrandDto>();
            CreateMap<Country, CountryDto>();
            CreateMap<Category, CategoryModel>();
        }
    }
}
