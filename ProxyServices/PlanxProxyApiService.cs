using CoreSystem2024.Controllers;
using CoreSystem2024.Helpers;
using Dplo.ViewModels.PlanxModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Security;
using CoreSystem2024.HttpClientWrapper;
using dplo.Domain;
using dplo.Domain.Entities;
using Umbraco.Cms.Core;

namespace CoreSystem2024.ProxyServices
{

    public interface IPlanxProxyApiService
    {
        Task<IEnumerable<PlanxMenuPart>> GetMenuCall(int planogramId);
        Task<IEnumerable<PlanxMenuPart>> GetCategoryMenuCall(int planogramId, int categoryId);
        Task<PlanXMenuViewModel> GetMenuCategoriesCall(int planogramId);
        Task<PlanXPlanogramViewModel> GetPlanogramCall(int planogramId);
        Task<PlanXStandViewModel> GetStandCall(int standId);

        Task<string> GetPlanogramPreviewCall(int planogramId);
        //Task<IActionResult> GetLatestVersionCall(int planogramId);
        Task<IEnumerable<PlanxPartInfo>> GetPlanogramShelvesCall(int planogramId);

        Task<IEnumerable<PlanxPartInfo>> GetPlanogramPartsCall(int planogramId);

        Task<IEnumerable<PlanxPartInfo>> GetNewPlanogramPartsCall(int planogramId);

        Task<IEnumerable<PlanogramPart>> GetNonMarketPartsCall(int planogramId);

        Task<IEnumerable<PlanxPartInfo>> GetScratchPadCall(int planogramId);

        Task<PartViewModel> GetPartCall(int partId);

        Task<PartProductsViewModel> GetPartProductsCall(int partId, int planogramId);


        Task<ProductShadesViewModel> GetProductShadesCall(int productId);

        Task<string> GetPlanoLockCall(int planogramId);

        Task<int> GetPlanoComCountCall(int planogramId);
        Task<string> UnlockCall(int planogramId);


        Task<HttpResponseMessage> SavePlanogramCallV2(PlanxPlanogramInfo planogramData);



        Task<HttpResponseMessage> SaveCassettesCall(PlanxShelfInfoList shelves);


        Task<HttpResponseMessage> SavePlanogramJpegCall(PlanogramImageViewModel planoJpeg);
        Task<HttpResponseMessage> SavePlanogramSvgCall(PlanogramImageViewModel planoSvg);

        Task<HttpResponseMessage> GetPlanoPDFCall(PlanogramImageViewModel planoSvg);

        //Task<IActionResult> SaveScratchPadCall(PlanxShelfInfoList scratchpad);



    }
    public class PlanxProxyApiService : IPlanxProxyApiService
    {

        #region Services, managers

        //protected UserViewModel _userInfo => AuthHelper.GetUserInfo(User);
        private readonly ILogger<YourPlanogramApiController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemberManager _memberManager;

        public PlanxProxyApiService(ILogger<YourPlanogramApiController> logger, IConfiguration configuration, IMemberManager memberManager)
        {
            _logger = logger;
            _configuration = configuration;
            _memberManager = memberManager;
        }
        #endregion



        #region remote api calls

        public async Task<IEnumerable<PlanxMenuPart>> GetMenuCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(accessToken);
            //_logger.LogError("access token " + " ---- " + jwtSecurityToken.ToString());
            var uri = "api/v2/planx/get-menu/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanxMenuPart>> httpClient = new SecureHttpClient<IEnumerable<PlanxMenuPart>>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<IEnumerable<PlanxMenuPart>> GetCategoryMenuCall(int planogramId, int categoryId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uri = "api/v2/planx/get-category-menu/" + planogramId + "/" + categoryId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanxMenuPart>> httpClient = new SecureHttpClient<IEnumerable<PlanxMenuPart>>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<PlanXMenuViewModel> GetMenuCategoriesCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;



            var uri = "api/v2/planx/get-menu-categories/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here


            using (SecureHttpClient<PlanXMenuViewModel> httpClient = new SecureHttpClient<PlanXMenuViewModel>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }

        public async Task<PlanXPlanogramViewModel> GetPlanogramCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-planogram/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<PlanXPlanogramViewModel> httpClient = new SecureHttpClient<PlanXPlanogramViewModel>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }


