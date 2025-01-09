using System.Net.Http.Headers;
using CoreSystem2024.Helpers;
using CoreSystem2024.HttpClientWrapper;
using CoreSystem2024.Models;
using dplo.Domain;
using dplo.Domain.Entities;
using dplo.Service;
using Dplo.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Security;

namespace CoreSystem2024.ProxyServices
{
    public interface IYourPlanogramProxyApiService
    {
        Task<string> LockPlanogramCall(int planogramId);
        Task<int> RenamePlanogramCall(PlanogramUpdate data);
        Task<int> GetCommentsCountCall(int planogramId);
        Task<string> CreateSkuList(int planogramId);
        Task<IEnumerable<ExportSkuModel>> CreateJsonSkuList(int planogramId);
        Task<string> CreateCassetteList(int planogramId);
        Task<int> SubmitPlanogramCall(int planogramId);
        Task<int> ApprovePlanogramCall(int planogramId);
        Task<int> RejectPlanogramCall(int planogramId);
        Task<string> ArchivePlanogramCall(int planogramId, string jobNumber);

        Task<IEnumerable<PlanogramInfo>> GetArchivedPlanogramsByJobCodeCall(int jobCode, int countryId = 0,
            int regionId = 0, int standTypeId = 0);

        Task<int> GetStandTypesCall(int brandId);
        Task<int> GetRegionsCall(int brandId);
        Task<int> GetCountriesByRegionCall(int regionId);

        Task<IEnumerable<PlanogramInfo>> GetPlanogramsCall(PlanaogramStatusEnum status, int countryId = 0,
            int regionId = 0, int standTypeId = 0);

        Task<IEnumerable<JobViewModel>> GetJobNumbersCall();

        Task<IEnumerable<JobFolderViewModel>> GetJobFoldersCall(int countryId = 0, int regionId = 0,
            int standTypeId = 0);

        Task<IEnumerable<JobInfo>> GetJobNumbersForFoldersCall(int jobFolderId);
        Task<IEnumerable<PlanogramClusterModel>> GetTemplatesCall(int standId);
        Task<int> ClonePlanogramCall(int planogramId, string planoName);
        Task<int> CreatePlanogramCall(int clusterId, string planoName);

        Task<Order> GetOpenOrderCall(int planogramId);
        Task<IEnumerable<Order>> GetOpenOrdersCall(int planogramId);
        Task<string> AddToOrderCall(int orderId, int planogramId, int quantity, bool isFullPlano);
    }
    public class YourPlanogramProxyApiService : IYourPlanogramProxyApiService
    {

        private readonly ILogger<YourPlanogramProxyApiService> _logger;
        private ICountryService _countryService;
        private readonly IConfiguration _configuration;
        private readonly IMemberManager _memberManager;
        private string domain;
        private string brandId;
        private string readScope;
        private string writeScope;
        public YourPlanogramProxyApiService(ILogger<YourPlanogramProxyApiService> logger, ICountryService countryService, IConfiguration configuration, IMemberManager memberManager)
        {
            _logger = logger;
            _countryService = countryService;
            _configuration = configuration;
            _memberManager = memberManager;
            domain = _configuration["AppSettings:ApiUrl"];
            brandId = _configuration["AppSettings:ClientBrandId"];
            readScope = _configuration["AzureB2C:ReadScope"];
            writeScope = _configuration["AzureB2CWriteScope"];


        }

        #region remote api calls

