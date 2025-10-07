using CoreSystem2024.Controllers;
using CoreSystem2024.Helpers;
using CoreSystem2024.HttpClientWrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
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
        public CreatePlanogramProxyApiService(ILogger<YourPlanogramApiController> logger, IConfiguration configuration, IMemberManager memberManager)
        {
            _logger = logger;
            _configuration = configuration;
            _memberManager = memberManager;
        }
        #endregion


        #region remote api calls

        public async Task<IEnumerable<SelectListItem>> GetStandsWithClusters(int standTypeId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/stand/getBrandedWithClusters/" + brand + "/" + userInfo.DiamCountryId + "/" + standTypeId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<SelectListItem>> httpClient = new SecureHttpClient<IEnumerable<SelectListItem>>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }

        public async Task<IEnumerable<PlanmClusterDto>> GetClustersCall(int standId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];


            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/clusters/get/" + brand + "/" + standId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanmClusterDto>> httpClient = new SecureHttpClient<IEnumerable<PlanmClusterDto>>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }

        public async Task<IEnumerable<PlanmClusterDto>> GetTemplatesCall(int standId)
        {
            //var getPartURL = $("#apiURL").val() + "api/planogram/template/get/" + $('#brandId').val() + "/" + standId + "?token=" + _authCode + "&callback=?";

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uriSuffix = "api/v2/planogram/template/get/" + brand + "/" + standId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanmClusterDto>> httpClient = new SecureHttpClient<IEnumerable<PlanmClusterDto>>(domain, uriSuffix, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }


        public async Task<int> ClonePlanogramCall(int planogramId, string planoName)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];


            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);

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
                    throw;
                }
            }

        }

        public async Task<int> CreatePlanogramCall(int clusterId, string planoName)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];
            //we need to re-auth using the reauth process

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(ClaimsPrincipal.Current);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var url = domain + "api/v2/planogram/create/" + clusterId + "/" + planoName + "/" + brand;

            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, url, _configuration))
            {
                try
                {
                    var result = await httpClient.Get(accessToken);
                    return result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }

        #endregion

    }
}