        public async Task<PlanXStandViewModel> GetStandCall(int standId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uri = "api/v2/planx/get-stand/" + standId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<PlanXStandViewModel> httpClient = new SecureHttpClient<PlanXStandViewModel>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }
        public async Task<string> GetPlanogramPreviewCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-planogram-preview/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }
        //public async Task<IActionResult> GetLatestVersionCall(int planogramId)
        //{

        //    string domain = _configuration["AppSettings:ApiUrl"];
        //    string brand = _configuration["AppSettings:ClientBrandId"];
        //    string readScope = _configuration["AzureB2C:ReadScope"];
        //    string writeScope = _configuration["AzureB2CWriteScope"];

        //    var accessToken = await AuthHelper.GetAccessToken(new string[] { readScope });

        //    var uri = "api/v2/planx/get-latest-version/" + planogramId;
        //    var url = string.Format("{0}{1}", domain, uri);

        //    using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uri, _configuration))
        //    {

        //        try
        //        {
        //            var response = await httpClient.Get(accessToken);
        //            return response;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw (ex);
        //        }
        //    }


        //}
        public async Task<IEnumerable<PlanxPartInfo>> GetPlanogramShelvesCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-planogram-shelves/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<IEnumerable<PlanxPartInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanxPartInfo>>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<IEnumerable<PlanxPartInfo>> GetPlanogramPartsCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-planogram-parts/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<IEnumerable<PlanxPartInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanxPartInfo>>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<IEnumerable<PlanxPartInfo>> GetNewPlanogramPartsCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uri = "api/v2/planx/get-new-parts/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<IEnumerable<PlanxPartInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanxPartInfo>>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<IEnumerable<PlanogramPart>> GetNonMarketPartsCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-nonmarket-parts/" + planogramId + "/" + UserInfo.DiamCountryId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<IEnumerable<PlanogramPart>> httpClient = new SecureHttpClient<IEnumerable<PlanogramPart>>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<IEnumerable<PlanxPartInfo>> GetScratchPadCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-planogram-scratchpad/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<IEnumerable<PlanxPartInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanxPartInfo>>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<PartViewModel> GetPartCall(int partId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-part/" + partId;
            var url = string.Format("{0}{1}", domain, uri);
            using (SecureHttpClient<PartViewModel> httpClient = new SecureHttpClient<PartViewModel>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<PartProductsViewModel> GetPartProductsCall(int partId, int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-part-products/" + partId + "/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<PartProductsViewModel> httpClient = new SecureHttpClient<PartProductsViewModel>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }


        public async Task<ProductShadesViewModel> GetProductShadesCall(int productId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-product-shades/" + productId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<ProductShadesViewModel> httpClient = new SecureHttpClient<ProductShadesViewModel>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }

        public async Task<string> GetPlanoLockCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];


            try
            {
                //we need to re-auth using the reauth process
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var userInfo = AuthHelper.GetUserInfo(memberIdentity);

                var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
                var uri = "api/v2/planx/get-plano-lock/" + planogramId + "/" + userInfo.Id + "/" + userInfo.DisplayName;
                var url = string.Format("{0}{1}", domain, uri);
                //maybe log something here

                using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uri, _configuration))
                {

                    try
                    {
                        var response = await httpClient.Get(accessToken);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
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

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];


            //we need to re-auth using the reauth process
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planogram/getCommentCount/" + planogramId + "/" + brand;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<int> httpClient = new SecureHttpClient<int>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }
        public async Task<string> UnlockCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planogram/unlock/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (SecureHttpClient<string> httpClient = new SecureHttpClient<string>(domain, uri, _configuration))
            {

                try
                {
                    var response = await httpClient.Get(accessToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }


        }


        public async Task<HttpResponseMessage> SavePlanogramCallV2(PlanxPlanogramInfo planogramData)
        {
            _logger.LogDebug("Save Cassettes call start ");



            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var json = JsonConvert.SerializeObject(planogramData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/save-planogramV2/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save planogram make call ");


            //using (SecureHttpClient<PlanxPlanogramInfo> httpClient = new SecureHttpClient<PlanxPlanogramInfo>(domain, uri, _configuration))
            //{

            //    try
            //    {
            //        await httpClient.PutRequest(url, accessToken, planogramData);
            //        //_logger.LogDebug("response from save = " + response + " :: " + StatusText);

            //        return new HttpResponseMessage(HttpStatusCode.OK);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw (ex);
            //    }
            //}

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = content;
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);

                    var StatusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                    responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                    _logger.LogDebug("response from save = " + response + " :: " + StatusText);

                    return response;
                }
            }


        }



        public async Task<HttpResponseMessage> SaveCassettesCall(PlanxShelfInfoList shelves)
        {
            _logger.LogDebug("Save Cassettes call start ");



            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var json = JsonConvert.SerializeObject(shelves);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/save-planogram-cassettes/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save Cassettes make call ");


            using (SecureHttpClient<PlanxShelfInfoList> httpClient = new SecureHttpClient<PlanxShelfInfoList>(domain, uri, _configuration))
            {

                try
                {
                    await httpClient.PutRequest(url, accessToken, shelves);
                    //_logger.LogDebug("response from save = " + response + " :: " + StatusText);

                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }

        }


        public async Task<HttpResponseMessage> SavePlanogramJpegCall(PlanogramImageViewModel planoJpeg)
        {
            _logger.LogDebug("Save planogram jpg call start ");


            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uri = "api/v2/planx/save-planogram-jpeg-image/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save planogram svg make call ");

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                //string imageFile = Convert.ToBase64String(buffer);
                var json = JsonConvert.SerializeObject(planoJpeg);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                request.Content = content;
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);

                    var StatusText = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;
                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                    responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                    _logger.LogDebug("response from save jpg = " + responseBodyAsText + " :: " + StatusText);

                    return response;
                }
            }

        }
        public async Task<HttpResponseMessage> SavePlanogramSvgCall(PlanogramImageViewModel planoSvg)
        {
            _logger.LogDebug("Save planogram svg call start ");

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var json = JsonConvert.SerializeObject(planoSvg);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/save-planogram-svg-image/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save planogram svg make call ");


            using (SecureHttpClient<PlanogramImageViewModel> httpClient = new SecureHttpClient<PlanogramImageViewModel>(domain, uri, _configuration))
            {

                try
                {
                    await httpClient.PutRequest(url, accessToken, planoSvg);
                    //_logger.LogDebug("response from save = " + response + " :: " + StatusText);

                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }
        }

        public async Task<HttpResponseMessage> GetPlanoPDFCall(PlanogramImageViewModel planoSvg)
        {
            _logger.LogDebug("Get planogram pdf call start ");


            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var json = JsonConvert.SerializeObject(planoSvg);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/get-planogram-pdf/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Get Planogram PDF make call ");


            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.PostAsync(url, content);
                    return response;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GetPlanoPDFCall error: " + ex.Message);
                throw ex;
            }




        }

        //public async Task<HttpResponseMessage> SaveScratchPadCall(PlanxShelfInfoList scratchpad)
        //{
        //    _logger.LogDebug("Save scratchpad call start ");


        //    string domain = _configuration["AppSettings:ApiUrl"];
        //    string brand = _configuration["AppSettings:ClientBrandId"];
        //    string readScope = _configuration["AzureB2C:ReadScope"];
        //    string writeScope = _configuration["AzureB2CWriteScope"];

        //    var accessToken = await AuthHelper.GetAccessToken(new string[] { readScope });


        //    var json = JsonConvert.SerializeObject(scratchpad);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var uri = "api/v2/planx/save-planogram-scratchpad/";
        //    var url = string.Format("{0}{1}", domain, uri);

        //    _logger.LogDebug("Save scratchpad make call ");

        //    using (SecureHttpClient<PlanxShelfInfoList> httpClient = new SecureHttpClient<PlanxShelfInfoList>(domain, uri, _configuration))
        //    {

        //        try
        //        {
        //            await httpClient.PutRequest(url, accessToken, planoSvg);
        //            //_logger.LogDebug("response from save = " + response + " :: " + StatusText);

        //            return new HttpResponseMessage(HttpStatusCode.OK);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw (ex);
        //        }
        //    }

        //}

        #endregion
    }
}