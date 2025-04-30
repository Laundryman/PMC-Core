using CoreSystem2024.CMSModelBuilderModels;
using CoreSystem2024.Controllers.shop;
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
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;

namespace diam_planogram.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class YourPlanogramsController : BaseMvcController
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
        private readonly IConfiguration _config;
        private readonly IMemberManager _memberManager;

        // GET: CreateStand
        public YourPlanogramsController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IStandService standService, ICountryService countryService, IRegionService regionService, IConfiguration configuration, IConfiguration config, IMemberManager memberManager) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _standService = standService;
            _countryService = countryService;
            _regionService = regionService;
            Configuration = configuration;
            _config = config;
            _memberManager = memberManager;
            domain = Configuration["AppSettings:ApiUrl"];
            brandId = Configuration["AppSettings:ClientBrandId"];
            readScope = Configuration["AzureB2C:ReadScope"];
            writeScope = Configuration["AzureB2CWriteScope"];
        }

        public async Task<IActionResult> YourPlanograms(YourPlanograms model)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var userCountry = _countryService.GetCountry(userInfo.DiamCountryId);

            var systemRole = RolesHelper.GetUserRole(userInfo.Roles, _config);
            //we will create a custom model

            model.UserFirstName = userInfo.GivenName;
            model.UserLastName = userInfo.Surname;
            model.BrandId = int.Parse(brandId);
            model.ApiUrl = domain;
            model.UserRoles = userInfo.Roles;
            model.SystemRole = (int)systemRole;



            //GET THE FILTER LISTS
            if (RolesHelper.IsRegionalUser(userInfo.Roles) || RolesHelper.IsClientValidator(userInfo.Roles))
            {

                var regions = _regionService.GetRegionsByBrand(model.BrandId).ToList();
                model.RegionId = 0;

                
                var userRegion = userCountry.Regions.FirstOrDefault(r => r.BrandId == model.BrandId);
                if (userRegion != null)
                {
                    model.RegionId = userRegion.RegionId;
                }
                    
                var countries = new List<Country>();

                countries = _countryService.GetCountriesByRegion(model.RegionId).ToList();

                model.Countries = countries.ToSelectListItems(userCountry.CountryId).ToList();

                //need to change the text if is regional manager
                model.Countries.Insert(0, new System.Web.Mvc.SelectListItem { Selected = true, Text = "Select a region first", Value = "0" });
                model.Regions = regions.ToSelectListItems(model.RegionId).ToList();
                model.Regions.Insert(0, new System.Web.Mvc.SelectListItem { Selected = false, Text = "Select a region", Value = "0" });

            }
            var brandedStandTypes = _standService.GetStandTypes(model.BrandId).ToList();
            model.StandTypes = brandedStandTypes.ToSelectListItems(-1).ToList();
            model.StandTypes.Insert(0, new System.Web.Mvc.SelectListItem { Selected = true, Text = "Select a stand type", Value = "0" });



            //model.StandTypes = StandService.GetStandTypesWithStands(createStandModel.BrandId).Select(st => (StandTypeViewModel)st).ToList();

            return CurrentTemplate(model);
        }
    }
}