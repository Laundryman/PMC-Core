using CoreSystem2024.CMSModelBuilderModels;
using CoreSystem2024.Controllers.shop;
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
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace diam_planogram.Controllers
{
    //[Authorize]
    public class CreatePlanogramController : BaseMvcController
    {

        #region Services, managers

        private IStandService _standService;
        private IConfiguration _azureSettings;
        private IPlanogramService _planogramService;
        private IConfiguration _config;
        private readonly IConfiguration Configuration;
        private readonly IMemberManager _memberManager;
        public CreatePlanogramController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IStandService standService, IPlanogramService planogramService, IProductService productService, IConfiguration config, IConfiguration configuration, IMemberManager memberManager) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _standService = standService;
            _planogramService = planogramService;
            _config = configuration;
            _memberManager = memberManager;
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
        public async Task<IActionResult> CreatePlanogram(CreatePlanogram model)
        {

                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var userInfo = AuthHelper.GetUserInfo(memberIdentity);
                string urlReferrer = Request.Headers["Referer"].ToString();
                //if (string.IsNullOrEmpty(urlReferrer))
                //{
                //    Response.Redirect("/home");
                //    return null;
                //}

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
                        _planogramService.UnLockPlanogram(planoIdToUnLock, userInfo);
                    }
                    catch (Exception Ex)
                    {
                    }
                }


            var systemRole = RolesHelper.GetUserRole(userInfo.Roles, _config);
            //we will create a custom model


            model.UserFirstName = userInfo.GivenName;
            model.UserLastName = userInfo.Surname;
            model.BrandId = int.Parse(_config["AppSettings:ClientBrandId"]);
            model.ApiUrl = _config["AppSettings:apiURL"];
            model.ImageServerUrl = _config["AppSettings:ServerURL"];
            model.UserRoles = userInfo.Roles;
            model.SystemRole = (int)systemRole;


                model.StandTypes = _standService.GetStandTypesWithStands(model.BrandId).Select(st => (StandTypeViewModel)st).ToList();

            return CurrentTemplate(model);
        }
    }
}