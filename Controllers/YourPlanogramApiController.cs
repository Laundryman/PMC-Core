using CoreSystem2024.Controllers.shop;
using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using CoreSystemII.Controllers.Proxy;
using dplo.Domain.Entities;
using dplo.Service;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web;
using UserInfo = CoreSystem2024.Helpers.UserInfo;

namespace CoreSystem2024.Controllers
{

    public class YourPlanogramApiController : BaseApiController
    {
        //private static readonly ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ILogger<YourPlanogramApiController> _logger;
        private YourPlanogramProxyController proxyApi;

        #region Services, managers

        private ICountryService _countryService;
        #endregion

        #region LocalApiCalls

        public YourPlanogramApiController(ICategoryService categoryService, ICatalogueService catalogueService, ICountryService countryService, IPlanogramService planogramService, IOrderService orderService, IStandService standService) : base(categoryService, catalogueService, countryService, planogramService, orderService, standService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/LockPlanogram")]
        public async Task<IActionResult> LockPlanogram(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.LockPlanogramCall(planogramId);

            if (response is OkResult)
            {
                return Ok(planogramId);

            }
            else
            {
                return Conflict(planogramId);
            }
        }


        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetCommentCount")]
        public async Task<IActionResult> GetCommentsCount(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetCommentsCountCall(planogramId);
            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                return Ok(result.Value);

            }
            else
            {
                return BadRequest(planogramId);
            }
        }


        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetYourInProgress")]
        public async Task<IActionResult> GetYourInProgress(int standTypeId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetInProgress();

            //Get the json data from the result
            //IEnumerable<SelectListItem> stands = new IEnumerable<SelectListItem>();
            var stands = new List<SelectListItem>();



            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                stands = (List<SelectListItem>)result.Value;

            }
            else
            {
                return BadRequest();
            }

            return Ok(stands);
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


            var response = await proxyApi.GetJobNumbersCall();

