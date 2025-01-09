using CoreSystem2024.Controllers;
using CoreSystem2024.Controllers.shop;
using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using CoreSystem2024.ProxyServices;
using dplo.Domain;
using dplo.Domain.Entities;
using dplo.Service;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Security;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace CoreSystemII.Controllers
{
    [Authorize]
    public class CreatePlanogramApiController : BaseApiController
    {
        //private static readonly ILogger _logger = LogManager.GetLogger("System");

        #region Services, managers

        private ILogger<YourPlanogramApiController> _logger;
        private readonly IPlanogramService _planogramService;
        private readonly IBrandService _brandService;
        private readonly ICountryService _countryService;
        private readonly ICreatePlanogramProxyService _proxyApi;
        private readonly IMemberManager _memberManager;
        private readonly IConfiguration _config;

        #endregion


        #region LocalApiCalls

        public CreatePlanogramApiController(ICountryService countryService, IPlanogramService planogramService, IBrandService brandService, ILogger<YourPlanogramApiController> logger, IMemberManager memberManager, ICreatePlanogramProxyService proxyApi, IConfiguration config) : base(config)
        {
            _planogramService = planogramService;
            _brandService = brandService;
            _logger = logger;
            _memberManager = memberManager;
            _proxyApi = proxyApi;
            _config = config;
            _countryService = countryService;
        }

        [HttpGet]
        [Route("/Api/CreatePlanogramApi/GetStands")]
        public async Task<IEnumerable<SelectListItem>> GetStands(int standTypeId)
        {
            //we need to re-auth using the reauth process
            //client.RefreshAuthorization(Authorization);
            //RefreshUserSession(HttpContext.Current.Request, Authorization);
            //AuthHelper.ReAuth(Authorization, client);

            var response = await _proxyApi.GetStandsWithClusters(standTypeId);


            return response;
        }

        /// <summary>
        /// Gets the clusters associated with the stand
        /// </summary>
        /// <param name="standId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Api/CreatePlanogramApi/GetClusters")]
        public async Task<IEnumerable<PlanogramClusterModel>> GetClusters(int standId)
        {
            //we need to re-auth using the reauth process
            //client.RefreshAuthorization(Authorization);
            //RefreshUserSession(HttpContext.Current.Request, Authorization);
            //AuthHelper.ReAuth(Authorization, client);

            var response = await _proxyApi.GetClustersCall(standId);

            return response;

        }
        /// <summary>
        /// Gets the template planograms for the selected stand size.
        /// </summary>
        /// <param name="standId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Api/CreatePlanogramApi/GetTemplates")]
        public async Task<IEnumerable<PlanogramClusterModel>> GetTemplates(int standId)
        {
            //we need to re-auth using the reauth process
            //client.RefreshAuthorization(Authorization);
            //RefreshUserSession(HttpContext.Current.Request, Authorization);
            //AuthHelper.ReAuth(Authorization, client);

            var response = await _proxyApi.GetTemplatesCall(standId);

            //Get the json data from the result
            var templates = new List<PlanogramClusterModel>();
            //var response =
            return response;
        }

        [HttpGet]
        [Route("/Api/CreatePlanogramApi/CreatePlanogram")]
        public async Task<int> CreatePlanogram(int baseItemId, string planogramName)
        {
            try
            {

                //We've removed the make template option, so now we just always create a new planogram.
                var response = await _proxyApi.CreatePlanogramCall(baseItemId, planogramName);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/Api/CreatePlanogramApi/EditPlanogram")]
        public async Task<IActionResult> EditPlanogram(int planogramId)
        {

            try
            {
                Planogram planogram = _planogramService.GetPlanogram(planogramId);

                var countryId = UserInfo.DiamCountryId; ;
                Country country = _countryService.GetCountry(countryId);
                var brand = _brandService.GetBrand(int.Parse(ConfigurationManager.AppSettings["brand"]));
                var editPCreds = new EditPlanogramCreds
                {
                    Action = "edit",
                    UserId = UserInfo.Id,
                    PlanogramId = planogram.PlanogramId,
                    clusterId = planogram.ClusterId,
                    countryId = country.CountryId.ToString(),
                    apiURL = ConfigurationManager.AppSettings["apiURL"],
                    cookieDomain = ConfigurationManager.AppSettings["cookieDomain"],
                    brandId = brand.BrandId.ToString(),
                    uname = UserInfo.UserName,
                    //accessToken = Authorization.AccessToken,
                    debug = ConfigurationManager.AppSettings["debug"],
                    themeId = brand.ThemeId

                };
                return Ok(editPCreds);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        [Route("/Api/CreatePlanogramApi/UnlockPlano")]
        public async Task<IActionResult> UnlockPlano(int planogramId)
        {

            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var userProfile = AuthHelper.GetUserInfo(memberIdentity);
                _planogramService.UnLockPlanogram(planogramId, userProfile);
                return Ok();
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex.Message);
            }
        }

        #endregion




        #region helper functions

        #endregion
    }
}
