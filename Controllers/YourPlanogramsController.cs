using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using dplo.Domain;
using dplo.Service;
using dplo.Service.MSGraphUtils;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;

namespace diam_planogram.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class YourPlanogramsController : RenderController
    {


        #region Services, managers

        public IStandService _standService;
        public ICountryService _countryService;
        public IRegionService _regionService;
        #endregion
        private readonly IConfiguration Configuration;
        private string domain;
        private string brandId;
        private string readScope;
        private string writeScope;
        // GET: CreateStand
        public YourPlanogramsController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IStandService standService, ICountryService countryService, IRegionService regionService, IConfiguration configuration) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _standService = standService;
            _countryService = countryService;
            _regionService = regionService;
            Configuration = configuration;
            domain = Configuration["AppSettings:ApiUrl"];
            brandId = Configuration["AppSettings:ClientBrandId"];
            readScope = Configuration["AzureB2C:ReadScope"];
            writeScope = Configuration["AzureB2CWriteScope"];
        }

        public async Task<IActionResult> YourPlanograms(ContentModel model)
        {

            try
            {
                var proxySupport = new ProxyApiSupport(Configuration);
                //// Retrieve the token with the specified scopes
                var result = await proxySupport.AcquireTokenForScopes(new string[]
                    { readScope, writeScope });
            }
            catch (MsalUiRequiredException)
            {
                /*
                    If the tokens have expired or become invalid for any reason, ask the user to sign in again.
                    Another cause of this exception is when you restart the app using InMemory cache.
                    It will get wiped out while the user will be authenticated still because of their cookies, requiring the TokenCache to be initialized again
                    through the sign in flow.
                */
                return new RedirectResult("/AzureAccount/SignUpSignIn?redirectUrl=/");
            }
            var userCountry = _countryService.GetCountry(UserInfo.DiamCountryId);

            var systemRole = RolesHelper.GetUserRole(UserInfo.Roles);
            //we will create a custom model
            var yourPlanogramsModel = new YourPlanogramsModel
            {
                UserFirstName = UserInfo.GivenName,
                UserLastName = UserInfo.Surname,
                BrandId = int.Parse(brandId),
                ApiUrl = domain,
                UserRoles = UserInfo.Roles,
                SystemRole = (int)systemRole

            };


            //GET THE FILTER LISTS
            if (RolesHelper.IsDiamUser(UserInfo.Roles) || RolesHelper.IsClientValidator(UserInfo.Roles))
            {

                var regions = _regionService.GetRegionsByBrand(yourPlanogramsModel.BrandId).ToList();
                //regionFilter = IsInteger(this.regionFilterList.SelectedValue) ? int.Parse(this.regionFilterList.SelectedValue) : 0;
                var regionId = 0;

                if (regionId == 0)
                {
                    var userRegion = userCountry.Regions.FirstOrDefault();
                    if (userRegion != null)
                        regionId = userRegion.RegionId;
                }
                var countries = new List<Country>();

                countries = _countryService.GetCountriesByRegion(regionId).ToList();

                yourPlanogramsModel.Countries = countries.ToSelectListItems(-1).ToList();
                yourPlanogramsModel.Countries.Insert(0, new System.Web.Mvc.SelectListItem { Selected = true, Text = "Select a region first", Value = "0" });
                yourPlanogramsModel.Regions = regions.ToSelectListItems(-1).ToList();
                yourPlanogramsModel.Regions.Insert(0, new System.Web.Mvc.SelectListItem { Selected = true, Text = "Select a region", Value = "0" });

            }
            var brandedStandTypes = _standService.GetStandTypes(yourPlanogramsModel.BrandId).ToList();
            yourPlanogramsModel.StandTypes = brandedStandTypes.ToSelectListItems(-1).ToList();
            yourPlanogramsModel.StandTypes.Insert(0, new System.Web.Mvc.SelectListItem { Selected = true, Text = "Select a stand type", Value = "0" });



            //yourPlanogramsModel.StandTypes = StandService.GetStandTypesWithStands(createStandModel.BrandId).Select(st => (StandTypeViewModel)st).ToList();

            return CurrentTemplate(yourPlanogramsModel);
        }
    }
}