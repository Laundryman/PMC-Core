using CoreSystem2024.ProxyServices;
using dplo.Data;
using dplo.Data.Infrastructure;
using dplo.Service;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSystemII.DI
{
    public static class diExtensions
    {
        //public static IServiceCollection AddConfig(
        //    this IServiceCollection services, IConfiguration config)
        //{
        //    services.Configure<PositionOptions>(config.GetSection(PositionOptions.Position));
        //    services.Configure<ColorOptions>(config.GetSection(ColorOptions.Color));

        //    return services;
        //}

        public static IServiceCollection AddDploServiceCollection(
            this IServiceCollection services)
        {
            services.AddScoped<IDatabaseFactory, DatabaseFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICatalogueService, CatalogueService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJobFolderService, JobFolderService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<ILMAuditService, LMAuditService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderWindowService, OrderWindowService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IPlanogramService, PlanogramService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IStandService, StandService>();
            services.AddScoped<IYourPlanogramProxyApiService, YourPlanogramProxyApiService>();

            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IClusterRepository, ClusterRepository>();
            services.AddScoped<IClusterPartRepository, ClusterPartRepository>();
            services.AddScoped<IClusterShelfRepository, ClusterShelfRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IHeroProductRepository, HeroProductRepository>();
            services.AddScoped<IJobFolderRepository, JobFolderRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<ILMAuditRepository, LMAuditRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderPlanogramRepository, OrderPlanogramRepository>();
            services.AddScoped<IOrderWindowRepository, OrderWindowRepository>();
            services.AddScoped<IPageRepository, PageRepository>();
            services.AddScoped<IPartRepository, PartRepository>();
            services.AddScoped<IPartTypeRepository, PartTypeRepository>();
            services.AddScoped<IPlanogramLockRepository, PlanogramLockRepository>();
            services.AddScoped<IPlanogramNoteRepository, PlanogramNoteRepository>();
            services.AddScoped<IPlanogramPartRepository, PlanogramPartRepository>();
            services.AddScoped<IPlanogramPartFacingRepository, PlanogramPartFacingRepository>();
            services.AddScoped<IPlanogramRepository, PlanogramRepository>();
            services.AddScoped<IPlanogramShelfRepository, PlanogramShelfRepository>();
            services.AddScoped<IPlanogramStatusRepository, PlanogramStatusRepository>();
            services.AddScoped<IPlanogramVersionRepository, PlanogramVersionRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IShadeRepository, ShadeRepository>();
            services.AddScoped<IScratchPadRepository, ScratchPadRepository>();
            services.AddScoped<IStandColumnRepository, StandColumnRepository>();
            services.AddScoped<IStandColumnUprightRepository, StandColumnUprightRepository>();
            services.AddScoped<IStandRepository, StandRepository>();
            services.AddScoped<IStandRowRepository, StandRowRepository>();
            services.AddScoped<IStandTypeRepository, StandTypeRepository>();
            return services;
        }
    }
}