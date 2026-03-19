using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PMApplication.Dtos;
using PMApplication.Dtos.PlanModels;
using PMApplication.Entities.CountriesAggregate;
using PMApplication.Entities.JobsAggregate;
using PMApplication.Entities.OrderAggregate;
using PMApplication.Entities.PlanogramAggregate;
using PMApplication.Interfaces.ServiceInterfaces;
using PMApplication.Helpers;
using Umbraco.Cms.Core.Security;

namespace CoreSystem2024.ProxyServices
{
    public interface IYourPlanogramProxyApiService
    {
        Task<string> LockPlanogramCall(long planogramId);
        Task<long> RenamePlanogramCall(PlanogramUpdate data);
        Task<int> GetCommentsCountCall(long planogramId);
        Task<string> CreateSkuList(long planogramId);
        Task<IEnumerable<ExportSkuDto>> CreateJsonSkuList(long planogramId);
        Task<string> CreateCassetteList(long planogramId);
        Task<long> SubmitPlanogramCall(long planogramId);
        Task<long> ApprovePlanogramCall(long planogramId);
        Task<long> RejectPlanogramCall(long planogramId);
        Task<long> DeletePlanogramCall(long planogramId);
        Task<long> ValidatePlanogramCall(long planogramId);
        Task<string> ArchivePlanogramCall(long planogramId, string jobNumber, int jobId);

        Task<IEnumerable<PlanogramInfo>> GetArchivedPlanogramsByJobCall(int jobId, string jobCode, int countryId = 0,
            int regionId = 0, int standTypeId = 0);

        Task<int> GetStandTypesCall(int brandId);
        Task<int> GetRegionsCall(int brandId);
        Task<int> GetCountriesByRegionCall(int regionId);

        Task<IEnumerable<PlanogramInfo>> GetPlanogramsCall(int status, int countryId = 0,
            int regionId = 0, int standTypeId = 0);

        Task<IEnumerable<JobDto>> GetJobNumbersCall();

        Task<IEnumerable<JobFolderInfo>> GetJobFoldersCall(int countryId = 0, int regionId = 0,
            int standTypeId = 0);

        Task<IEnumerable<JobInfo>> GetJobNumbersForFoldersCall(int jobFolderId);
        Task<IEnumerable<PlanmPlanoClusterDto>> GetTemplatesCall(int standId);
        Task<long> ClonePlanogramCall(long planogramId, string planoName);
        Task<long> CreatePlanogramCall(long clusterId, string planoName);

        Task<Order> GetOpenOrderCall(long planogramId);
        Task<IEnumerable<Order>> GetOpenOrdersCall(long planogramId);
        Task<string> AddToOrderCall(long orderId, long planogramId, int quantity, bool isFullPlano);
    }
    public class YourPlanogramProxyApiService : IYourPlanogramProxyApiService
    {

        private readonly ILogger<YourPlanogramProxyApiService> _logger;
        private readonly ICountryService _countryService;
        private readonly IRegionService _regionService;
        private readonly IConfiguration _configuration;
        private readonly IMemberManager _memberManager;
        private string domain;
        private string brandId;
        private string readScope;
        private string writeScope;
        private readonly IHttpClientFactory _httpClientFactory;
        public YourPlanogramProxyApiService(ILogger<YourPlanogramProxyApiService> logger, ICountryService countryService, IConfiguration configuration, IMemberManager memberManager, IRegionService regionService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _countryService = countryService;
            _configuration = configuration;
            _memberManager = memberManager;
            _regionService = regionService;
            _httpClientFactory = httpClientFactory;
            domain = _configuration["AppSettings:ApiUrl"];
            brandId = _configuration["AppSettings:ClientBrandId"];
            readScope = _configuration["AzureB2C:ReadScope"];
            writeScope = _configuration["AzureB2CWriteScope"];


        }

        #region remote api calls

        public async Task<string> LockPlanogramCall(long planogramId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var url = "api/v2/planogram/lock/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<string>(url);
            return response;

        }

