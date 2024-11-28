using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using CoreSystem.Controllers;
using CoreSystem.Controllers.shop;
using CoreSystem.Helpers;
using CoreSystem.HttpClientWrapper;
using CoreSystem.Models;
using CoreSystemII.Controllers.Proxy;
using dplo.Domain;
using dplo.Domain.Entities;
using dplo.Service;
using dplo.Service.MSGraphUtils;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoreSystemII.Controllers
{
    [Authorize]
    public class CreatePlanogramApiController : BaseApiController
    {
        //private static readonly ILogger _logger = LogManager.GetLogger("System");

        #region Services, managers

        private ILogger<YourPlanogramApiController> _logger;
        public IPlanogramService _planogramService;
        public IBrandService _brandService;
        public ICountryService _countryService;
        private CreatePlanogramProxyController proxyApi;

        #endregion


        #region LocalApiCalls

        public CreatePlanogramApiController(ICategoryService categoryService, ICatalogueService catalogueService, ICountryService countryService, IPlanogramService planogramService, IOrderService orderService, IStandService standService, IBrandService brandService, ILogger<YourPlanogramApiController> logger) : base(categoryService, catalogueService, countryService, planogramService, orderService, standService)
        {
            _planogramService = planogramService;
            _brandService = brandService;
            _logger = logger;
            _countryService = countryService;
        }

        [HttpGet]
        [Route("/Api/CreatePlanogramApi/GetStands")]
        public async Task<IActionResult> GetStands(int standTypeId)
        {
            //we need to re-auth using the reauth process
            //client.RefreshAuthorization(Authorization);
            //RefreshUserSession(HttpContext.Current.Request, Authorization);
            //AuthHelper.ReAuth(Authorization, client);

            var response = await proxyApi.GetStandsWithClusters(standTypeId);
            
            //Get the json data from the result
            //IEnumerable<SelectListItem> stands = new IEnumerable<SelectListItem>();
            var stands = new List<SelectListItem>();



            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                stands = (List<SelectListItem>)result.Value;

            }
            else if (response is UnauthorizedResult)
            {
                //we need to re-auth using the reauth process

            }
            else
            {
                //Something has gone wrong, handle it here
            }

            return Ok( stands);
        }

        /// <summary>
        /// Gets the clusters associated with the stand
        /// </summary>
        /// <param name="standId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Api/CreatePlanogramApi/GetClusters")]
        public async Task<IActionResult> GetClusters(int standId)
        {
            //we need to re-auth using the reauth process
            //client.RefreshAuthorization(Authorization);
            //RefreshUserSession(HttpContext.Current.Request, Authorization);
            //AuthHelper.ReAuth(Authorization, client);

            var response = await proxyApi.GetClustersCall(standId);

            //Get the json data from the result
            var templates = new List<PlanogramClusterModel>();
            //var response =
            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                var templatesJson = result.Value;
                templates = JsonConvert.DeserializeObject<List<PlanogramClusterModel>>(templatesJson.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
            }
            return Ok(templates);

        }
        /// <summary>
        /// Gets the template planograms for the selected stand size.
        /// </summary>
        /// <param name="standId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Api/CreatePlanogramApi/GetTemplates")]
        public async Task<IActionResult> GetTemplates(int standId)
        {
            //we need to re-auth using the reauth process
            //client.RefreshAuthorization(Authorization);
            //RefreshUserSession(HttpContext.Current.Request, Authorization);
            //AuthHelper.ReAuth(Authorization, client);

            var response = await proxyApi.GetTemplatesCall(standId);

            //Get the json data from the result
            var templates = new List<PlanogramClusterModel>();
            //var response =
            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                var templatesJson = result.Value;
                templates = JsonConvert.DeserializeObject<List<PlanogramClusterModel>>(templatesJson.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
            }
            return Ok(templates);
        }

        [HttpGet]
        [Route("/Api/CreatePlanogramApi/CreatePlanogram")]
        public async Task<IActionResult> CreatePlanogram(int baseItemId, string planogramName)
        {
            try
            {

                //We've removed the make template option, so now we just always create a new planogram.
                var planogramId = 0;
                    var response = await proxyApi.CreatePlanogramCall(baseItemId, planogramName);
                    if (response is OkResult)
                    {
                        var result = response as OkObjectResult;
                        planogramId = int.Parse(result.Value.ToString());
                    }


                return Ok(planogramId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/Api/CreatePlanogramApi/EditPlanogram")]
        public async Task<IActionResult> EditPlanogram(int planogramId)
        {

            try
            {
                Planogram planogram = _planogramService.GetPlanogram(planogramId);

                var countryId = UserInfo.DiamCountryId;;
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
        public IActionResult UnlockPlano(int planogramId)
        {

            try
            {
                _planogramService.UnLockPlanogram(planogramId, UserInfo);
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
