using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using CoreSystem.Controllers.shop;
using CoreSystem.Helpers;
using CoreSystem.Models;
using dplo.Data;
using dplo.Domain;
using dplo.Domain.Entities;
using dplo.Helpers;
using Dplo.ViewModels;
using Dplo.ViewModels.PlanxModels;
using dplo.Service;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using dplo.Service.MSGraphUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoreSystem.Controllers;

namespace CoreSystemII.Controllers.Proxy
{
    public class PlanxProxyController : Controller
    {

        #region Services, managers
        protected UserViewModel _userInfo => AuthHelper.GetUserInfo(User);
        private ILogger<YourPlanogramApiController> _logger;


        public PlanxProxyController(ILogger<YourPlanogramApiController> logger)
        {
            _logger = logger;
        }
        #endregion



        #region remote api calls

        public async Task<IActionResult> GetMenuCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(accessToken);
            //_logger.LogError("access token " + " ---- " + jwtSecurityToken.ToString());
            var uri = "api/v2/planx/get-menu/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetCategoryMenuCall(int planogramId, int categoryId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uri = "api/v2/planx/get-category-menu/" + planogramId + "/" + categoryId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetMenuCategoriesCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });



            var uri = "api/v2/planx/get-menu-categories/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetPlanogramCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planx/get-planogram/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }


        public async Task<IActionResult> GetStandCall(int standId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uri = "api/v2/planx/get-stand/" + standId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }
        public async Task<IActionResult> GetPlanogramPreviewCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planx/get-planogram-preview/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }
        public async Task<IActionResult> GetLatestVersionCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uri = "api/v2/planx/get-latest-version/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }
        public async Task<IActionResult> GetPlanogramShelvesCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planx/get-planogram-shelves/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetPlanogramPartsCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planx/get-planogram-parts/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetNewPlanogramPartsCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

            var uri = "api/v2/planx/get-new-parts/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetNonMarketPartsCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planx/get-nonmarket-parts/" + planogramId + "/" + UserInfo.DiamCountryId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetScratchPadCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planx/get-planogram-scratchpad/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetPartCall(int partId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planx/get-part/" + partId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetPartProductsCall(int partId, int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planx/get-part-products/" + partId + "/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }


        public async Task<IActionResult> GetProductShadesCall(int productId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planx/get-product-shades/" + productId;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }

        public async Task<IActionResult> GetPlanoLockCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            try
            {
                //we need to re-auth using the reauth process
                var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope, Globals.WriteTasksScope });


                var uri = "api/v2/planx/get-plano-lock/" + planogramId + "/" + _userInfo.Id + "/" + _userInfo.DisplayName;
                var url = string.Format("{0}{1}", domain, uri);
                //maybe log something here

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        var response = await httpClient.SendAsync(request);
                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPlanoLockCall error: " + ex.Message);
                throw ex;
            }


        }

        public async Task<IActionResult> GetPlanoComCountCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planogram/getCommentCount/" + planogramId + "/" + brand;
            var url = string.Format("{0}{1}", domain, uri);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }
        public async Task<IActionResult> UnlockCall(int planogramId)
        {
            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var uri = "api/v2/planogram/unlock/" + planogramId;
            var url = string.Format("{0}{1}", domain, uri);
            //maybe log something here

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }


        }


        public async Task<IActionResult> SavePlanogramCallV2(PlanxPlanogramInfo planogramData)
        {
            _logger.LogDebug("Save Cassettes call start ");


            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var json = JsonConvert.SerializeObject(planogramData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/save-planogramV2/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save planogram make call ");

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
                    _logger.LogDebug("response from save = " + responseBodyAsText + " :: " + StatusText);

                    return Ok(response);
                }
            }


        }



        public async Task<IActionResult> SaveCassettesCall(PlanxShelfInfoList shelves)
        {
            _logger.LogDebug("Save Cassettes call start ");


            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var json = JsonConvert.SerializeObject(shelves);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/save-planogram-cassettes/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save Cassettes make call ");

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
                    _logger.LogDebug("response from save = " + responseBodyAsText + " :: " + StatusText);

                    return Ok(response);

                }
            }


        }


        public async Task<IActionResult> SavePlanogramJpegCall(PlanogramImageViewModel planoJpeg)
        {
            _logger.LogDebug("Save planogram jpg call start ");

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });

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
                    _logger.LogDebug("response from save = " + responseBodyAsText + " :: " + StatusText);

                    return Ok(response);

                }
            }
        }
        public async Task<IActionResult> SavePlanogramSvgCall(PlanogramImageViewModel planoSvg)
        {
            _logger.LogDebug("Save planogram svg call start ");

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var json = JsonConvert.SerializeObject(planoSvg);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/save-planogram-svg-image/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save planogram svg make call ");

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
                    _logger.LogDebug("response from save = " + responseBodyAsText + " :: " + StatusText);

                    return Ok(response);

                }
            }

        }

        public async Task<IActionResult> GetPlanoPDFCall(PlanogramImageViewModel planoSvg)
        {
            _logger.LogDebug("Get planogram pdf call start ");

            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var json = JsonConvert.SerializeObject(planoSvg);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/get-planogram-pdf/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Get Planogram PDF make call ");

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = content;
                using (HttpClient httpClient = new HttpClient())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);
                    return Ok(response);
                }
            }

        }

        public async Task<IActionResult> SaveScratchPadCall(PlanxShelfInfoList scratchpad)
        {
            _logger.LogDebug("Save scratchpad call start ");


            string domain = ConfigurationManager.AppSettings["apiUrl"];
            string brand = ConfigurationManager.AppSettings["brand"];

            //we need to re-auth using the reauth process
            var accessToken = await AuthHelper.GetAccessToken(new string[] { Globals.ReadTasksScope });


            var json = JsonConvert.SerializeObject(scratchpad);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var uri = "api/v2/planx/save-planogram-scratchpad/";
            var url = string.Format("{0}{1}", domain, uri);

            _logger.LogDebug("Save scratchpad make call ");

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
                    _logger.LogDebug("response from save = " + responseBodyAsText + " :: " + StatusText);

                    return Ok(response);

                }
            }


        }

        #endregion
    }
}