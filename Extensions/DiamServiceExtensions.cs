using AutoMapper;
using CoreSystem2024.AutoMapper;
using CoreSystem2024.Helpers;
using CoreSystem2024.ProxyServices;
using dplo.Data;
using dplo.Data.Infrastructure;
using dplo.Service;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace CoreSystem2024.Extensions
{
    public static class DiamServiceExtensions
    {
        public static IUmbracoBuilder AddDiamServices(this IUmbracoBuilder builder)
        {
            builder.Services.AddAutoMapper(typeof(DiamClientProfile));
            builder.Services.AddTransient<ICategoryService, CategoryService>();
            builder.Services.AddTransient<ICatalogueService, CatalogueService>();
            builder.Services.AddTransient<ICountryService, CountryService>();
            builder.Services.AddTransient<IBrandService, BrandService>();
            builder.Services.AddTransient<IJobService, JobService>();
            builder.Services.AddTransient<IJobFolderService, JobFolderService>();
            builder.Services.AddTransient<IOrderService, OrderService>();
            builder.Services.AddTransient<IOrderWindowService, OrderWindowService>();
            builder.Services.AddTransient<IPlanogramService, PlanogramService>();
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<IRegionService, RegionService>();
            builder.Services.AddTransient<IStandService, StandService>();
            builder.Services.AddScoped<IMemberManager, DiamMemberManager>();
            builder.Services.AddScoped<IYourPlanogramProxyApiService, YourPlanogramProxyApiService>();
            builder.Services.AddScoped<IPlanxProxyApiService, PlanxProxyApiService>();
            builder.Services.AddScoped<ICreatePlanogramProxyService, CreatePlanogramProxyApiService>();
            builder.Services.AddScoped<EmailHelper>();
            return builder;
        }

        public static IUmbracoBuilder AddRepositories(this IUmbracoBuilder builder)
        {
            builder.Services.AddScoped<IDatabaseFactory, DatabaseFactory>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IBrandRepository, BrandRepository>();
            builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
            builder.Services.AddTransient<IClusterRepository, ClusterRepository>();
            builder.Services.AddTransient<IClusterShelfRepository, ClusterShelfRepository>();
            builder.Services.AddTransient<IClusterPartRepository, ClusterPartRepository>();
            builder.Services.AddTransient<ICountryRepository, CountryRepository>();
            builder.Services.AddTransient<IEmailRepository, EmailRepository>();
            builder.Services.AddTransient<IHeroProductRepository, HeroProductRepository>();
            builder.Services.AddTransient<IJobRepository, JobRepository>();
            builder.Services.AddTransient<IJobFolderRepository, JobFolderRepository>();
            builder.Services.AddTransient<IOrderPlanogramRepository, OrderPlanogramRepository>();
            builder.Services.AddTransient<IOrderRepository, OrderRepository>();
            builder.Services.AddTransient<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddTransient<IOrderWindowRepository, OrderWindowRepository>();
            builder.Services.AddTransient<IPartRepository, PartRepository>();
            builder.Services.AddTransient<IPartTypeRepository, PartTypeRepository>();
            builder.Services.AddTransient<IPlanogramRepository, PlanogramRepository>();
            builder.Services.AddTransient<IPlanogramLockRepository, PlanogramLockRepository>();
            builder.Services.AddTransient<IPlanogramNoteRepository, PlanogramNoteRepository>();
            builder.Services.AddTransient<IPlanogramPartRepository, PlanogramPartRepository>();
            builder.Services.AddTransient<IPlanogramPartFacingRepository, PlanogramPartFacingRepository>();
            builder.Services.AddTransient<IPlanogramShelfRepository, PlanogramShelfRepository>();
            builder.Services.AddTransient<IPlanogramStatusRepository, PlanogramStatusRepository>();
            builder.Services.AddTransient<IProductRepository, ProductRepository>();
            builder.Services.AddTransient<IRegionRepository, RegionRepository>();
            builder.Services.AddTransient<IStandRepository, StandRepository>();
            builder.Services.AddTransient<IStandColumnRepository, StandColumnRepository>();
            builder.Services.AddTransient<IStandColumnUprightRepository, StandColumnUprightRepository>();
            builder.Services.AddTransient<IStandRowRepository, StandRowRepository>();
            builder.Services.AddTransient<IStandTypeRepository, StandTypeRepository>();
            builder.Services.AddTransient<IShadeRepository, ShadeRepository>();
            builder.Services.AddTransient<IScratchPadRepository, ScratchPadRepository>();
            builder.Services.AddTransient<IStandService, StandService>();

            return builder;
        }

    }
}
