using System.Net.Http.Headers;
using CoreSystem.Helpers;
using CoreSystem.HttpClientWrapper;
using dplo.Service.MSGraphUtils;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using CoreSystem.Controllers;
using Microsoft.Extensions.Logging;

namespace CoreSystemII.Controllers.Proxy
{
    internal class CreatePlanogramProxyController : Controller
    {

        #region Services, managers

        private ILogger<YourPlanogramApiController> _logger;


        public CreatePlanogramProxyController(ILogger<YourPlanogramApiController> logger)
        {
            _logger = logger;
        }
        #endregion


        #region remote api calls

        public async Task<IActionResult> GetStandsWithClusters(int standTypeId)
        {

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

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

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

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
            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

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
