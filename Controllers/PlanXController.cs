using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using CoreSystem2024.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Globalization;
using CoreSystem2024.Controllers.shop;
using PMApplication.Interfaces.ServiceInterfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using UserInfo = CoreSystem2024.Helpers.UserInfo;
using PMApplication.Specifications.Filters;


namespace diam_planogram.Controllers
{
    [Authorize]
    public class PlanXController : BaseMvcController
    {


        #region Services, managers

        private IOrderWindowService _orderWindowService;
        private readonly IConfiguration Configuration;
        private IConfiguration _azureSettings;
        #endregion

        public PlanXController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IOrderWindowService orderWindowService, IConfiguration azureSettings, IConfiguration configuration) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _orderWindowService = orderWindowService;
            Configuration = configuration;
            _azureSettings = configuration.GetSection("AzureB2C");
        }


        public async Task<IActionResult> EditPlanX(ContentModel model)
        {

            //we will create a custom model
            var planXModel = new PlanXModel(model.Content);
            if (UserInfo.userViewModel != null)
            {
                planXModel.UserFirstName = UserInfo.GivenName;
                planXModel.UserLastName = UserInfo.Surname;
            }
            var brandId = Configuration["AppSettings:ClientBrandId"];

            planXModel.BrandId = int.Parse(brandId);
            planXModel.ApiUrl = Configuration["AppSettings:ApiURL"];

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            var owFilter = new OrderWindowFilter
            {
                BrandId = planXModel.BrandId,
                IncludeExpired = false
            };
            var orderWindows = await _orderWindowService.GetOrderWindows(owFilter);
                if (orderWindows.Count == 0)
                    owFilter.IncludeExpired = true;
            orderWindows = await _orderWindowService.GetOrderWindows(owFilter);
            var orderWindow = orderWindows.FirstOrDefault();

            if (orderWindow != null)
            {
                planXModel.OrderWindowOpening = orderWindow.StartDate.AsUtc().ToString("O");
                planXModel.OrderWindowClosing = orderWindow.EndDate.AsUtc().ToString("O");
            }
            //get the calender - change the filter
            owFilter.GetFutureWindows = true;
            owFilter.IncludeExpired = false;
            var orderWindowCalendar = await _orderWindowService.GetOrderWindows(owFilter);

            if (orderWindowCalendar.Any())
            {
                planXModel.OrderWindowCalendar = JsonConvert.SerializeObject(orderWindowCalendar);
            }

            return CurrentTemplate(planXModel);
        }

        //public IActionResult MihLandingPage(ContentModel model)
        //{

        //    //if (Authorization == null || Authorization.AccessTokenExpirationUtc < DateTime.UtcNow)
        //    //{
        //    //    //var accessToken = //AuthHelper.ReAuth(Authorization, client);
        //    //    if (accessToken == null)
        //    //    {
        //    //        Response.Redirect("/welcome");
        //    //        return null;
        //    //    }

        //    //}

        //    //we will create a custom model
        //    //var HomeModel = new HomeModel(model.Content);
        //    //if (UserInfo.userViewModel != null)
        //    //{
        //    //    HomeModel.UserFirstName = UserInfo.GivenName;
        //    //    HomeModel.UserLastName = UserInfo.Surname;
        //    //}

        //    //HomeModel.BrandId = int.Parse(Configuration["AppSettings:ClientBrandId"]);
        //    //HomeModel.ApiUrl = Configuration["AppSettings:ApiURL"];


        //    //simply use the protected method CurrentTemplate<T>, this does all of the
        //    //above for you... must nicer.
        //    return CurrentTemplate(HomeModel);
        //}

    }
}