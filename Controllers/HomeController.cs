using System.Configuration;
using System.Globalization;
using dplo.Service;
using CoreSystem.Models;
using CoreSystem.Helpers;
using Newtonsoft.Json;
using UserInfo = CoreSystem.Helpers.UserInfo;
using dplo.Service.MSGraphUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;


namespace diam_planogram.Controllers
{
    ////[MvcAuthorize]
    //[Authorize]
    public class HomeController : RenderController
    {
        #region helpers


        private readonly IOrderWindowService _orderWindowService;
        private readonly IPlanogramService _planogramService;
        private readonly ICountryService _countryService;
        public HomeController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IOrderWindowService orderWindowService, IPlanogramService planogramService, ICountryService countryService) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _orderWindowService = orderWindowService;
            _planogramService = planogramService;
            _countryService = countryService;
        }

        private static string RemoveQueryStringFromUri(string uri)
        {
            int index = uri.IndexOf('?');
            if (index > -1)
            {
                uri = uri.Substring(0, index);
            }
            return uri;
        }
        #endregion





        public async Task<IActionResult> Home(ContentModel model)
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
                //var proxySupport = new ProxyApiSupport();
                //var result = await proxySupport.AcquireTokenInteractive(new string[]
                //    { Globals.ReadTasksScope, Globals.WriteTasksScope });
                return new RedirectResult("/Welcome");
            }

            if (User.Identity.IsAuthenticated)
            {
                var userClaims = User.Claims;

                //we will create a custom model
                var homeModel = new HomeModel(model.Content);
                if (UserInfo.userViewModel != null)
                {
                    homeModel.UserFirstName = UserInfo.GivenName;
                    homeModel.UserLastName = UserInfo.Surname;
                }

                var brandId = ConfigurationManager.AppSettings["brand"];

                homeModel.BrandId = int.Parse(brandId);
                homeModel.ApiUrl = ConfigurationManager.AppSettings["apiURL"];
                homeModel.CountryId = UserInfo.DiamCountryId;
                var country = _countryService.GetCountry(UserInfo.DiamCountryId);
                UserInfo.userViewModel.DiamCountryName = country.Name;
                homeModel.CountryName = country.Name;
                homeModel.CountryFlag = country.FlagFileName;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");


                var orderWindow = _orderWindowService.GetCurrentOrderWindow(int.Parse(brandId), false)
                                  ?? _orderWindowService.GetNextOrderWindow(int.Parse(brandId))
                                  ?? _orderWindowService.GetCurrentOrderWindow(int.Parse(brandId), true);

                if (orderWindow != null)
                {
                    homeModel.OrderWindowOpening = orderWindow.StartDate.AsUtc().ToString("O");
                    homeModel.OrderWindowClosing = orderWindow.EndDate.AsUtc().ToString("O");
                }

                var orderWindowCalendar = _orderWindowService.GetOrderWindowCalendar(int.Parse(brandId));

                if (orderWindowCalendar != null && orderWindowCalendar.Any())
                {
                    homeModel.OrderWindowCalendar = JsonConvert.SerializeObject(orderWindowCalendar);
                }

                return CurrentTemplate(homeModel);
            }
            else             {
                Response.Redirect("/Welcome");
                return null;
            }
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