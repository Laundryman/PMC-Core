using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using dplo.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using CoreSystem2024.CMSModelBuilderModels;
using CoreSystem2024.Controllers.shop;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;


namespace diam_planogram.Controllers
{
    ////[MvcAuthorize]
    //[Authorize]
    public class HomeController : BaseMvcController
    {
        #region helpers


        private readonly IOrderWindowService _orderWindowService;
        private readonly IPlanogramService _planogramService;
        private readonly ICountryService _countryService;
        private IConfiguration _azureSettings;
        private readonly IConfiguration _config;
        private readonly IMemberManager _memberManager;

        public HomeController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IOrderWindowService orderWindowService, IPlanogramService planogramService, ICountryService countryService, IConfiguration azureSettings, IMemberManager memberManager) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _orderWindowService = orderWindowService;
            _planogramService = planogramService;
            _countryService = countryService;
            _azureSettings = azureSettings.GetSection("AzureB2C");
            _config = azureSettings;
            _memberManager = memberManager;
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





        public async Task<IActionResult> Home(Home model)
        {
            //try
            //{
            //    var proxySupport = new ProxyApiSupport(Configuration);
            //    //// Retrieve the token with the specified scopes
            //    var result = await proxySupport.AcquireTokenForScopes(new string[]
            //        { _azureSettings["ReadScope"], _azureSettings["WriteScope"]});
            //}
            //catch (MsalUiRequiredException)
            //{
            //    //var proxySupport = new ProxyApiSupport();
            //    //var result = await proxySupport.AcquireTokenInteractive(new string[]
            //    //    { Globals.ReadTasksScope, Globals.WriteTasksScope });
            //    return new RedirectResult("/Welcome");
            //}
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var logins = _memberManager.GetLoginsAsync(memberIdentity);
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            if (User.Identity.IsAuthenticated)
            {
                var userClaims = User.Claims;

                //we will create a custom model

                if (userInfo.GivenName != null)
                {
                    model.UserFirstName = userInfo.GivenName;
                    model.UserLastName = userInfo.Surname;
                }

                var brandId = _config["AppSettings:ClientBrandId"];

                model.BrandId = int.Parse(brandId);
                model.ApiUrl = _config["AppSettings:ApiURL"];
                model.ShopUrl = _config["AppSettings:ShopURL"];
                model.CountryId = userInfo.DiamCountryId;
                var country = _countryService.GetCountry(userInfo.DiamCountryId);
                userInfo.DiamCountryName = country.Name;
                model.CountryName = country.Name;
                model.CountryFlag = country.FlagFileName;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");


                var orderWindow = _orderWindowService.GetCurrentOrderWindow(int.Parse(brandId), false)
                                  ?? _orderWindowService.GetNextOrderWindow(int.Parse(brandId))
                                  ?? _orderWindowService.GetCurrentOrderWindow(int.Parse(brandId), true);

                if (orderWindow != null)
                {
                    model.OrderWindowOpening = orderWindow.StartDate;
                    model.OrderWindowClosing = orderWindow.EndDate;
                }

                var orderWindowCalendar = _orderWindowService.GetOrderWindowCalendar(int.Parse(brandId));

                if (orderWindowCalendar != null && orderWindowCalendar.Any())
                {
                    model.OrderWindowCalendar = JsonConvert.SerializeObject(orderWindowCalendar);
                }

                return CurrentTemplate(model);
            }
            else
            {
                Response.Redirect("/Welcome");
                return null;
            }
        }

        public async Task<IActionResult> MihLandingPage(ContentModel model)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);



            ////we will create a custom model
            //var model = new model(model.Content);
            //if (userInfo != null)
            //{
            //    model.UserFirstName = userInfo.GivenName;
            //    model.UserLastName = userInfo.Surname;
            //}
            //model.BrandId = int.Parse(Configuration["AppSettings:ClientBrandId"]);
            //model.ApiUrl = Configuration["AppSettings:ApiURL"];

            //simply use the protected method CurrentTemplate<T>, this does all of the
            //above for you... must nicer.
            return CurrentTemplate(model);
        }

    }
}