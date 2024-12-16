using CoreSystem2024.Controllers;
using CoreSystem2024.Helpers;
using CoreSystem2024.HttpClientWrapper;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace CoreSystemII.Controllers.Proxy
{
    internal class CreatePlanogramProxyController : Controller
    {

        #region Services, managers

        private ILogger<YourPlanogramApiController> _logger;
        private readonly IConfiguration Configuration;

        public CreatePlanogramProxyController(ILogger<YourPlanogramApiController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }
        #endregion


        #region remote api calls

        public async Task<IActionResult> GetStandsWithClusters(int standTypeId)
        {

            string domain = Configuration["AppSettings:ApiUrl"];
            string brand = Configuration["AppSettings:ClientBrandId"];
            string readScope = Configuration["AzureB2C:ReadScope"];
            string writeScope = Configuration["AzureB2CWriteScope"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { readScope });

            var uriSuffix = "api/v2/stand/getBrandedWithClusters/" + brand + "/" + UserInfo.DiamCountryId + "/" + standTypeId;

            //maybe log something here

            using (SecureHttpClient<IEnumerable<SelectListItem>> httpClient = new SecureHttpClient<IEnumerable<SelectListItem>>(domain, uriSuffix))
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

        public async Task<IActionResult> GetClustersCall(int standId)
        {

            string domain = Configuration["AppSettings:ApiUrl"];
            string brand = Configuration["AppSettings:ClientBrandId"];
            string readScope = Configuration["AzureB2C:ReadScope"];
            string writeScope = Configuration["AzureB2CWriteScope"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { readScope });

            var uriSuffix = "api/v2/clusters/get/" + brand + "/" + standId;

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

        public async Task<IActionResult> GetTemplatesCall(int standId)
        {
            //var getPartURL = $("#apiURL").val() + "api/planogram/template/get/" + $('#brandId').val() + "/" + standId + "?token=" + _authCode + "&callback=?";

            string domain = Configuration["AppSettings:ApiUrl"];
            string brand = Configuration["AppSettings:ClientBrandId"];
            string readScope = Configuration["AzureB2C:ReadScope"];
            string writeScope = Configuration["AzureB2CWriteScope"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { readScope });

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

            string domain = Configuration["AppSettings:ApiUrl"];
            string brand = Configuration["AppSettings:ClientBrandId"];
            string readScope = Configuration["AzureB2C:ReadScope"];
            string writeScope = Configuration["AzureB2CWriteScope"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { readScope });

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

            string domain = Configuration["AppSettings:ApiUrl"];
            string brand = Configuration["AppSettings:ClientBrandId"];
            string readScope = Configuration["AzureB2C:ReadScope"];
            string writeScope = Configuration["AzureB2CWriteScope"];
            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { readScope });

            var url = domain + "api/v2/planogram/create/" + clusterId + "/" + planoName + "/" + brand;

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
                    _logger.LogDebug("response from save = " + responseBodyAsText + " :: " + StatusText);

                    return Ok(response);
                }
            }

        }

        #endregion

    }
}
