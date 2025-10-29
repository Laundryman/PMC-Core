using System.Security.Claims;
using AutoMapper;
using CoreSystem2024.CMSModelBuilderModels;
using CoreSystem2024.Controllers.shop;
using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using PMApplication.Dtos;
using PMApplication.Helpers;
using PMApplication.Interfaces.ServiceInterfaces;
using PMApplication.Specifications.Filters;
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
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public CreatePlanogramController(ILogger<CreatePlanogramController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IStandService standService, IPlanogramService planogramService, IProductService productService, IConfiguration config, IConfiguration configuration, IMemberManager memberManager, IMapper mapper) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _standService = standService;
            _planogramService = planogramService;
            Configuration = configuration;
            _config = configuration;
            _memberManager = memberManager;
            _mapper = mapper;
            _logger = logger;
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
                        var lockFilter = new PlanogramLockFilter
                        {
                            PlanogramId = planoIdToUnLock,
                            User = userInfo
                        };
                    //string userId = UserInfo.id;
                    await _planogramService.UnLockPlanogram(lockFilter);
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

            var filter = new StandTypeFilter
            {
                BrandId = model.BrandId
            };
                //model.StandTypes = await _standService.GetStandTypesWithStands(model.BrandId).Select(st => (StandTypeDto)st).ToList();

                var standTypes = await _standService.GetStandTypes(filter);
                var stDtos = _mapper.Map<List<StandTypeDto>>(standTypes);
                model.StandTypes = stDtos;
            return CurrentTemplate(model);
        }
    }
}