using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using dplo.Service;
using dplo.Service.MSGraphUtils;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace diam_planogram.Controllers
{
    //[Authorize]
    public class CreatePlanogramController : RenderController
    {

        #region Services, managers

        private IStandService _standService;
        private IConfiguration _azureSettings;
        private IPlanogramService _planogramService;
        private IConfiguration _config;

        public CreatePlanogramController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IStandService standService, IPlanogramService planogramService, IProductService productService, IConfiguration config) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _standService = standService;
            _planogramService = planogramService;
            _azureSettings = config.GetSection("AzureB2C");
            config = config;
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
                var proxySupport = new ProxyApiSupport(_config);
                //// Retrieve the token with the specified scopes
                var result = await proxySupport.AcquireTokenForScopes(new string[]
                    {_azureSettings["ReadScope"], _azureSettings["WriteScope"]});
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