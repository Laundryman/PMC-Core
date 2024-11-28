using CoreSystem.Helpers;
using CoreSystem.HttpClientWrapper;
using dplo.Domain.Entities;
using dplo.Domain;
using dplo.Service.MSGraphUtils;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Configuration;
using UserInfo = CoreSystem.Helpers.UserInfo;
using CoreSystem.Controllers;
using Microsoft.Extensions.Logging;
using dplo.Service;

namespace CoreSystemII.Controllers.Proxy
{
    internal class YourPlanogramProxyController : Controller
    {

        private readonly ILogger<YourPlanogramProxyController> _logger;
        private ICountryService _countryService;
        public YourPlanogramProxyController(ILogger<YourPlanogramProxyController> logger, ICountryService countryService)
        {
            _logger = logger;
            _countryService = countryService;
        }

        #region remote api calls

        public async Task<IActionResult> LockPlanogramCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uriSuffix = "api/v2/planogram/lock/" + planogramId;


            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }
        public async Task<IActionResult> GetCommentsCountCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];
            //Confirm the authorization so we can call the api 
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uriSuffix = "api/v2/planogram/getCommentCount/" + planogramId + "/" + brandId;


            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }


        }

        public async Task<IActionResult> CreateSkuList(int planogramId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];
            //Confirm the authorization so we can call the api 
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var isPowerUser = false;
            var isDiamUser = false;

            var uriSuffix = "api/v2/planogram/get/skulist/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        public async Task<IActionResult> CreateJsonSkuList(int planogramId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];
            //Confirm the authorization so we can call the api 
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var isPowerUser = false;
            var isDiamUser = false;

            var uriSuffix = "api/v2/planogram/get/jsonskulist/" + planogramId;
            _logger.LogDebug("making api call with url " + uriSuffix);
            //maybe log something here

            using (SecureHttpClient<IEnumerable<SkuList>> httpClient = new SecureHttpClient<IEnumerable<SkuList>>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("error calling dssapi from createjsonskulist " + ex.Message);
                    return BadRequest(ex.Message);
                }
            }

        }

        public async Task<IActionResult> CreateCassetteList(int planogramId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/planogram/get/casslist/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }


        }


        public async Task<IActionResult> ArchivePlanogramCall(int planogramId, string jobNumber)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/planogram/archive/" + planogramId + "/" + jobNumber;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        public async Task<IActionResult> GetArchivedPlanogramsByJobCodeCall(string jobCode, int countryId = 0, int regionId = 0, int standTypeId = 0)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];

            var isPowerUser = false;
            var isDiamUser = false;

            if (RolesHelper.IsAdministrator(UserInfo.Roles))
            {
                isDiamUser = true;
                isPowerUser = true;
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, countryFilter, regionFilter, standTypeFilter, true);

            }
            else if (RolesHelper.IsValidator(UserInfo.Roles))
            {
                isPowerUser = true;
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
                if (countryId == 0)
                {
                    Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

                }
            }
            else if (RolesHelper.IsApprover(UserInfo.Roles))
            {
                isPowerUser = true;

            }
            else
            {
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
                if (countryId == 0)
                {
                    //need to make this a non local call - either api - or get the ID from the userInfo
                    Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

                }
            }


            var uri = "api/v2/planogram/get/archived/jobcode/" + isPowerUser + "/" + jobCode + "/" + brandId + "/" + countryId + "/" + regionId + "/" + standTypeId + "/" + isDiamUser.ToString();

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);

                    var StatusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                    responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                    _logger.LogDebug("response from GetPlanogramsByJobCode = " + responseBodyAsText + " :: " + StatusText);

                    return Ok(response);
                }
            }

        }

        public async Task<IActionResult> GetInProgress(int countryId = 0, int regionId = 0, int standTypeId = 0)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];
            //Confirm the authorization so we can call the api 
            //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
            //var accessToken = currentAuth.AccessToken;
            //var country = CountryService.GetCountry(UserInfo.DiamCountryId);

            var isDiamUser = false;

            if (RolesHelper.IsAdministrator(UserInfo.Roles))
            {
                isDiamUser = true;
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, countryFilter, regionFilter, standTypeFilter, true);

            }
            else if (RolesHelper.IsValidator(UserInfo.Roles))
            {
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
                if (countryId == 0)
                {
                    Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

                }
            }
            else if (RolesHelper.IsApprover(UserInfo.Roles))
            {

            }
            else
            {
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
                if (countryId == 0)
                {
                    //need to make this a non local call - either api - or get the ID from the userInfo
                    Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

                }
            }

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/planogram/get/inprogress/" + countryId + "/" + regionId + "/" + standTypeId + "/" + isDiamUser + "";

            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanogramInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanogramInfo>>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        public async Task<IActionResult> GetJobNumbersCall()
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/jobs/get/" + brandId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<JobViewModel>> httpClient = new SecureHttpClient<IEnumerable<JobViewModel>>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        public async Task<IActionResult> GetJobFoldersCall(int countryId = 0, int regionId = 0, int standTypeId = 0)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];
            //Confirm the authorization so we can call the api 
            //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
            //var accessToken = currentAuth.AccessToken;

            var isPowerUser = false;
            var isDiamUser = false;

            if (RolesHelper.IsAdministrator(UserInfo.Roles))
            {
                isDiamUser = true;
                isPowerUser = true;
            }
            else if (RolesHelper.IsValidator(UserInfo.Roles))
            {
                isPowerUser = true;
                if (countryId == 0)
                {
                    Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

                }
            }
            else if (RolesHelper.IsApprover(UserInfo.Roles))
            {
                isPowerUser = true;
            }
            else
            {
                if (countryId == 0)
                {
                    //need to make this a non local call - either api - or get the ID from the userInfo
                    Country country = _countryService.GetCountry(UserInfo.DiamCountryId);

                }
            }

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/jobFolders/get/" + brandId + "/" + countryId + "/" + regionId + "/" + standTypeId + "/" + isDiamUser;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<JobFolderViewModel>> httpClient = new SecureHttpClient<IEnumerable<JobFolderViewModel>>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        public async Task<IActionResult> GetJobNumbersForFoldersCall(int jobFolderId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/jobNumbersForFolder/get/" + jobFolderId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                //request.Content = content;
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);

                    var StatusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                    responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                    _logger.LogDebug("response from GetJobNumbersForFolder = " + responseBodyAsText + " :: " + StatusText);

                    return Ok(response);
                }
            }

        }



        public async Task<IActionResult> GetTemplatesCall(int standId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/planogram/template/get/" + brand + "/" + standId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanogramClusterModel>> httpClient = new SecureHttpClient<IEnumerable<PlanogramClusterModel>>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }


        public async Task<IActionResult> ClonePlanogramCall(int planogramId, string planoName)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/planogram/clone/" + planogramId + "/" + planoName;

            //maybe log something here

            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        public async Task<IActionResult> CreatePlanogramCall(int clusterId, string planoName)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/planogram/create/" + clusterId + "/" + planoName;

            //maybe log something here

            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        public async Task<IActionResult> GetOpenOrderCall(int planogramId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];

            var uri = domain + "api/v2/order/getOpen/" + brandId + "/" + planogramId;

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/order/getOpen/" + brandId + "/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<Order> httpClient = new SecureHttpClient<Order>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        public async Task<IActionResult> GetOpenOrdersCall(int planogramId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brandId = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/order/getOpenOrders/" + brandId + "/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<Order>> httpClient = new SecureHttpClient<IEnumerable<Order>>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }




        public async Task<IActionResult> AddToOrderCall(int orderId, int planogramId, int quantity, bool isFullPlano)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            //Confirm the authorization so we can call the api 
            //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
            //var accessToken = currentAuth.AccessToken;

            var userId = UserInfo.Id; ;

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uriSuffix = "api/v2/order/addToOrder/" + orderId + "/" + planogramId + "/" + quantity + "/" + userId + "/" + isFullPlano;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }




        #endregion
    }
}
