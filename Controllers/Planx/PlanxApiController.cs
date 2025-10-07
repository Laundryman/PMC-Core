using CoreSystem2024.Controllers.shop;
using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using CoreSystem2024.ProxyServices;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Security;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using System.Net.Http;
using dplo_shop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Graph.Models;
using PMApplication.Dtos.PlanModels;
using PMApplication.Entities.PlanogramAggregate;
using PMApplication.Interfaces.ServiceInterfaces;
using PMApplication.Specifications.Filters;

namespace CoreSystem2024.Controllers.Planx
{
    public class PlanxApiController : BaseApiController
    {

        #region Services, managers

        private readonly ILogger<PlanxApiController> _logger;
        private readonly IPlanogramService _planogramService;
        private readonly ICountryService _countryService;
        private readonly ICategoryService _categoryService;
        private readonly IPlanxProxyApiService proxyApi;
        private readonly IMemberManager _memberManager;
        private readonly IOrderService _orderService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;

        #endregion



        #region LocalApiCalls

        public PlanxApiController(ICategoryService categoryService, ICountryService countryService, IPlanogramService planogramService, IOrderService orderService, IStandService standService, ILogger<PlanxApiController> logger, IMemberManager memberManager, IPlanxProxyApiService proxyApi, IConfiguration config, SignInManager<IdentityUser> signInManager) : base(config)
        {
            _logger = logger;
            _memberManager = memberManager;
            this.proxyApi = proxyApi;
            _config = config;
            _signInManager = signInManager;
            _planogramService = planogramService;
            _orderService = orderService;
            _countryService = countryService;
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("/api/planxapi/GetMenuGetImageLocation")]
        public async Task<IActionResult> GetImageLocation()
        {
            var imageLoc = ConfigurationManager.AppSettings["apiUrl"];
            return Ok(imageLoc);
        }


        [HttpPost]
        [Route("/api/planxapi/GetMenu")]
        public async Task<IActionResult> GetMenu(GetMenuParams data)
        {

            try
            {

                var response = await proxyApi.GetMenuCall(data.planogramId);
                //Get the json data from the result
                return Ok(response);
            }
            catch (Exception ex)
            {
                //Something has gone wrong, handle it here
                _logger.LogError("Error getting menu " + " --- " + ex.Message + " stack trace -- " + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/api/planxapi/GetCategoryMenu")]
        public async Task<IActionResult> GetCategoryMenu(GetMenuParams data)
        {
            var pcatFilter = new CategoryFilter
            {
                ParentCatId = 0
            };
            var parentCategories = await _categoryService.GetCategories(pcatFilter);

            var parentCat = parentCategories.Where(c => c.Name == data.category).FirstOrDefault();

            var response = await proxyApi.GetCategoryMenuCall(data.planogramId, parentCat.Id);
            //Get the json data from the result

            return Ok(response);
        }


        [HttpPost]
        [Route("/api/planxapi/GetMenuCategories")]
        public async Task<IActionResult> GetMenuCategories(GetMenuParams data)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetMenuCategoriesCall(data.planogramId);

            return Ok(response);
        }


        [HttpGet]
        [Route("/api/planxapi/GetPlanogram")]
        public async Task<IActionResult> GetPlanogram(int planogramId)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);

            var response = await proxyApi.GetPlanogramCall(planogramId);
            return Ok(response);
        }

        [HttpPost]
        [Route("/api/planxapi/GetScratchPad")]
        public async Task<IActionResult> GetScratchPad(GetMenuParams data)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetScratchPadCall(data.planogramId);
            //Something has gone wrong, handle it here
            return Ok(response);
        }

