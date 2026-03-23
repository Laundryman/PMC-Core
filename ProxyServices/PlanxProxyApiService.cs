using CoreSystem2024.Controllers;
using CoreSystem2024.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Security;
using PMApplication.Dtos;
using Umbraco.Cms.Core;
using PMApplication.Dtos.PlanModels;
using PMApplication.Entities.PlanogramAggregate;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;
using System.Net.Http;

namespace CoreSystem2024.ProxyServices
{

    public interface IPlanxProxyApiService
    {
        Task<IEnumerable<PlanmMenuPart>> GetMenuCall(int planogramId);
        Task<IEnumerable<PlanmMenuPart>> GetCategoryMenuCall(int planogramId, int categoryId);
        Task<MenuDto> GetMenuCategoriesCall(int planogramId);
        Task<PlanmPlanogramDto> GetPlanogramCall(int planogramId);
        Task<PlanmStandDto> GetStandCall(int standId);

        Task<string> GetPlanogramPreviewCall(int planogramId);
        //Task<IActionResult> GetLatestVersionCall(int planogramId);
        Task<IEnumerable<PlanmPartInfo>> GetPlanogramShelvesCall(int planogramId);

        Task<IEnumerable<PlanmPartInfo>> GetPlanogramPartsCall(int planogramId);

        Task<IEnumerable<PlanmPartInfo>> GetNewPlanogramPartsCall(int planogramId);

        Task<IEnumerable<PlanogramPart>> GetNonMarketPartsCall(int planogramId);

        Task<IEnumerable<PlanmPartInfo>> GetScratchPadCall(int planogramId);

        Task<PartDto> GetPartCall(int partId);

        Task<PartProductsDto> GetPartProductsCall(int partId, int planogramId);


        Task<ProductShadesDto> GetProductShadesCall(int productId);

        Task<string> GetPlanoLockCall(int planogramId);

        Task<int> GetPlanoComCountCall(int planogramId);
        Task<string> UnlockCall(int planogramId);


        Task<HttpResponseMessage> SavePlanogramCallV2(PlanmPlanogramInfo planogramData);



        Task<HttpResponseMessage> SaveCassettesCall(PlanmShelfInfoList shelves);


        Task<HttpResponseMessage> SavePlanogramJpegCall(PlanmPlanoImageDto planoJpeg);
        Task<HttpResponseMessage> SavePlanogramSvgCall(PlanmPlanoImageDto planoSvg);

        Task<HttpResponseMessage> GetPlanoPDFCall(PlanmPlanoImageDto planoSvg);

        //Task<IActionResult> SaveScratchPadCall(PlanxShelfInfoList scratchpad);



    }
    public class PlanxProxyApiService : IPlanxProxyApiService
    {

        #region Services, managers

        //protected UserViewModel _userInfo => AuthHelper.GetUserInfo(memberIdentity);
        private readonly ILogger<YourPlanogramApiController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemberManager _memberManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public PlanxProxyApiService(ILogger<YourPlanogramApiController> logger, IConfiguration configuration, IMemberManager memberManager, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _memberManager = memberManager;
            _httpClientFactory = httpClientFactory;
        }
        #endregion



        #region remote api calls

        public async Task<IEnumerable<PlanmMenuPart>> GetMenuCall(int planogramId)
        {
            try
            {

                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var handler = new JwtSecurityTokenHandler();
                var url = "api/v2/planx/get-menu/" + planogramId;
                //maybe log something here
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanmMenuPart>>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetMenuCall error: " + ex.Message);
                throw ex;
            }


        }


        public async Task<IEnumerable<PlanmMenuPart>> GetCategoryMenuCall(int planogramId, int categoryId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-category-menu/" + planogramId + "/" + categoryId;
                //maybe log something here
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanmMenuPart>>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCategoryMenuCall error: " + ex.Message);
                throw ex;
            }



        }

