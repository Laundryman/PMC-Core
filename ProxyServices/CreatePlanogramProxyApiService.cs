using CoreSystem2024.Controllers;
using CoreSystem2024.Helpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.Graph.Models;
using PMApplication.Dtos.PlanModels;
using Umbraco.Cms.Core.Security;

namespace CoreSystem2024.ProxyServices
{
    public interface ICreatePlanogramProxyService
    {
        Task<IEnumerable<SelectListItem>> GetStandsWithClusters(int standTypeId);


        Task<IEnumerable<PlanmClusterDto>> GetClustersCall(int standId);

        Task<IEnumerable<PlanmClusterDto>> GetTemplatesCall(int standId);

        Task<int> ClonePlanogramCall(int planogramId, string planoName);
        Task<int> CreatePlanogramCall(int clusterId, string planoName);
    }
    public class CreatePlanogramProxyApiService : ICreatePlanogramProxyService
    {

        #region Services, managers

        private ILogger<YourPlanogramApiController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemberManager _memberManager;
        private readonly IHttpClientFactory _httpClientFactory;
        public CreatePlanogramProxyApiService(ILogger<YourPlanogramApiController> logger, IConfiguration configuration, IMemberManager memberManager, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _memberManager = memberManager;
            _httpClientFactory = httpClientFactory;
        }
        #endregion


        #region remote api calls

        public async Task<IEnumerable<SelectListItem>> GetStandsWithClusters(int standTypeId)
        {
            try
            {
                string brand = _configuration["AppSettings:ClientBrandId"];

                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var userInfo = AuthHelper.GetUserInfo(memberIdentity);

                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

                var uriSuffix = "api/v2/stand/getBrandedWithClusters/" + brand + "/" + userInfo.DiamCountryId + "/" +
                                standTypeId;

                //maybe log something here
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<SelectListItem>>(uriSuffix);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStandsWithClusters");
                throw;
            }


        }

        public async Task<IEnumerable<PlanmClusterDto>> GetClustersCall(int standId)
        {
            try
            {
                string brand = _configuration["AppSettings:ClientBrandId"];
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var userInfo = AuthHelper.GetUserInfo(memberIdentity);

                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

                var uriSuffix = "api/v2/clusters/get/" + brand + "/" + standId;

                //maybe log something here
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanmClusterDto>>(uriSuffix);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetClustersCall");
                throw;
            }

        }

        public async Task<IEnumerable<PlanmClusterDto>> GetTemplatesCall(int standId)
        {
            //var getPartURL = $("#apiURL").val() + "api/planogram/template/get/" + $('#brandId').val() + "/" + standId + "?token=" + _authCode + "&callback=?";
            try
            {
                string brand = _configuration["AppSettings:ClientBrandId"];
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();

                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

                var uriSuffix = "api/v2/planogram/template/get/" + brand + "/" + standId;

                //maybe log something here
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanmClusterDto>>(uriSuffix);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTemplatesCall");
                throw;

            }
        }


        public async Task<int> ClonePlanogramCall(int planogramId, string planoName)
        {

            try
            {

                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

                var uriSuffix = "api/v2/planogram/clone/" + planogramId + "/" + planoName;

                //maybe log something here
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetFromJsonAsync<int>(uriSuffix);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ClonePlanogramCall");
                throw;
            }

        }


        public async Task<int> CreatePlanogramCall(int clusterId, string planoName)
        {
            try
            {

                //string domain = _configuration["AppSettings:ApiUrl"];
                string brand = _configuration["AppSettings:ClientBrandId"];
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();

                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

                var url = "api/v2/planogram/create/" + clusterId + "/" + planoName + "/" + brand;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetFromJsonAsync<int>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreatePlanogramCall");
                throw;
            }
        }

        #endregion

    }
}