        public async Task<long> RenamePlanogramCall(PlanogramUpdate data)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var url = "api/v2/planogram/rename/" + data.PlanogramId + "/" + data.PlanogramName;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<int>(url);
            return response;

        }
        public async Task<int> GetCommentsCountCall(long planogramId)
        {


            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var url = "api/v2/planogram/getCommentCount/" + planogramId + "/" + brandId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<int>(url);
            return response;

        }

        public async Task<string> CreateSkuList(long planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var isPowerUser = false;
            var isDiamUser = false;

            var url = "api/v2/planogram/get/skulist/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<string>(url);
            return response;

        }

        public async Task<IEnumerable<ExportSkuDto>> CreateJsonSkuList(long planogramId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var isPowerUser = false;
            var isDiamUser = false;

            var url = "api/v2/planogram/get/jsonskulist/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<IEnumerable<ExportSkuDto>>(url);
            return response;
            _logger.LogDebug("making api call with url " + url);
        }

        public async Task<string> CreateCassetteList(long planogramId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/get/casslist/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<string>(url);
            return response;
        }


        public async Task<long> SubmitPlanogramCall(long planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/submit/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<long>(url);
            return planogramId;

        }


        public async Task<long> ApprovePlanogramCall(long planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/approve/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<long>(url);
            return planogramId;
        }


        public async Task<long> RejectPlanogramCall(long planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/reject/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<long>(url);
            return planogramId;
        }


        public async Task<long> ValidatePlanogramCall(long planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/validate/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<long>(url);
            return planogramId;

        }