        [HttpGet]
        [Route("/api/planxapi/GetStand")]
        public async Task<IActionResult> GetStand(int standId)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetStandCall(standId);
            //Get the json data from the result
            return Ok(response);
        }

        [HttpGet]
        [Route("/api/planxapi/GetPlanogramPreview")]
        public async Task<IActionResult> GetPlanogramPreview(int planogramId)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPlanogramPreviewCall(planogramId);
            //Get the json data from the result
            //var svg = new PlanXStandViewModel();
            return Ok(response);
        }

        //[HttpGet]
        //[Route("/api/planxapi/GetLatestVersion")]
        //public async Task<IActionResult> GetLatestVersion(int planogramId)
        //{
        //    //we need to re-auth using the reauth process
        //    //var accessToken = //AuthHelper.ReAuth(Authorization, client);


        //    var response = await proxyApi.GetLatestVersionCall(planogramId);
        //    return Ok(response);
        //}



        [HttpGet]
        [Route("/api/planxapi/GetPlanogramShelves")]
        public async Task<IActionResult> GetPlanogramShelves(int planogramId)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPlanogramShelvesCall(planogramId);

            return Ok(response);
            //return Ok(shelves);
        }


        [HttpGet]
        [Route("/api/planxapi/GetPlanogramParts")]
        public async Task<IActionResult> GetPlanogramParts(int planogramId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //if (memberIdentity != null)
            //{
            //    var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(memberIdentity);
            //    var userInfo = AuthHelper.GetUserInfo(claimsPrincipal);
            //    var countryId = _countryService.GetCountry(userInfo.DiamCountryId);
            //}


            var response = await proxyApi.GetPlanogramPartsCall(planogramId);

            return Ok(response);
            //return Ok(parts);
        }

        [HttpGet]
        [Route("/api/planxapi/GetNewPlanogramParts")]
        public async Task<IActionResult> GetNewPlanogramParts(int planogramId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            //var userInfo = AuthHelper.GetUserInfo(memberIdentity);
            //var countryId = _countryService.GetCountry(userInfo.DiamCountryId);


            var response = await proxyApi.GetNewPlanogramPartsCall(planogramId);

            return Ok(response);
            //return Ok(parts);
        }

        [HttpGet]
        [Route("/api/planxapi/GetNonMarketParts")]
        public async Task<IActionResult> GetNonMarketParts(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetNonMarketPartsCall(planogramId);
            return Ok(response);
        }

        [HttpGet]
        [Route("/api/planxapi/GetPart")]
        public async Task<IActionResult> GetPart(int partId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPartCall(partId);
            return Ok(response);
        }


        [HttpGet]
        [Route("/api/planxapi/GetPartProducts")]
        public async Task<IActionResult> GetPartProducts(int partId, int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPartProductsCall(partId, planogramId);
            return Ok(response);
        }


        [HttpGet]
        [Route("/api/planxapi/GetProductShades")]
        public async Task<IActionResult> GetProductShades(int productId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetProductShadesCall(productId);
            return Ok(response);
        }

        [HttpPost]
        [Route("/api/planxapi/SavePlanogramV2")]
        public async Task<HttpResponseMessage> SavePlanogramV2(PlanmPlanogramInfo planogramData)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            if (memberIdentity != null)
            {
                var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(memberIdentity);
                var userProfile = AuthHelper.GetUserInfo(claimsPrincipal);
                var userInfo = AuthHelper.GetUserInfo(claimsPrincipal);
                planogramData.UserId = userProfile.Id;
                planogramData.UserName = userProfile.DisplayName;
                planogramData.CountryId = userProfile.DiamCountryId;
                planogramData.UserRoles = userProfile.Roles;
            }

            _logger.LogDebug("Save Planogram start ");

            var response = await proxyApi.SavePlanogramCallV2(planogramData);

            _logger.LogDebug("Save Planogram end ");

            //Get the json data from the result
            //var productShades = new ProductShadesViewModel();
            //var response =
            if (response is OkResult)
            {
                return response;
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
        }

        [HttpPost]
        [Route("/api/planxapi/SavePlanogramJpeg")]
        public async Task<IActionResult> SavePlanogramJpeg(PlanmPlanoImageDto planoJpeg)
        {
            _logger.LogDebug("Save planogram jpg");

            try
            {
                var response = await proxyApi.SavePlanogramJpegCall(planoJpeg);
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    throw new Exception(response.Content.ToString());
                }
            }
            catch (Exception Ex)
            {
                // Get stack trace for the exception with source file information
                string message;
                if (Ex.InnerException != null)
                {
                    message = Ex.Message + Ex.InnerException;
                }
                else
                {
                    message = Ex.Message + Ex.StackTrace;
                }
                //log an error
                _logger.LogError("error saving jpg ---- " + message);

                return BadRequest(message);
            }
        }
        [HttpPost]
        [Route("/api/planxapi/SavePlanogramSvg")]
        public async Task<HttpResponseMessage> SavePlanogramSvg(PlanmPlanoImageDto planoSvg)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            _logger.LogDebug("Save Planogram Image ");

            var response = await proxyApi.SavePlanogramSvgCall(planoSvg);

            _logger.LogDebug("Save scratchpad end ");

            //Get the json data from the result
            //var productShades = new ProductShadesViewModel();
            //var response =
            if (response is OkResult)
            {
                return response;
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }

        }


        [HttpPost]
        [Route("/api/planxapi/GetPlanoPDF")]
        public async Task<IActionResult> GetPlanoPDF(PlanmPlanoImageDto planoSvg)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            _logger.LogDebug("Save Planogram Image ");
            var planogram = await _planogramService.GetPlanogram(planoSvg.PlanogramId);
            var response = await proxyApi.GetPlanoPDFCall(planoSvg);

            _logger.LogDebug("Save scratchpad end ");

            //Get the json data from the result
            if (response.IsSuccessStatusCode)
            {
                HttpResponseMessage finalResponse = new HttpResponseMessage(HttpStatusCode.OK);
                //read the pdf response stream
                //var pdfByteStream = await response.Content.ReadAsStringAsync();
                //adding bytes to memory stream
                //var dataStream = new MemoryStream(pdfByteStream);
                //finalResponse.Content = new StreamContent(dataStream);
                //finalResponse.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                //finalResponse.Content.Headers.ContentDisposition.FileName = planogram.Name + ".pdf";
                //finalResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

                //var pdfString = new pdfByteStream.;
                //finalResponse.Content = response.Content;
                //finalResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                //read the pdf response stream
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = planogram.Name + ".pdf",
                    Inline = false  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                
                Response.Headers.ContentType = "application/pdf";

                var pdfByteStream = response.Content.ReadAsByteArrayAsync().Result;
                var dataStream = new MemoryStream(pdfByteStream);
                var pdfString = new StringContent(Convert.ToBase64String(pdfByteStream));

                var fileResponse = new PdfResponseModel()
                {
                    pdfBase64String = pdfString,
                    error = null
                };

                return File(dataStream, "application/pdf");
                //return Ok(fileResponse);
                //finalResponse.Content = pdfString;
                //return finalResponse;

            }
            else
            {
                //Something has gone wrong, handle it here
                return BadRequest(response.Content.ToString());
            }

        }

        //[HttpPost]
        //[Route("/api/planxapi/SaveScratchPad")]
        //public async Task<IActionResult> SaveScratchPad(PlanxShelfInfoList scratchpad)
        //{
        //    //we need to re-auth using the reauth process
        //    //var accessToken = //AuthHelper.ReAuth(Authorization, client);


        //    _logger.LogDebug("Save scratchpad start ");

        //    var response = await proxyApi.SaveScratchPadCall(scratchpad);

        //    _logger.LogDebug("Save scratchpad end ");

        //    //Get the json data from the result
        //    //var productShades = new ProductShadesViewModel();
        //    //var response =
        //    if (response is OkResult)
        //    {
        //        return response;
        //    }
        //    else
        //    {
        //        //Something has gone wrong, handle it here
        //        return response;
        //    }
        //}

        [HttpPost]
        [Route("/api/planxapi/getPlanoLock")]
        public async Task<IActionResult> GetPlanoLock(GetMenuParams data)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPlanoLockCall(data.planogramId);
            //var response =
            if (response == "locked")
            {
                //return error
                return BadRequest(response);
            }
            else
            {
                return Ok(response);

            }
        }

        [HttpPost]
        [Route("/api/planxapi/GetPlanoComCount")]
        public async Task<IActionResult> GetPlanoComCount(GetMenuParams data)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPlanoComCountCall(data.planogramId);
            //var response =
                return Ok(response);
        }
        [HttpGet]
        [Route("/api/planxapi/Unlock")]
        public async Task<IActionResult> Unlock(int planogramId = 0)
        {
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();

                //var userProfile = AuthHelper.GetUserInfo(memberIdentity);

                var isLocked = _planogramService.IsLocked(planogramId, null);
                if (isLocked)
                {
                    _planogramService.UnLockPlanogram(planogramId);
                }

                return Ok();
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex);
            }
        }


        ////[Route("api/v2/order/export/{orderId}")]
        //[HttpGet]
        //[Route("/api/planxapi/ExportSku")]
        //public async Task<IActionResult> ExportSku(int planogramId = 0)
        //{
        //    // we can retrieve the userId from the request
        //    var currentUser = User;

        //    try
        //    {
        //        Planogram planogram = await _planogramService.GetPlanogram(planogramId);
        //        string fileName = string.Format("planogram_{0}_{1}.xls", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
        //            planogram.Name);
        //        string filePath = string.Format("{0}files\\ExportImport\\{1}",
        //            HttpContext.Request.PathBase, fileName);

        //        var currentUri = new Uri(Request.GetDisplayUrl());
        //        string domainURI = currentUri.Scheme + "://" + currentUri.Authority;

        //        string wPath = string.Format("{0}/files/ExportImport/", domainURI);
        //        string webPath = string.Format("{0}{1}", wPath, Uri.EscapeDataString(fileName));


        //        var exportManager = new ExportManager(_planogramService, _orderService);
        //        exportManager.ExportSkuListToXls(filePath, planogram);

        //        //ExportManager.WriteResponseXls(filePath, fileName);

        //        return Ok(new FileDesc(fileName, webPath, 0));
        //    }
        //    catch (Exception Ex)
        //    {

        //        // Get stack trace for the exception with source file information
        //        //var st = new StackTrace(ex, true);
        //        // Get the top stack frame
        //        //var frame = st.GetFrame(0);
        //        // Get the line number from the stack frame
        //        //var line = frame.GetFileLineNumber();
        //        string message;
        //        if (Ex.InnerException != null)
        //        {
        //            message = (Ex.Message + Ex.InnerException);
        //        }
        //        else
        //        {
        //            message = (Ex.Message + Ex.StackTrace);
        //        }
        //        //log an error

        //        return BadRequest(message);
        //    }

        //}


        #endregion


    }
}