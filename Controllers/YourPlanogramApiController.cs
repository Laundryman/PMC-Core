using CoreSystem2024.Controllers.shop;
using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using dplo.Domain.Entities;
using dplo.Service;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Web;
using CoreSystem2024.ProxyServices;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Security;
using CoreSystemII.Config;
using Umbraco.Cms.Core;
using System.Text.Json;

namespace CoreSystem2024.Controllers
{

    public class YourPlanogramApiController : BaseApiController
    {
        //private static readonly ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ILogger<YourPlanogramApiController> _logger;
        //private YourPlanogramProxyController _proxyApi = new ProxiApi();

        #region Services, managers

        private ICountryService _countryService;
        private IYourPlanogramProxyApiService _proxyApi;
        private readonly IConfiguration _config;
        private readonly IMemberManager _memberManager;
        private readonly EmailHelper _emailHelper;

        #endregion

        #region LocalApiCalls



        public YourPlanogramApiController(ICategoryService categoryService, ICatalogueService catalogueService, ICountryService countryService, 
            IPlanogramService planogramService, IOrderService orderService, 
            IStandService standService, IYourPlanogramProxyApiService proxyApi, IMemberManager memberManager, IConfiguration config, EmailHelper emailHelper, ILogger<YourPlanogramApiController> logger) : base(config)
        {
            _countryService = countryService;
            _proxyApi = proxyApi;
            _memberManager = memberManager;
            _config = config;
            _emailHelper = emailHelper;
            _logger = logger;
        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/LockPlanogram")]
        public async Task<IActionResult> LockPlanogram(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await _proxyApi.LockPlanogramCall(planogramId);

            if (response is OkResult)
            {
                return Ok(planogramId);

            }
            else
            {
                return Conflict(planogramId);
            }
        }


        [HttpPost]
        [Route("/Api/YourPlanogramApi/RenamePlanogram")]
        public async Task<IActionResult> RenamePlanogram([FromBody] PlanogramUpdate data)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            try
            {
                var response = await _proxyApi.RenamePlanogramCall(data);
                return Ok(response);
            }
            catch (Exception ex) {
    
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetStandTypes")]
        public async Task<IActionResult> GetStandTypes(int brandId)
        {
            try
            {
                var response = await _proxyApi.GetStandTypesCall(brandId);

                return Ok(response);
            }

            catch
            {
                return BadRequest(brandId);
            }
        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetRegions")]
        public async Task<IActionResult> GetRegions(int brandId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            try {
                var response = await _proxyApi.GetRegionsCall(brandId);
                return Ok(response);
            }
            catch
            {
                return BadRequest(brandId);
            }
        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetCountriesByRegion")]
        public async Task<IActionResult> GetCountriesByRegion(int regionId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            try {
                var response = await _proxyApi.GetCountriesByRegionCall(regionId);

                return Ok(response);

            }
            catch
            {
                return BadRequest(regionId);
            }
        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetCommentCount")]
        public async Task<IActionResult> GetCommentsCount(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            try {
                var response = await _proxyApi.GetCommentsCountCall(planogramId);
                return Ok(response);

            }
            catch
            {
                return BadRequest(planogramId);
            }
        }



        /// <summary>
        /// Gets the job numbers for the brand
        /// </summary>
        /// <returns>JobViewModel List</returns>
        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetJobNumbers")]
        public async Task<IActionResult> GetJobNumbers()
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            try
            {
                var response = await _proxyApi.GetJobNumbersCall();

                return Ok(response);
            }
            catch

            {
                //Something has gone wrong, handle it here
                return BadRequest();
            }

        }

        /// <summary>
        /// Gets the job numbers for the brand
        /// </summary>
        /// <returns>JobViewModel List</returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/GetJobFolders")]
        public async Task<IActionResult> GetJobFolders([FromBody] GetPlanoParams data)
        {
            try
            {
                var response = await _proxyApi.GetJobFoldersCall(data.CountryId, data.RegionId, data.StandTypeId);
                //Get the json data from the result

                if (response.Any())
                {
                    response = response.OrderBy(j => j.Name).ToList();
                }

                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetJobNumbersForFolder")]
        public async Task<IActionResult> GetJobNumbersForFolder(int jobFolderId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);
            try {

            var response = await _proxyApi.GetJobNumbersForFoldersCall(jobFolderId);
                return Ok(response);
            }
            catch
            {
                //Something has gone wrong, handle it here
                return BadRequest();
            }

        }


        /// <summary>
        /// Archives a planogram
        /// </summary>
        /// <param name="planogramId"></param>
        /// <param name="jobNumber"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/GetPlanograms")]
        public async Task<IActionResult> GetPlanograms([FromBody] GetPlanoParams data)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            RolesHelper.Initialize(_config);

            IEnumerable<PlanogramInfo> response;
            try
            {
                int status = data.Status;
                if (RolesHelper.IsAdministrator(userInfo.Roles) || (RolesHelper.IsValidator(userInfo.Roles))
                    || (RolesHelper.IsApprover(userInfo.Roles)))
                {
                    if (RolesHelper.IsSuperUser(userInfo.Roles))
                    {
                        response = await _proxyApi.GetPlanogramsCall(status, data.CountryId, data.RegionId, data.StandTypeId);
                    }
                    else
                    {
                        response = await _proxyApi.GetPlanogramsCall(status, _countryService.GetCountry(userInfo.DiamCountryId).CountryId, data.RegionId, data.StandTypeId);
                    }
                }
                else
                {
                    response = await _proxyApi.GetArchivedPlanogramsByJobCodeCall(data.JobCode, _countryService.GetCountry(userInfo.DiamCountryId).CountryId, data.RegionId, data.StandTypeId);
                }

                    return Ok(response);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Archives a planogram
        /// </summary>
        /// <param name="planogramId"></param>
        /// <param name="jobNumber"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/GetArchivedPlanogramsByJobCode")]
        public async Task<IActionResult> GetArchivedPlanogramsByJobCode([FromBody] GetPlanoParams data)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);


            IEnumerable<PlanogramInfo> response;
            try
            {
                if (RolesHelper.IsAdministrator(userInfo.Roles) || (RolesHelper.IsValidator(userInfo.Roles))
                    || (RolesHelper.IsApprover(userInfo.Roles)))
                {
                    if (RolesHelper.IsSuperUser(userInfo.Roles))
                    {
                        response = await _proxyApi.GetArchivedPlanogramsByJobCodeCall(data.JobCode, _countryService.GetCountry(userInfo.DiamCountryId).CountryId, data.RegionId, data.StandTypeId);
                    }
                    else
                    {
                        response = await _proxyApi.GetArchivedPlanogramsByJobCodeCall(data.JobCode, data.CountryId, data.RegionId, data.StandTypeId);
                    }
                }
                else
                {
                    response = await _proxyApi.GetArchivedPlanogramsByJobCodeCall(data.JobCode, _countryService.GetCountry(userInfo.DiamCountryId).CountryId, data.RegionId, data.StandTypeId);
                }

                    return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Submits a planogram
        /// </summary>
        /// <param name="planogramId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/SubmitPlanogram/")]
        public async Task<IActionResult> SubmitPlanogram(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);
            try {

            var response = await _proxyApi.SubmitPlanogramCall(planogramId);

                ///////////////
                //if we get an ok we need to save an entry in the audit tracking table - used to use the Logger for this - but maybe not anymore
                /// We Also need to send an email
                /// ////////////
                //var planogram = await _proxyApi.GetPlanogramCall(planogramId);
                SendSubmittedEmail(planogramId, _config);
                return Ok(planogramId);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Approves a planogram to approved
        /// </summary>
        /// <param name="planogramId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/ApprovePlanogram/")]
        public async Task<IActionResult> ApprovePlanogram(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);
            try
            {

                var response = await _proxyApi.ApprovePlanogramCall(planogramId);

                ///////////////
                //if we get an ok we need to save an entry in the audit tracking table - used to use the Logger for this - but maybe not anymore
                /// We Also need to send an email
                /// ////////////
                //var planogram = await _proxyApi.GetPlanogramCall(planogramId);
                return Ok(planogramId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Rejects a planogram from approved to submitted
        /// </summary>
        /// <param name="planogramId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/RejectPlanogram/")]
        public async Task<IActionResult> RejectPlanogram(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);
            try
            {

                var response = await _proxyApi.RejectPlanogramCall(planogramId);

                ///////////////
                //if we get an ok we need to save an entry in the audit tracking table - used to use the Logger for this - but maybe not anymore
                /// We Also need to send an email
                /// ////////////
                return Ok(planogramId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Validates or unvalidates an approved planogram
        /// </summary>
        /// <param name="planogramId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/ValidatePlanogram/")]
        public async Task<IActionResult> ValidatePlanogram(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);
            try
            {

                var response = await _proxyApi.ValidatePlanogramCall(planogramId);

                ///////////////
                //if we get an ok we need to save an entry in the audit tracking table - used to use the Logger for this - but maybe not anymore
                /// We Also need to send an email
                /// ////////////
                //var planogram = await _proxyApi.GetPlanogramCall(planogramId);
                return Ok(planogramId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Archives a planogram
        /// </summary>
        /// <param name="planogramId"></param>
        /// <param name="jobNumber"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/ArchivePlanogram")]
        public async Task<IActionResult> ArchivePlanogram([FromBody] ArchivePlanogramData data)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await _proxyApi.ArchivePlanogramCall(data.PlanogramId, data.JobNumber);

            //Get the result
            if (response is OkResult)
            {
                return Ok(data.PlanogramId);
            }
            else
            {
                return BadRequest();
            }

        }



        [Route("/Api/YourPlanogramApi/GetReport")]
        [HttpPost]
        public async Task<IActionResult> GetReport([FromBody] GetReportParams data)
        {

            try
            {
                var filePath = "";
                if (data.ReportType == (int)ReportTypes.SkuList)
                {
                    var response = await _proxyApi.CreateSkuList(data.PlanogramId);

                    return Ok(response);

                }

                if (data.ReportType == (int)ReportTypes.CassetteList)
                {
                    var response = await _proxyApi.CreateCassetteList(data.PlanogramId);

                        var fileJson = response;
                        filePath = fileJson.ToString();

                }
                return Ok(filePath);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }


        /// <summary>
        /// Obsolete Generates the skulist on the server
        /// </summary>
        /// <returns>FileDesc entity List</returns>
        /// 
        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetSkuList")]
        public async Task<IActionResult> GetSkuList(int planogramId)
        {
            try
            {
                _logger.LogDebug("Calling CreateSkuList");

                var response = await _proxyApi.CreateSkuList(planogramId);

                    var fileJson = response;
                    //result = fileJson;
                    return Ok(HttpUtility.UrlEncode(fileJson.ToString()));

            }
            catch (Exception ex)
            {
                _logger.LogDebug("response from CreateSkuList = " + ex.Message);

                //Something has gone wrong, handle it here
                throw new Exception("failed to get the skulist");

            }
        }
        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetJsonSkuList")]
        public async Task<IActionResult> GetJsonSkuList(int planogramId)
        {
            try
            {
                _logger.LogDebug("Calling CreateSkuList with planogramId " + planogramId.ToString());

                var response = await _proxyApi.CreateJsonSkuList(planogramId);
                //IEnumerable<SkuList> SkuList = response;
                if (response != null)
                {
                    var skuList = response.ToList();
                    var json = JsonSerializer.Serialize(skuList);
                    return Ok(HttpUtility.UrlEncode(json.ToString()));
                }
                else
                {
                    return BadRequest("Nothing found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateSkuList error = " + ex.Message);

                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetOpenOrder")]
        public async Task<IActionResult> GetOpenOrder(int planogramId)
        {
            try
            {
                var response = await _proxyApi.GetOpenOrderCall(planogramId);

                Order order;

                var responseJson = response.ToString();
                order = JsonSerializer.Deserialize<Order>(responseJson);

                return Ok(order);
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetOpenOrders")]
        public async Task<IActionResult> GetOpenOrders(int planogramId)
        {

            try {
                var response = await _proxyApi.GetOpenOrdersCall(planogramId);
                List<Order> orders;
                orders = response.ToList();
                return Ok(orders);
            }
            catch
            {
                return BadRequest();
            }

        }


        [HttpPost]
        [Route("/Api/YourPlanogramApi/AddToOrder")]
        public async Task<IActionResult> AddToOrder(AddToOrder model)
        {
            try {
                var response = await _proxyApi.AddToOrderCall(model.OrderId, model.PlanogramId, model.Quantity, model.IsFullPlano);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }



        #endregion

        #region helper functions

        private async Task SendSubmittedEmail(int planogramId, IConfiguration config)
        {

            if (config["EmailSettings:EmailEnabled"] == "true")
            {
                try
                {

                    var email = new Email()
                    {
                        BccList = config["EmailSettings:BCCList"],
                        DateSent = DateTime.Now,
                        EmailTrigger = (int)EmailTrigger.PlanogramSubmitted,
                        FromAddress = config["EmailSettings:FromAddress"],
                        PlanogramId = planogramId,
                        ToAddress = config["EmailSettings:ToAddress"],
                        RecipientName = config["EmailSettings:RecipientName"],
                        //UserId = UserInfo.Id,
                        EmailEnabled = config["EmailSettings:EmailEnabled"] == "true",
                        EmailSubject = "Planogram Submitted"
                    };
                    var response = await _emailHelper.PlanogramSubmittedEmail(email);

                }
                catch (Exception ex)
                {
                    //SystemLog.DebugFormat("asyncSendMail exception " + ex.ToString());
                    _logger.LogError("planogram submitted email exception " + ex.Message);
                }
                //}
            }
        }

        #endregion
    }
}
