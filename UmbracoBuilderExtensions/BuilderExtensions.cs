using dplo.Data;
using dplo.Data.Infrastructure;
using Umbraco.Cms.Core.Notifications;
using dplo.Service;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.UmbracoContext;
using DatabaseFactory = NPoco.DatabaseFactory;

namespace DiamUmbracoClient.UmbracoBuilderExtensions;

public static class MyCustomBuilderExtensions
{
    // The first dependency is registered
    //public static IUmbracoBuilder RegisterCustomNotificationHandlers(this IUmbracoBuilder builder)
    //{
    //    //builder.AddNotificationHandler<ContentTypeSavedNotification, ContentTypeSavedHandler>();
    //    //{ ...}
    //    //return builder;
    //}

    // The second dependency is registered
    public static IUmbracoBuilder RegisterCustomServices(this IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<DatabaseFactory>();
        builder.Services.AddSingleton<UnitOfWork>();
        //builder.RegisterInstance(LogManager.GetLogger("SystemLog")).As<ILog>();
        //builder.Services.AddSingleton<UserSessionRepository>().As<IUserSessionRepository>();
        //builder.Services.AddSingleton<UserSessionService>().As<ISessionService>();
        builder.Services.AddSingleton<CategoryRepository>();
        builder.Services.AddSingleton<PartRepository>();
        builder.Services.AddSingleton<CountryRepository>();
        builder.Services.AddSingleton<RegionRepository>();
        builder.Services.AddSingleton<ProductRepository>();
        builder.Services.AddSingleton<HeroProductRepository>();
        builder.Services.AddSingleton<BrandRepository>();
        builder.Services.AddSingleton<StandRepository>();
        builder.Services.AddSingleton<StandColumnRepository>();
        builder.Services.AddSingleton<StandColumnUprightRepository>();
        builder.Services.AddSingleton<StandRowRepository>();
        builder.Services.AddSingleton<StandTypeRepository>();
        builder.Services.AddSingleton<PartTypeRepository>();
        builder.Services.AddSingleton<ShadeRepository>();
        builder.Services.AddSingleton<ClusterRepository>();
        builder.Services.AddSingleton<ClusterShelfRepository>();
        builder.Services.AddSingleton<ClusterPartRepository>();
        builder.Services.AddSingleton<PlanogramRepository>();
        builder.Services.AddSingleton<PlanogramNoteRepository>();
        builder.Services.AddSingleton<ScratchPadRepository>();
        builder.Services.AddSingleton<PlanogramPartRepository>();
        builder.Services.AddSingleton<PlanogramPartFacingRepository>();
        builder.Services.AddSingleton<PlanogramShelfRepository>();
        builder.Services.AddSingleton<PlanogramStatusRepository>();
        builder.Services.AddSingleton<PlanogramLockRepository>();
        builder.Services.AddSingleton<JobRepository>();
        builder.Services.AddSingleton<JobFolderRepository>();
        builder.Services.AddSingleton<PlanogramService>();
        builder.Services.AddSingleton<PlanogramSectionService>();
        builder.Services.AddSingleton<PlanogramSectionRepository>();

        builder.Services.AddSingleton<CategoryService>();
        builder.Services.AddSingleton<CatalogueService>();
        builder.Services.AddSingleton<CountryService>();
        builder.Services.AddSingleton<RegionService>();
        builder.Services.AddSingleton<ProductService>();
        builder.Services.AddSingleton<StandService>();
        builder.Services.AddSingleton<BrandService>();
        builder.Services.AddSingleton<JobService>();
        builder.Services.AddSingleton<JobFolderService>();

        builder.Services.AddSingleton<OrderService>();
        builder.Services.AddSingleton<OrderRepository>();

        builder.Services.AddSingleton<OrderItemRepository>();
        builder.Services.AddSingleton<OrderPlanogramRepository>();

        builder.Services.AddSingleton<OrderWindowService>();
        builder.Services.AddSingleton<OrderWindowRepository>();


        //builder.Register(c => UmbracoContext.Current).AsSelf();

        return builder;
    }

    // The two dependencies are bundled together
    public static IUmbracoBuilder AddCustomServices(this IUmbracoBuilder builder)
    {
        //builder.RegisterCustomNotificationHandlers();
        builder.RegisterCustomServices();
        return builder;
    }
}