        public async Task<long> DeletePlanogramCall(long planogramId)
        {


            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

                //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

                var url = "api/v2/planogram/delete/" + planogramId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetFromJsonAsync<long>(url);
                return planogramId;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting planogram with id " + planogramId + ": " + ex.Message);
                throw;

            }

        }


        public async Task<long> AddToOrderCall(long planogramId)
        {



            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/reject/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<string>(url);
            return planogramId;

        }


        public async Task<string> ArchivePlanogramCall(long planogramId, string jobNumber, int jobId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/archive/" + planogramId + "/" + jobNumber + "/" + jobId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<string>(url);
            return response;

        }

        public async Task<IEnumerable<PlanogramInfo>> GetArchivedPlanogramsByJobCall(int jobId, string jobCode, int countryId = 0, int regionId = 0, int standTypeId = 0)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            if (memberIdentity != null)
            {
                //var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(memberIdentity);
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
                        Country country = await _countryService.GetCountry(userInfo.DiamCountryId);

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
                        Country country = await _countryService.GetCountry(userInfo.DiamCountryId);

                    }

                    if (jobCode == string.Empty)
                    {
                        jobCode = "0";
                    }
                }


                var url = "api/v2/planogram/get/archived/job/" + (isPowerUser ? 1 : 0) + "/" + jobId + "/" + jobCode +
                          "/" + brandId + "/" + countryId + "/" + regionId + "/" + standTypeId + "/" +
                          (isDiamUser ? 1 : 0);
                //var uri = "api/v2/planogram/get/archived/job/" + jobId + "/" + jobCode + "/" + brandId + "/" + countryId + "/" + regionId + "/" + standTypeId;

                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanogramInfo>>(url);
                return response;
            }
            else
            {
                throw new Exception("User not authenticated");
            }

        }

        public async Task<int> GetStandTypesCall(int brandId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var url = "api/v2/planogram/getStandTypes/" + brandId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<int>(url);
            return response;

        }

        public async Task<int> GetRegionsCall(int brandId)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var url = "api/v2/planogram/getRegions/" + brandId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<int>(url);
            return response;

        }

        public async Task<int> GetCountriesByRegionCall(int regionId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var url = "api/v2/planogram/getCountries/" + regionId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<int>(url);
            return response;

        }

        public async Task<IEnumerable<PlanogramInfo>> GetPlanogramsCall(int status, int countryId = 0, int regionId = 0, int standTypeId = 0)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            if (memberIdentity != null)
            {
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
                        Country country = await _countryService.GetCountry(userInfo.DiamCountryId);

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
                        Country country = await _countryService.GetCountry(userInfo.DiamCountryId);

                    }
                }

                //            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

                var url = "api/v2/planogram/get/yourplanograms/" + (int)status + "/" + countryId + "/" +
                                regionId + "/" + standTypeId + "/" + brandId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");


                try
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanogramInfo>>(url);
                    return response;
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    throw;
                }
            }
            else
            {
                throw new Exception("User not authenticated");
            }

        }

        public async Task<IEnumerable<JobDto>> GetJobNumbersCall()
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/jobs/get/" + brandId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<IEnumerable<JobDto>>(url);
            return response;

        }

        public async Task<IEnumerable<JobFolderInfo>> GetJobFoldersCall(int countryId = 0, int regionId = 0, int standTypeId = 0)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                if (memberIdentity != null)
                {
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
                        Country country = await _countryService.GetCountry(userInfo.DiamCountryId);
                        var countryList = new List<Country>();
                        countryList.Add(country);
                        if (RolesHelper.IsSuperUser(userInfo.Roles))
                        {

                            var regions =
                                await _regionService.GetRegionsForCountryList(int.Parse(brandId), countryList);
                            regionId = regions.FirstOrDefault().Id;
                            countryId = 0;
                        }
                        else
                        {
                            countryId = userInfo.DiamCountryId;
                            regionId = 0;
                        }
                    }
                    else if (RolesHelper.IsApprover(userInfo.Roles))
                    {
                        isPowerUser = true;
                        countryId = userInfo.DiamCountryId;
                        regionId = 0;
                    }
                    else
                    {
                        if (countryId == 0)
                        {
                            //need to make this a non local call - either api - or get the ID from the userInfo
                            //Country country = _countryService.GetCountry(userInfo.DiamCountryId);
                            countryId = userInfo.DiamCountryId;
                            regionId = 0;
                        }
                    }

                    var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

                    //var uriSuffix = "api/v2/jobFolders/get/" + brandId + "/" + countryId + "/" + regionId + "/" + standTypeId + "/" + isDiamUser;
                    var url = "api/v2/jobFolders/get/" + brandId + "/" + countryId + "/" + regionId;
                    var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.GetFromJsonAsync<IEnumerable<JobFolderInfo>>(url);
                    return response;
                }
                else
                {
                    throw new Exception("User not authenticated");
                }
            }

            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }



        }

        public async Task<IEnumerable<JobInfo>> GetJobNumbersForFoldersCall(int jobFolderId)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var url = "api/v2/jobNumbersForFolder/get/" + jobFolderId;
            
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<IEnumerable<JobInfo>>(url);
            return response;
        }



        public async Task<IEnumerable<PlanmPlanoClusterDto>> GetTemplatesCall(int standId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/template/get/" + brandId + "/" + standId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanmPlanoClusterDto>>(url);
            return response;

        }


        public async Task<long> ClonePlanogramCall(long planogramId, string planoName)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/clone/" + planogramId + "/" + planoName;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<int>(url);
            return response;


        }

        public async Task<long> CreatePlanogramCall(long clusterId, string planoName)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/planogram/create/" + clusterId + "/" + planoName;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<int>(url);
            return response;

        }

        public async Task<Order> GetOpenOrderCall(long planogramId)
        {




            var uri = domain + "api/v2/order/getOpen/" + brandId + "/" + planogramId;
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/order/getOpen/" + brandId + "/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<Order>(url);
            return response;

        }

        public async Task<IEnumerable<Order>> GetOpenOrdersCall(long planogramId)
        {

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);


            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = "api/v2/order/getOpenOrders/" + brandId + "/" + planogramId;
            var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetFromJsonAsync<IEnumerable<Order>>(url);
            return response;

        }




        public async Task<string> AddToOrderCall(long orderId, long planogramId, int quantity, bool isFullPlano)
        {


            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            if (memberIdentity != null)
            {
                var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);

                var userId = userInfo.Id;
                ;

                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

                var url = "api/v2/order/addToOrder/" + orderId + "/" + planogramId + "/" + quantity + "/" +
                                userId + "/" + isFullPlano + "/" + brandId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetFromJsonAsync<string>(url);
                return response;
            }
            else
            {
                throw new Exception("User not authenticated");
            }

        }




        #endregion
    }
}