        public async Task<MenuDto> GetMenuCategoriesCall(int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-menu-categories/" + planogramId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<MenuDto>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetMenuCategoriesCall error: " + ex.Message);
                throw ex;
            }


        }

        public async Task<PlanmPlanogramDto> GetPlanogramCall(int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-planogram/" + planogramId;

                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<PlanmPlanogramDto>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPlanogramCall error: " + ex.Message);
                throw ex;
            }

        }


        public async Task<PlanmStandDto> GetStandCall(int standId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-stand/" + standId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<PlanmStandDto>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetStandCall error: " + ex.Message);
                throw ex;
            }


        }

        public async Task<string> GetPlanogramPreviewCall(int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-planogram-preview/" + planogramId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<string>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPlanogramPreviewCall error: " + ex.Message);
                throw ex;

            }
        }


        public async Task<IEnumerable<PlanmPartInfo>> GetPlanogramShelvesCall(int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();

                var url = "api/v2/planx/get-planogram-shelves/" + planogramId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanmPartInfo>>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPlanogramShelvesCall error: " + ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<PlanmPartInfo>> GetPlanogramPartsCall(int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-planogram-parts/" + planogramId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanmPartInfo>>(url);
                return response;
                //return new List<PlanmPartInfo>();
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPlanogramPartsCall error: " + ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<PlanmPartInfo>> GetNewPlanogramPartsCall(int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-new-parts/" + planogramId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanmPartInfo>>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetNewPlanogramPartsCall error: " + ex.Message);
                throw ex;
            }


        }

        public async Task<IEnumerable<PlanogramPart>> GetNonMarketPartsCall(int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-nonmarket-parts/" + planogramId + "/" + UserInfo.DiamCountryId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanogramPart>>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetNonMarketPartsCall error: " + ex.Message);
                throw ex;
            }

        }

        public async Task<IEnumerable<PlanmPartInfo>> GetScratchPadCall(int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();

                var url = "api/v2/planx/get-planogram-scratchpad/" + planogramId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<IEnumerable<PlanmPartInfo>>(url);
                return response;
                //return new List<PlanmPartInfo>();
            }
            catch (Exception ex)
            {
                _logger.LogError("GetScratchPadCall error: " + ex.Message);
                throw ex;
            }
        }

        public async Task<PartDto> GetPartCall(int partId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-part/" + partId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<PartDto>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPartCall error: " + ex.Message);
                throw ex;
            }
        }

        public async Task<PartProductsDto> GetPartProductsCall(int partId, int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-part-products/" + partId + "/" + planogramId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<PartProductsDto>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPartProductsCall error: " + ex.Message);
                throw ex;
            }

        }


        public async Task<ProductShadesDto> GetProductShadesCall(int productId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/get-product-shades/" + productId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<ProductShadesDto>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetProductShadesCall error: " + ex.Message);
                throw ex;
            }

        }

        public async Task<String> GetPlanoLockCall(int planogramId)
        {

            try
            {
                //we need to re-auth using the reauth process
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                if (memberIdentity != null)
                {
                    var userInfo = AuthHelper.GetUserInfo(memberIdentity);
                    var url = "api/v2/planx/get-plano-lock/" + planogramId + "/" + userInfo.Id + "/" +
                              userInfo.DisplayName;
                    
                    //maybe log something here
                    var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                    Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                    var response = await httpClient.GetFromJsonAsync<string>(url);
                    return response;

                }
                else
                {
                    throw new Exception("Member identity is null");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GetPlanoLockCall error: " + ex.Message);
                throw ex;
            }
        }

        public async Task<int> GetPlanoComCountCall(int planogramId)
        {
            try
            {
                string brand = _configuration["AppSettings:ClientBrandId"];
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planogram/getCommentCount/" + planogramId + "/" + brand;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<int>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPlanoComCountCall error: " + ex.Message);
                throw ex;
            }

        }
        public async Task<string> UnlockCall(int planogramId)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planogram/unlock/" + planogramId;
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                var response = await httpClient.GetFromJsonAsync<string>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("UnlockCall error: " + ex.Message);
                throw ex;
            }

        }



        public async Task<HttpResponseMessage> SavePlanogramCallV2(PlanmPlanogramInfo planogramData)
        {
            _logger.LogDebug("Save Cassettes call start ");
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var json = JsonConvert.SerializeObject(planogramData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = "api/v2/planx/save-planogram/";
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);

                _logger.LogDebug("Save planogram make call ");


                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Content = content;
                    var response = await httpClient.SendAsync(request);

                    var StatusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                    responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                    _logger.LogDebug("response from save = " + response + " :: " + StatusText);

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SavePlanogramCallV2 error: " + ex.Message);
                throw ex;
            }


        }



        public async Task<HttpResponseMessage> SaveCassettesCall(PlanmShelfInfoList shelves)
        {
            _logger.LogDebug("Save Cassettes call start ");
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var json = JsonConvert.SerializeObject(shelves);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = "api/v2/planx/save-planogram-cassettes/";

                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                httpClient.PutAsJsonAsync<PlanmShelfInfoList>(url, shelves);
                _logger.LogDebug("Save Cassettes make call ");

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("SaveCassettesCall error: " + ex.Message);
                throw ex;
            }

        }


        public async Task<HttpResponseMessage> SavePlanogramJpegCall(PlanmPlanoImageDto planoJpeg)
        {
            _logger.LogDebug("Save planogram jpg call start ");
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var url = "api/v2/planx/save-planogram-jpeg-image/";
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                _logger.LogDebug("Save planogram svg make call ");

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    //string imageFile = Convert.ToBase64String(buffer);
                    var json = JsonConvert.SerializeObject(planoJpeg);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    request.Content = content;
                    var response = await httpClient.SendAsync(request);

                    var StatusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                    responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                    _logger.LogDebug("response from save jpg = " + responseBodyAsText + " :: " + StatusText);

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SavePlanogramJpegCall error: " + ex.Message);
                throw ex;

            }
        }

        public async Task<HttpResponseMessage> SavePlanogramSvgCall(PlanmPlanoImageDto planoSvg)
        {
            _logger.LogDebug("Save planogram svg call start ");

            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var json = JsonConvert.SerializeObject(planoSvg);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = "api/v2/planx/save-planogram-svg-image/";
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);
                await httpClient.PutAsJsonAsync<PlanmPlanoImageDto>(url, planoSvg);
                _logger.LogDebug("Save planogram svg make call ");


                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("SavePlanogramSvgCall error: " + ex.Message);
                throw ex;
            }
        }

        public async Task<HttpResponseMessage> GetPlanoPDFCall(PlanmPlanoImageDto planoSvg)
        {
            _logger.LogDebug("Get planogram pdf call start ");
            try
            {

                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var json = JsonConvert.SerializeObject(planoSvg);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = "api/v2/planx/get-planogram-pdf/";
                var httpClient = _httpClientFactory.CreateClient("PMCApiClient");
                Utilities.SetHttpClientHeaders(httpClient, memberIdentity);

                _logger.LogDebug("Get Planogram PDF make call ");

                var response = await httpClient.PostAsync(url, content);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPlanoPDFCall error: " + ex.Message);
                throw ex;
            }

        }

        #endregion
    }
}