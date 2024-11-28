using System.Configuration;
using CoreSystem.Helpers;
using CoreSystem.Models;
using dplo.Service;
using Dplo.ViewModels;
using dplo.Service.MSGraphUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace diam_planogram.Controllers
{
    //[Authorize]
    public class CreatePlanogramController : RenderController
    {

        #region Services, managers

        private IStandService _standService;
        private IPlanogramService _planogramService;
        private ICatalogueService _catalogueService;
        private ICountryService _countryService;
        private ICategoryService _categoryService;
        private IProductService _productService;

        public CreatePlanogramController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IStandService standService, IPlanogramService planogramService, ICatalogueService catalogueService, ICountryService countryService, ICategoryService categoryService, IProductService productService) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _standService = standService;
            _planogramService = planogramService;
            _catalogueService = catalogueService;
            _countryService = countryService;
            _categoryService = categoryService;
            _productService = productService;
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


        // GET: CreateStand
        public async Task<IActionResult> CreatePlanogram()
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

            string urlReferrer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(urlReferrer))
            {
                Response.Redirect("/home");
                return null;
            }
            if (urlReferrer.Contains("edit-planogram.aspx"))
                {
                    var querystring = Request.Query;
                    var qparams = Request.Query;
                    var paramsList = new List<Tuple<string, string>>();
                    var planoIdToUnLock = 0;
                    foreach (var param in qparams)
                    {
                        if (param.Key.ToLower() == "pid")
                        {
                            planoIdToUnLock = int.Parse(param.Value);
                            break;
                        }
                    }
                    try
                    {
                        //string userId = UserInfo.Id;
                        _planogramService.UnLockPlanogram(planoIdToUnLock, UserInfo.userViewModel);
                    }
                    catch (Exception Ex)
                    {
                    }
                }

                var systemRole = RolesHelper.GetUserRole(UserInfo.Roles);
                //we will create a custom model
                var createStandModel = new CreateStandModel
                {
                    UserFirstName = UserInfo.GivenName,
                    UserLastName = UserInfo.Surname,
                    BrandId = int.Parse(ConfigurationManager.AppSettings["brand"]),
                    ApiUrl = ConfigurationManager.AppSettings["apiURL"],
                    UserRoles = UserInfo.Roles,
                    SystemRole = (int)systemRole

                };

                createStandModel.StandTypes = _standService.GetStandTypesWithStands(createStandModel.BrandId).Select(st => (StandTypeViewModel)st).ToList();

                return CurrentTemplate(createStandModel);
        }
    }
}