            //Get the json data from the result
            var jobs = new List<JobViewModel>();
            //var response =
            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                var jobsJson = result.Value.ToString();
                jobs = JsonConvert.DeserializeObject<List<JobViewModel>>(jobsJson);
            }
            else
            {
                //Something has gone wrong, handle it here
                return BadRequest();
            }
            return Ok(jobs);

        }

        /// <summary>
        /// Gets the job numbers for the brand
        /// </summary>
        /// <returns>JobViewModel List</returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/GetJobFolders")]
        public async Task<IActionResult> GetJobFolders([FromBody] GetArchivedPlanoParams data)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetJobFoldersCall(data.CountryId, data.RegionId, data.StandTypeId);
            //Get the json data from the result
            var jobs = new List<JobFolderViewModel>();
            //var response =
            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                var jobsJson = result.Value.ToString();
                jobs = JsonConvert.DeserializeObject<List<JobFolderViewModel>>(jobsJson);

                if (jobs.Any())
                {
                    jobs = jobs.OrderBy(j => j.Name).ToList();
                }
            }
            else
            {
                //Something has gone wrong, handle it here
                return BadRequest();
            }
            return Ok(jobs);

        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetJobNumbersForFolder")]
        public async Task<IActionResult> GetJobNumbersForFolder(int jobFolderId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetJobNumbersForFoldersCall(jobFolderId);

            //Get the json data from the result
            var jobs = new List<JobViewModel>();
            //var response =
            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                var jobsJson = result.Value.ToString();
                jobs = JsonConvert.DeserializeObject<List<JobViewModel>>(jobsJson);
            }
            else
            {
                //Something has gone wrong, handle it here
                return BadRequest();
            }
            return Ok(jobs);

        }


        /// <summary>
        /// Archives a planogram
        /// </summary>
        /// <param name="planogramId"></param>
        /// <param name="jobNumber"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Api/YourPlanogramApi/GetArchivedPlanogramsByJobCode")]
        public async Task<IActionResult> GetArchivedPlanogramsByJobCode([FromBody] GetArchivedPlanoParams data)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            IActionResult response;
            try
            {
                if (RolesHelper.IsAdministrator(UserInfo.Roles) || (RolesHelper.IsValidator(UserInfo.Roles))
                    || (RolesHelper.IsApprover(UserInfo.Roles)))
                {
                    if (RolesHelper.IsSuperUser(UserInfo.Roles))
                    {
                        response = await proxyApi.GetArchivedPlanogramsByJobCodeCall(data.JobCode, _countryService.GetCountry(UserInfo.DiamCountryId).CountryId, data.RegionId, data.StandTypeId);
                    }
                    else
                    {
                        response = await proxyApi.GetArchivedPlanogramsByJobCodeCall(data.JobCode, data.CountryId, data.RegionId, data.StandTypeId);
                    }
                }
                else
                {
                    response = await proxyApi.GetArchivedPlanogramsByJobCodeCall(data.JobCode, _countryService.GetCountry(UserInfo.DiamCountryId).CountryId, data.RegionId, data.StandTypeId);
                }
                //Get the json data from the result
                var planos = new List<PlanogramInfo>();
                if (response is OkResult)
                {
                    var result = response as OkObjectResult;
                    var planosJson = result.Value.ToString();
                    planos = JsonConvert.DeserializeObject<List<PlanogramInfo>>(planosJson);
                    return Ok(planos);

                }
                else
                {
                    return BadRequest();
                }
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


            var response = await proxyApi.ArchivePlanogramCall(data.PlanogramId, data.JobNumber);

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
                    var response = await proxyApi.CreateSkuList(data.PlanogramId);

                    if (response is OkResult)
                    {
                        var result = response as OkObjectResult;
                        var fileJson = result.Value.ToString();
                        filePath = fileJson;
                    }
                    else
                    {
                        //Something has gone wrong, handle it here
                    }

                }

                if (data.ReportType == (int)ReportTypes.CassetteList)
                {
                    var response = await proxyApi.CreateCassetteList(data.PlanogramId);

                    if (response is OkResult)
                    {
                        var result = response as OkObjectResult;
                        var fileJson = result;
                        filePath = fileJson.Value.ToString();
                    }
                    else
                    {
                        //Something has gone wrong, handle it here
                    }

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

                var response = await proxyApi.CreateSkuList(planogramId);

                //Get the json data from the result
                //var result = "";
                //var response =
                if (response is OkResult)
                {
                    var result = response as OkObjectResult;
                    var fileJson = result.Value;
                    //result = fileJson;
                    return Ok(HttpUtility.UrlEncode(fileJson.ToString()));
                }
                else
                {
                    var result = response as BadRequestObjectResult;
                    _logger.LogDebug("response from CreateSkuList = " + result.Value);

                    //Something has gone wrong, handle it here
                    throw new Exception("failed to get the skulist");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetJsonSkuList")]
        public async Task<IActionResult> GetJsonSkuList(int planogramId)
        {
            try
            {
                _logger.LogDebug("Calling CreateSkuList with planogramId " + planogramId.ToString());

                var response = await proxyApi.CreateJsonSkuList(planogramId);

                if (response is OkResult)
                {
                    var result = response as OkObjectResult;
                    var json = result;
                    return Ok(HttpUtility.UrlEncode(json.Value.ToString()));
                }
                else
                {
                    var result = response as BadRequestObjectResult;
                    _logger.LogError("response from CreateSkuList = " + result.Value.ToString());

                    //Something has gone wrong, handle it here
                    throw new Exception("failed to get the skulist");
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
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            var response = await proxyApi.GetOpenOrderCall(planogramId);

            Order order;

            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                if (result != null)
                {
                    var responseJson = result.Value.ToString();
                    order = JsonConvert.DeserializeObject<Order>(responseJson);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            return Ok(order);

        }

        [HttpGet]
        [Route("/Api/YourPlanogramApi/GetOpenOrders")]
        public async Task<IActionResult> GetOpenOrders(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            var response = await proxyApi.GetOpenOrdersCall(planogramId);

            List<Order> orders;

            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                if (result != null)
                {
                    var responseJson = result.Value.ToString();
                    orders = JsonConvert.DeserializeObject<List<Order>>(responseJson);
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return response;
            }

            return Ok(orders);

        }


        [HttpPost]
        [Route("/Api/YourPlanogramApi/AddToOrder")]
        public async Task<IActionResult> AddToOrder(AddToOrder model)
        {
            var response = await proxyApi.AddToOrderCall(model.OrderId, model.PlanogramId, model.Quantity, model.IsFullPlano);

            if (response is OkResult)
            {
                return Ok();
            }
            else
            {
                return response;
            }
        }



        #endregion


        //#region remote api calls

        //private async Task<IActionResult> LockPlanogramCall(int planogramId)
        //{
        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brand = ConfigurationManager.AppSettings["brand"];

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });


        //    var uriSuffix = "api/v2/planogram/lock/" + planogramId;


        //    using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}
        //private async Task<IActionResult> GetCommentsCountCall(int planogramId)
        //{
        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];
        //    //Confirm the authorization so we can call the api 
        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });


        //    var uriSuffix = "api/v2/planogram/getCommentCount/" + planogramId + "/" + brandId ;


        //    using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }


        //}

        //private async Task<IActionResult> CreateSkuList(int planogramId)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];
        //    //Confirm the authorization so we can call the api 
        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var isPowerUser = false;
        //    var isDiamUser = false;

        //    var uriSuffix = "api/v2/planogram/get/skulist/" + planogramId ;

        //    //maybe log something here

        //    using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix ))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //           return BadRequest(ex.Message);
        //        }
        //    }

        //}

        //private async Task<IActionResult> CreateJsonSkuList(int planogramId)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];
        //    //Confirm the authorization so we can call the api 
        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var isPowerUser = false;
        //    var isDiamUser = false;

        //    var uriSuffix = "api/v2/planogram/get/jsonskulist/" + planogramId;
        //    _logger.LogDebug("making api call with url " + uriSuffix);
        //    //maybe log something here

        //    using (SecureHttpClient<IEnumerable<SkuList>> httpClient = new SecureHttpClient<IEnumerable<SkuList>>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogDebug("error calling dssapi from createjsonskulist " + ex.Message);
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}

        //private async Task<IActionResult> CreateCassetteList(int planogramId)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/planogram/get/casslist/" + planogramId;

        //    //maybe log something here

        //    using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }


        //}


        //private async Task<IActionResult> ArchivePlanogramCall(int planogramId, string jobNumber)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/planogram/archive/" + planogramId + "/" + jobNumber;

        //    //maybe log something here

        //    using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}

        //private async Task<IActionResult> GetArchivedPlanogramsByJobCodeCall(string jobCode, int countryId = 0, int regionId = 0, int standTypeId = 0)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];

        //    var isPowerUser = false;
        //    var isDiamUser = false;

        //    if (RolesHelper.IsAdministrator(UserInfo.Roles))
        //    {
        //        isDiamUser = true;
        //        isPowerUser = true;
        //        //planograms = planogramService.GetInProgressPlanograms(0, BrandId, countryFilter, regionFilter, standTypeFilter, true);

        //    }
        //    else if (RolesHelper.IsValidator(UserInfo.Roles))
        //    {
        //        isPowerUser = true;
        //        //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
        //        if (countryId == 0)
        //        {
        //            Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

        //        }
        //    }
        //    else if (RolesHelper.IsApprover(UserInfo.Roles))
        //    {
        //        isPowerUser = true;

        //    }
        //    else
        //    {
        //        //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
        //        if (countryId == 0)
        //        {
        //            //need to make this a non local call - either api - or get the ID from the userInfo
        //            Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

        //        }
        //    }


        //    var uri = "api/v2/planogram/get/archived/jobcode/" + isPowerUser + "/" + jobCode + "/" + brandId + "/" + countryId + "/" + regionId + "/" + standTypeId + "/" + isDiamUser.ToString() ;

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });


        //    var url = string.Format("{0}{1}", domain, uri);

        //    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
        //    {
        //        using (HttpClient httpClient = new HttpClient())
        //        {
        //            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //            var response = await proxyApi.httpClient.SendAsync(request);

        //            var StatusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
        //            var responseBodyAsText = await proxyApi.response.Content.ReadAsStringAsync();
        //            responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
        //            _logger.LogDebug("response from GetPlanogramsByJobCode = " + responseBodyAsText + " :: " + StatusText);

        //            return Ok(response);
        //        }
        //    }

        //}

        //private async Task<IActionResult> GetInProgress(int countryId =0, int regionId = 0, int standTypeId = 0)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];
        //    //Confirm the authorization so we can call the api 
        //    //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
        //    //var accessToken = currentAuth.AccessToken;
        //    //var country = CountryService.GetCountry(UserInfo.DiamCountryId);

        //    var isDiamUser = false;

        //    if (RolesHelper.IsAdministrator(UserInfo.Roles))
        //    {
        //        isDiamUser = true;
        //        //planograms = planogramService.GetInProgressPlanograms(0, BrandId, countryFilter, regionFilter, standTypeFilter, true);

        //    }
        //    else if (RolesHelper.IsValidator(UserInfo.Roles))
        //    {
        //        //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
        //        if (countryId == 0)
        //        {
        //            Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

        //        }
        //    }
        //    else if (RolesHelper.IsApprover(UserInfo.Roles))
        //    {

        //    }
        //    else
        //    {
        //        //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
        //        if (countryId == 0)
        //        {
        //            //need to make this a non local call - either api - or get the ID from the userInfo
        //            Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

        //        }
        //    }

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/planogram/get/inprogress/" + countryId + "/" + regionId + "/" + standTypeId + "/" + isDiamUser + "";

        //    //maybe log something here

        //    using (SecureHttpClient<IEnumerable<PlanogramInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanogramInfo>>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}

        //private async Task<IActionResult> GetJobNumbersCall()
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/jobs/get/" + brandId;

        //    //maybe log something here

        //    using (SecureHttpClient<IEnumerable<JobViewModel>> httpClient = new SecureHttpClient<IEnumerable<JobViewModel>>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}

        //private async Task<IActionResult> GetJobFoldersCall(int countryId = 0, int regionId = 0, int standTypeId = 0)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];
        //    //Confirm the authorization so we can call the api 
        //    //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
        //    //var accessToken = currentAuth.AccessToken;

        //    var isPowerUser = false;
        //    var isDiamUser = false;

        //    if (RolesHelper.IsAdministrator(UserInfo.Roles))
        //    {
        //        isDiamUser = true;
        //        isPowerUser = true;
        //    }
        //    else if (RolesHelper.IsValidator(UserInfo.Roles))
        //    {
        //        isPowerUser = true;
        //        if (countryId == 0)
        //        {
        //            Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

        //        }
        //    }
        //    else if (RolesHelper.IsApprover(UserInfo.Roles))
        //    {
        //        isPowerUser = true;
        //    }
        //    else
        //    {
        //        if (countryId == 0)
        //        {
        //            //need to make this a non local call - either api - or get the ID from the userInfo
        //            Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

        //        }
        //    }

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/jobFolders/get/" + brandId + "/" + countryId + "/" + regionId + "/" + standTypeId + "/" + isDiamUser;

        //    //maybe log something here

        //    using (SecureHttpClient<IEnumerable<JobFolderViewModel>> httpClient = new SecureHttpClient<IEnumerable<JobFolderViewModel>>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}

        //private async Task<IActionResult> GetJobNumbersForFoldersCall(int jobFolderId)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];
        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });


        //    var uri = "api/v2/jobNumbersForFolder/get/" + jobFolderId ;
        //    var url = string.Format("{0}{1}", domain, uri);
        //    //maybe log something here

        //    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
        //    {
        //        //request.Content = content;
        //        using (HttpClient httpClient = new HttpClient())
        //        {
        //            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //            var response = await proxyApi.httpClient.SendAsync(request);

        //            var StatusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
        //            var responseBodyAsText = await proxyApi.response.Content.ReadAsStringAsync();
        //            responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
        //            _logger.LogDebug("response from GetJobNumbersForFolder = " + responseBodyAsText + " :: " + StatusText);

        //            return Ok(response);
        //        }
        //    }

        //}



        //private async Task<IActionResult> GetTemplatesCall(int standId)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brand = ConfigurationManager.AppSettings["brand"];

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/planogram/template/get/" + brand + "/" + standId;

        //    //maybe log something here

        //    using (SecureHttpClient<IEnumerable<PlanogramClusterModel>> httpClient = new SecureHttpClient<IEnumerable<PlanogramClusterModel>>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}


        //private async Task<IActionResult> ClonePlanogramCall(int planogramId, string planoName)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brand = ConfigurationManager.AppSettings["brand"];

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/planogram/clone/" + planogramId + "/" + planoName;

        //    //maybe log something here

        //    using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}

        //private async Task<IActionResult> CreatePlanogramCall(int clusterId, string planoName)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brand = ConfigurationManager.AppSettings["brand"];

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/planogram/create/" + clusterId + "/" + planoName;

        //    //maybe log something here

        //    using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}

        //private async Task<IActionResult> GetOpenOrderCall(int planogramId)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];

        //    var uri = domain + "api/v2/order/getOpen/" + brandId + "/" + planogramId ;

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/order/getOpen/" + brandId + "/" + planogramId;

        //    //maybe log something here

        //    using (SecureHttpClient<Order> httpClient = new SecureHttpClient<Order>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}

        //private async Task<IActionResult> GetOpenOrdersCall(int planogramId)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    string brandId = ConfigurationManager.AppSettings["brand"];

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/order/getOpenOrders/" + brandId + "/" + planogramId;

        //    //maybe log something here

        //    using (SecureHttpClient<IEnumerable<Order>> httpClient = new SecureHttpClient<IEnumerable<Order>>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}




        //private async Task<IActionResult> AddToOrderCall(int orderId, int planogramId, int quantity, bool isFullPlano)
        //{

        //    string domain = ConfigurationManager.AppSettings["apiUrl"];
        //    //Confirm the authorization so we can call the api 
        //    //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
        //    //var accessToken = currentAuth.AccessToken;

        //    var userId = UserInfo.Id;;

        //    var accessToken = await proxyApi.GetAccessToken(new string[] { Globals.ReadTasksScope });

        //    var uriSuffix = "api/v2/order/addToOrder/" + orderId + "/" + planogramId + "/" + quantity + "/" + userId + "/" + isFullPlano;

        //    //maybe log something here

        //    using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix))
        //    {
        //        try
        //        {
        //            var result = await proxyApi.httpClient.Get(accessToken);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //}




        //#endregion

        #region helper functions
        #endregion
    }
}
