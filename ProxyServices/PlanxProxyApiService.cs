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
using CoreSystem2024.HttpClientWrapper;
using PMApplication.Dtos;
using Umbraco.Cms.Core;
using PMApplication.Dtos.PlanModels;
using PMApplication.Entities.PlanogramAggregate;
using Microsoft.AspNetCore.Identity;

namespace CoreSystem2024.ProxyServices
{

    public interface IPlanxProxyApiService
    {
        Task<IEnumerable<PlanmMenuPart>> GetMenuCall(int planogramId);
        Task<IEnumerable<PlanmMenuPart>> GetCategoryMenuCall(int planogramId, int categoryId);
        Task<MenuDto> GetMenuCategoriesCall(int planogramId);
        Task<PlanogramDto> GetPlanogramCall(int planogramId);
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

        //protected UserViewModel _userInfo => AuthHelper.GetUserInfo(User);
        private readonly ILogger<YourPlanogramApiController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemberManager _memberManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public PlanxProxyApiService(ILogger<YourPlanogramApiController> logger, IConfiguration configuration, IMemberManager memberManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _configuration = configuration;
            _memberManager = memberManager;
            _signInManager = signInManager;
        }
        #endregion



        #region remote api calls

        public async Task<IEnumerable<PlanmMenuPart>> GetMenuCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(accessToken);
            //_logger.LogError("access token " + " ---- " + jwtSecurityToken.ToString());
            var uri = "api/v2/planx/get-menu/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanmMenuPart>> httpClient = new SecureHttpClient<IEnumerable<PlanmMenuPart>>(domain, uri, _configuration))
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

        Task<IEnumerable<PlanmMenuPart>> IPlanxProxyApiService.GetCategoryMenuCall(int planogramId, int categoryId)
        {
            throw new NotImplementedException();
        }

        Task<MenuDto> IPlanxProxyApiService.GetMenuCategoriesCall(int planogramId)
        {
            throw new NotImplementedException();
        }

        Task<PlanogramDto> IPlanxProxyApiService.GetPlanogramCall(int planogramId)
        {
            throw new NotImplementedException();
        }

