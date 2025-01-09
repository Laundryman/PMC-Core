using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CoreSystem2024.Models;
using dplo.Domain.Entities;
using dplo.Domain;
using Dplo.ViewModels;

namespace CoreSystem2024.AutoMapper
{
    public class DiamClientProfile : Profile
    {
        public DiamClientProfile()
        {
            CreateMap<PlanogramNote, PNotesViewModel>();
            CreateMap<Brand, BrandViewModel>();
            CreateMap<Country, CountryViewModel>();
            CreateMap<Category, CategoryModel>();
        }
    }
}
