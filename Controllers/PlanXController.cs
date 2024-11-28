using System.Configuration;
using System.Globalization;
using dplo.Service;
using CoreSystem.Models;
using CoreSystem.Helpers;
using Newtonsoft.Json;
using UserInfo = CoreSystem.Helpers.UserInfo;
using dplo.Service.MSGraphUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;


namespace diam_planogram.Controllers
{
    [Authorize]
    public class PlanXController : RenderController
    {


        #region Services, managers

        private IOrderWindowService _orderWindowService;

        #endregion

        public PlanXController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IOrderWindowService orderWindowService) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _orderWindowService = orderWindowService;
        }

        public async Task<IActionResult> EditPlanX(ContentModel model)
        {

            try
            {
                var proxySupport = new ProxyApiSupport();
                //// Retrieve the token with the specified scopes
                var result = await proxySupport.AcquireTokenForScopes(new string[]
                    { Globals.ReadTasksScope, Globals.WriteTasksScope });
            }
            catch (MsalUiRequiredException)
            {
                /*
                    If the tokens have expired or become invalid for any reason, ask the user to sign in again.
                    Another cause of this exception is when you restart the app using InMemory cache.
                    It will get wiped out while the user will be authenticated still because of their cookies, requiring the TokenCache to be initialized again
                    through the sign in flow.
                */
                return new RedirectResult("/Welcome");
            }

            //we will create a custom model
            var planXModel = new PlanXModel(model.Content);
            if (UserInfo.userViewModel != null)
            {
                planXModel.UserFirstName = UserInfo.GivenName;
                planXModel.UserLastName = UserInfo.Surname;
            }
            var brandId = ConfigurationManager.AppSettings["brand"];

            planXModel.BrandId = int.Parse(brandId);
            planXModel.ApiUrl = ConfigurationManager.AppSettings["apiURL"];

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");


            var orderWindow = _orderWindowService.GetCurrentOrderWindow(int.Parse(brandId), false)
                ?? _orderWindowService.GetNextOrderWindow(int.Parse(brandId))
                ?? _orderWindowService.GetCurrentOrderWindow(int.Parse(brandId), true);

            if (orderWindow != null)
            {
                planXModel.OrderWindowOpening = orderWindow.StartDate.AsUtc().ToString("O");
                planXModel.OrderWindowClosing = orderWindow.EndDate.AsUtc().ToString("O");
            }

            var orderWindowCalendar = _orderWindowService.GetOrderWindowCalendar(int.Parse(brandId));

            if (orderWindowCalendar != null && orderWindowCalendar.Any())
            {
                planXModel.OrderWindowCalendar = JsonConvert.SerializeObject(orderWindowCalendar);
            }

            return CurrentTemplate(planXModel);
        }

        public IActionResult MihLandingPage(ContentModel model)
        {

            //if (Authorization == null || Authorization.AccessTokenExpirationUtc < DateTime.UtcNow)
            //{
            //    //var accessToken = //AuthHelper.ReAuth(Authorization, client);
            //    if (accessToken == null)
            //    {
            //        Response.Redirect("/welcome");
            //        return null;
            //    }

            //}

            //we will create a custom model
            var HomeModel = new HomeModel(model.Content);
            if (UserInfo.userViewModel != null)
            {
                HomeModel.UserFirstName = UserInfo.GivenName;
                HomeModel.UserLastName = UserInfo.Surname;
            }
            HomeModel.BrandId = int.Parse(ConfigurationManager.AppSettings["brand"]);
            HomeModel.ApiUrl = ConfigurationManager.AppSettings["apiURL"];

            //simply use the protected method CurrentTemplate<T>, this does all of the
            //above for you... must nicer.
            return CurrentTemplate(HomeModel);
        }

    }
}