        Task<PlanmStandDto> IPlanxProxyApiService.GetStandCall(int standId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<PlanmMenuPart>> IPlanxProxyApiService.GetMenuCall(int planogramId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PlanmMenuPart>> GetCategoryMenuCall(int planogramId, int categoryId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uri = "api/v2/planx/get-category-menu/" + planogramId + "/" + categoryId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (SecureHttpClient<IEnumerable<PlanmMenuPart>> httpClient = new SecureHttpClient<IEnumerable<PlanmMenuPart>>(domain, uri, _configuration))
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

        public async Task<MenuDto> GetMenuCategoriesCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;



            var uri = "api/v2/planx/get-menu-categories/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here


            using (SecureHttpClient<MenuDto> httpClient = new SecureHttpClient<MenuDto>(domain, uri, _configuration))
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

        public async Task<PlanmPlanogramDto> GetPlanogramCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-planogram/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<PlanmPlanogramDto> httpClient = new SecureHttpClient<PlanmPlanogramDto>(domain, uri, _configuration))
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


        public async Task<PlanmStandDto> GetStandCall(int standId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uri = "api/v2/planx/get-stand/" + standId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<PlanmStandDto> httpClient = new SecureHttpClient<PlanmStandDto>(domain, uri, _configuration))
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
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

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

        Task<IEnumerable<PlanmPartInfo>> IPlanxProxyApiService.GetPlanogramShelvesCall(int planogramId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<PlanmPartInfo>> IPlanxProxyApiService.GetPlanogramPartsCall(int planogramId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<PlanmPartInfo>> IPlanxProxyApiService.GetNewPlanogramPartsCall(int planogramId)
        {
            throw new NotImplementedException();
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
        public async Task<IEnumerable<PlanmPartInfo>> GetPlanogramShelvesCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-planogram-shelves/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<IEnumerable<PlanmPartInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanmPartInfo>>(domain, uri, _configuration))
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

        public async Task<IEnumerable<PlanmPartInfo>> GetPlanogramPartsCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-planogram-parts/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<IEnumerable<PlanmPartInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanmPartInfo>>(domain, uri, _configuration))
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

        public async Task<IEnumerable<PlanmPartInfo>> GetNewPlanogramPartsCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;

            var uri = "api/v2/planx/get-new-parts/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<IEnumerable<PlanmPartInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanmPartInfo>>(domain, uri, _configuration))
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
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

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

        Task<IEnumerable<PlanmPartInfo>> IPlanxProxyApiService.GetScratchPadCall(int planogramId)
        {
            throw new NotImplementedException();
        }

        Task<PartDto> IPlanxProxyApiService.GetPartCall(int partId)
        {
            throw new NotImplementedException();
        }

        Task<PartProductsDto> IPlanxProxyApiService.GetPartProductsCall(int partId, int planogramId)
        {
            throw new NotImplementedException();
        }

        Task<ProductShadesDto> IPlanxProxyApiService.GetProductShadesCall(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PlanmPartInfo>> GetScratchPadCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-planogram-scratchpad/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<IEnumerable<PlanmPartInfo>> httpClient = new SecureHttpClient<IEnumerable<PlanmPartInfo>>(domain, uri, _configuration))
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

        public async Task<PartDto> GetPartCall(int partId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-part/" + partId;
            var url = string.Format("{0}{1}", domain, uri);
            using (SecureHttpClient<PartDto> httpClient = new SecureHttpClient<PartDto>(domain, uri, _configuration))
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

        public async Task<PartProductsDto> GetPartProductsCall(int partId, int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-part-products/" + partId + "/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<PartProductsDto> httpClient = new SecureHttpClient<PartProductsDto>(domain, uri, _configuration))
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


        public async Task<ProductShadesDto> GetProductShadesCall(int productId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var uri = "api/v2/planx/get-product-shades/" + productId;
            var url = string.Format("{0}{1}", domain, uri);

            using (SecureHttpClient<ProductShadesDto> httpClient = new SecureHttpClient<ProductShadesDto>(domain, uri, _configuration))
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

        public async Task<String> GetPlanoLockCall(int planogramId)
        {

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];


            try
            {
                //we need to re-auth using the reauth process
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                if (memberIdentity != null)
                {
                    var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(memberIdentity);
                    var userInfo = AuthHelper.GetUserInfo(claimsPrincipal);

                    var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
                    var uri = "api/v2/planx/get-plano-lock/" + planogramId + "/" + userInfo.Id + "/" +
                              userInfo.DisplayName;
                    var url = string.Format("{0}{1}", domain, uri);
                    //maybe log something here

                    using (SecureHttpClient<string> httpClient =
                           new SecureHttpClient<string>(domain, uri, _configuration))
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

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];


            //we need to re-auth using the reauth process
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

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
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

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



        public async Task<HttpResponseMessage> SavePlanogramCallV2(PlanmPlanogramInfo planogramData)
        {
            _logger.LogDebug("Save Cassettes call start ");



            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

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



        public async Task<HttpResponseMessage> SaveCassettesCall(PlanmShelfInfoList shelves)
        {
            _logger.LogDebug("Save Cassettes call start ");



            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var json = JsonConvert.SerializeObject(shelves);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/save-planogram-cassettes/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save Cassettes make call ");


            using (SecureHttpClient<PlanmShelfInfoList> httpClient = new SecureHttpClient<PlanmShelfInfoList>(domain, uri, _configuration))
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


        public async Task<HttpResponseMessage> SavePlanogramJpegCall(PlanmPlanoImageDto planoJpeg)
        {
            _logger.LogDebug("Save planogram jpg call start ");


            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

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
        public async Task<HttpResponseMessage> SavePlanogramSvgCall(PlanmPlanoImageDto planoSvg)
        {
            _logger.LogDebug("Save planogram svg call start ");

            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;


            var json = JsonConvert.SerializeObject(planoSvg);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/save-planogram-svg-image/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save planogram svg make call ");


            using (SecureHttpClient<PlanmPlanoImageDto> httpClient = new SecureHttpClient<PlanmPlanoImageDto>(domain, uri, _configuration))
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

        public async Task<HttpResponseMessage> GetPlanoPDFCall(PlanmPlanoImageDto planoSvg)
        {
            _logger.LogDebug("Get planogram pdf call start ");


            string domain = _configuration["AppSettings:ApiUrl"];
            string brand = _configuration["AppSettings:ClientBrandId"];
            string readScope = _configuration["AzureB2C:ReadScope"];
            string writeScope = _configuration["AzureB2CWriteScope"];

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //if (memberIdentity != null)
            //{
            //    var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(memberIdentity);
            //    var userInfo = AuthHelper.GetUserInfo(claimsPrincipal);
            //}


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