        public async Task<string> LockPlanogramCall(int planogramId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uriSuffix = "api/v2/planogram/lock/" + planogramId;


            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw(ex);
                }
            }

        }

        public async Task<int> RenamePlanogramCall(PlanogramUpdate data)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uriSuffix = "api/v2/planogram/rename/" + data.PlanogramId + "/" + data.PlanogramName;


            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }
        public async Task<int> GetCommentsCountCall(int planogramId)
        {


            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uriSuffix = "api/v2/planogram/getCommentCount/" + planogramId + "/" + brandId;


            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw(ex);
                }
            }


        }

        public async Task<string> CreateSkuList(int planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var isPowerUser = false;
            var isDiamUser = false;

            var uriSuffix = "api/v2/planogram/get/skulist/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }

        public async Task<IEnumerable<ExportSkuModel>> CreateJsonSkuList(int planogramId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var isPowerUser = false;
            var isDiamUser = false;

            var uriSuffix = "api/v2/planogram/get/jsonskulist/" + planogramId;
            _logger.LogDebug("making api call with url " + uriSuffix);
            //maybe log something here

            using (SecureHttpClient<IEnumerable<ExportSkuModel>> httpClient = new SecureHttpClient<IEnumerable<ExportSkuModel>>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("error calling dssapi from createjsonskulist " + ex.Message);
                    throw (ex);
                }
            }

        }

        public async Task<string> CreateCassetteList(int planogramId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/get/casslist/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }


        public async Task<int> SubmitPlanogramCall(int planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/submit/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return planogramId;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }


        public async Task<int> ApprovePlanogramCall(int planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/approve/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return planogramId;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }


        public async Task<int> RejectPlanogramCall(int planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/reject/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return planogramId;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }


        public async Task<string> ArchivePlanogramCall(int planogramId, string jobNumber)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/archive/" + planogramId + "/" + jobNumber;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }

        public async Task<IEnumerable<PlanogramInfo>> GetArchivedPlanogramsByJobCodeCall(int jobCode, int countryId = 0, int regionId = 0, int standTypeId = 0)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);


            var isPowerUser = false;
            var isDiamUser = false;

            if (RolesHelper.IsAdministrator(userInfo.Roles))
            {
                isDiamUser = true;
                isPowerUser = true;
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, countryFilter, regionFilter, standTypeFilter, true);

            }
            else if (RolesHelper.IsValidator(userInfo.Roles))
            {
                isPowerUser = true;
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
                if (countryId == 0)
                {
                    Country country = _countryService.GetCountry(userInfo.DiamCountryId);

                }
            }
            else if (RolesHelper.IsApprover(userInfo.Roles))
            {
                isPowerUser = true;

            }
            else
            {
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
                if (countryId == 0)
                {
                    //need to make this a non local call - either api - or get the ID from the userInfo
                    Country country = _countryService.GetCountry(userInfo.DiamCountryId);

                }
            }


            var uri = "api/v2/planogram/get/archived/jobcode/" + isPowerUser + "/" + jobCode + "/" + brandId + "/" + countryId + "/" + regionId + "/" + standTypeId + "/" + isDiamUser.ToString();

                        var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var url = string.Format("{0}{1}", domain, uri);


            using (SecureHttpClient<IEnumerable<PlanogramInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanogramInfo>>(domain, uri, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }



            //using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            //{
            //    using (HttpClient httpClient = new HttpClient())
            //    {
            //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //        var response = await httpClient.SendAsync(request);

            //        var StatusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
            //        var responseBodyAsText = await response.Content.ReadAsStringAsync();
            //        responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
            //        _logger.LogDebug("response from GetPlanogramsByJobCode = " + responseBodyAsText + " :: " + StatusText);

            //        return response;
            //    }
            //}

        }

        public async Task<int> GetStandTypesCall(int brandId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uriSuffix = "api/v2/planogram/getStandTypes/" + brandId;


            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<int> GetRegionsCall(int brandId)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uriSuffix = "api/v2/planogram/getRegions/" + brandId;


            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<int> GetCountriesByRegionCall(int regionId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uriSuffix = "api/v2/planogram/getCountries/" + regionId;


            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<IEnumerable<PlanogramInfo>> GetPlanogramsCall(PlanaogramStatusEnum status, int countryId = 0, int regionId = 0, int standTypeId = 0)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var isDiamUser = false;

            if (RolesHelper.IsAdministrator(userInfo.Roles))
            {
                isDiamUser = true;
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, countryFilter, regionFilter, standTypeFilter, true);

            }
            else if (RolesHelper.IsValidator(userInfo.Roles))
            {
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
                if (countryId == 0)
                {
                    Country country = _countryService.GetCountry(userInfo.DiamCountryId);

                }
            }
            else if (RolesHelper.IsApprover(userInfo.Roles))
            {

            }
            else
            {
                //planograms = planogramService.GetInProgressPlanograms(0, BrandId, UserCountry.CountryId, 0, standTypeFilter, false);
                if (countryId == 0)
                {
                    //need to make this a non local call - either api - or get the ID from the userInfo
                    Country country = _countryService.GetCountry(userInfo.DiamCountryId);

                }
            }

            //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/get/yourplanograms/" + (int)status + "/" + countryId + "/" + regionId + "/" + standTypeId + "/" + brandId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanogramInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanogramInfo>>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }

        public async Task<IEnumerable<JobViewModel>> GetJobNumbersCall()
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/jobs/get/" + brandId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<JobViewModel>> httpClient = new SecureHttpClient<IEnumerable<JobViewModel>>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }

        public async Task<IEnumerable<JobFolderViewModel>> GetJobFoldersCall(int countryId = 0, int regionId = 0, int standTypeId = 0)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var isPowerUser = false;
            var isDiamUser = false;

            if (RolesHelper.IsAdministrator(userInfo.Roles))
            {
                isDiamUser = true;
                isPowerUser = true;
            }
            else if (RolesHelper.IsValidator(userInfo.Roles))
            {
                isPowerUser = true;
                if (countryId == 0)
                {
                    Country country = _countryService.GetCountry(userInfo.DiamCountryId);

                }
            }
            else if (RolesHelper.IsApprover(userInfo.Roles))
            {
                isPowerUser = true;
            }
            else
            {
                if (countryId == 0)
                {
                    //need to make this a non local call - either api - or get the ID from the userInfo
                    Country country = _countryService.GetCountry(userInfo.DiamCountryId);

                }
            }

                        var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            //var uriSuffix = "api/v2/jobFolders/get/" + brandId + "/" + countryId + "/" + regionId + "/" + standTypeId + "/" + isDiamUser;
            var uriSuffix = "api/v2/jobFolders/get/" + brandId + "/" + countryId + "/" + regionId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<JobFolderViewModel>> httpClient = new SecureHttpClient<IEnumerable<JobFolderViewModel>>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }

        public async Task<IEnumerable<JobInfo>> GetJobNumbersForFoldersCall(int jobFolderId)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/jobNumbersForFolder/get/" + jobFolderId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (SecureHttpClient<IEnumerable<JobInfo>> httpClient = new SecureHttpClient<IEnumerable<JobInfo>>(domain, url, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    _logger.LogDebug("number of numbers from GetJobNumbersForFolder = " + result.Count());
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }



        public async Task<IEnumerable<PlanogramClusterModel>> GetTemplatesCall(int standId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/template/get/" + brandId + "/" + standId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanogramClusterModel>> httpClient = new SecureHttpClient<IEnumerable<PlanogramClusterModel>>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }


        public async Task<int> ClonePlanogramCall(int planogramId, string planoName)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/clone/" + planogramId + "/" + planoName;

            //maybe log something here

            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }

        public async Task<int> CreatePlanogramCall(int clusterId, string planoName)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/create/" + clusterId + "/" + planoName;

            //maybe log something here

            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return (result);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }

        public async Task<Order> GetOpenOrderCall(int planogramId)
        {




            var uri = domain + "api/v2/order/getOpen/" + brandId + "/" + planogramId;
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/order/getOpen/" + brandId + "/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<Order> httpClient = new SecureHttpClient<Order>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }

        public async Task<IEnumerable<Order>> GetOpenOrdersCall(int planogramId)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);


            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/order/getOpenOrders/" + brandId + "/" + planogramId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<Order>> httpClient = new SecureHttpClient<IEnumerable<Order>>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }




        public async Task<string> AddToOrderCall(int orderId, int planogramId, int quantity, bool isFullPlano)
        {


            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var userId = userInfo.Id; ;

                        var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/order/addToOrder/" + orderId + "/" + planogramId + "/" + quantity + "/" + userId + "/" + isFullPlano;

            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }




        #endregion
    }
}
