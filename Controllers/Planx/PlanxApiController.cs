using CoreSystem2024.Controllers.shop;
using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using CoreSystemII.Controllers.Proxy;
using dplo.Domain;
using dplo.Domain.Entities;
using dplo.Helpers;
using dplo.Service;
using Dplo.ViewModels;
using Dplo.ViewModels.PlanxModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Configuration;
using Umbraco.Cms.Core.Security;

namespace CoreSystem2024.Controllers.Planx
{
    public class PlanxApiController : BaseApiController
    {

        #region Services, managers

        private ILogger<YourPlanogramApiController> _logger;
        public IPlanogramService _planogramService;
        public ICountryService _countryService;
        public ICategoryService _categoryService;
        private PlanxProxyController proxyApi;
        private readonly IMemberManager _memberManager;

        #endregion



        #region LocalApiCalls

        public PlanxApiController(ICategoryService categoryService, ICatalogueService catalogueService, ICountryService countryService, IPlanogramService planogramService, IOrderService orderService, IStandService standService, ILogger<YourPlanogramApiController> logger, IMemberManager memberManager) : base(categoryService, catalogueService, countryService, planogramService, orderService, standService)
        {
            _logger = logger;
            _memberManager = memberManager;
            _planogramService = planogramService;
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
                var menu = new List<PlanxMenuPart>();
                //var response =
                if (response is OkResult)
                {
                    var menuJson = response as OkObjectResult;
                    menu = JsonConvert.DeserializeObject<List<PlanxMenuPart>>(menuJson.Value.ToString());
                }
                else
                {
                    //Something has gone wrong, handle it here
                    _logger.LogError("Error getting menu " + " --- ");
                    return BadRequest();
                }

                return Ok(menu);
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
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);
            var parentCategories = _categoryService.GetParentCategories();

            var parentCat = parentCategories.Where(c => c.Name == data.category).FirstOrDefault();

            var response = await proxyApi.GetCategoryMenuCall(data.planogramId, parentCat.CategoryId);
            //Get the json data from the result
            var menu = new List<PlanxMenuPart>();
            //var response =
            if (response is OkResult)
            {
                var menuJson = response as OkObjectResult;
                menu = JsonConvert.DeserializeObject<List<PlanxMenuPart>>(menuJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(menu);
        }


        [HttpPost]
        [Route("/api/planxapi/GetMenuCategories")]
        public async Task<IActionResult> GetMenuCategories(GetMenuParams data)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetMenuCategoriesCall(data.planogramId);
            //Get the json data from the result
            var menu = new PlanXMenuViewModel();
            //var response =
            if (response is OkResult)
            {
                var menuJson = response as OkObjectResult;
                menu = JsonConvert.DeserializeObject<PlanXMenuViewModel>(menuJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(menu);
        }


        [HttpGet]
        [Route("/api/planxapi/GetPlanogram")]
        public async Task<IActionResult> GetPlanogram(int planogramId)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);

            var response = await proxyApi.GetPlanogramCall(planogramId);
            //Get the json data from the result
            var planogram = new PlanXPlanogramViewModel();
            //var response =
            if (response is OkResult)
            {
                var standJson = response as OkObjectResult;
                planogram = JsonConvert.DeserializeObject<PlanXPlanogramViewModel>(standJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                _logger.LogWarning("Error getting planogram " + " --- ");

                return response;
            }
            return Ok(planogram);
        }

        [HttpPost]
        [Route("/api/planxapi/GetScratchPad")]
        public async Task<IActionResult> GetScratchPad(GetMenuParams data)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetScratchPadCall(data.planogramId);
            //Get the json data from the result
            var parts = new List<PlanxPartInfo>();
            //var response =
            if (response is OkResult)
            {
                var partsJson = response as OkObjectResult;
                parts = JsonConvert.DeserializeObject<List<PlanxPartInfo>>(partsJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(parts);
        }

        [HttpGet]
        [Route("/api/planxapi/GetStand")]
        public async Task<IActionResult> GetStand(int standId)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetStandCall(standId);
            //Get the json data from the result
            var stand = new PlanXStandViewModel();
            //var response =
            if (response is OkResult)
            {
                var standJson = response as OkObjectResult;
                stand = JsonConvert.DeserializeObject<PlanXStandViewModel>(standJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(stand);
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
            var svgVersion = "";
            if (response is OkResult)
            {
                var versionJson = response as OkObjectResult;
                svgVersion = versionJson.Value.ToString();
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(svgVersion);
        }

        [HttpGet]
        [Route("/api/planxapi/GetLatestVersion")]
        public async Task<IActionResult> GetLatestVersion(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetLatestVersionCall(planogramId);
            //Get the json data from the result
            //var svg = new PlanXStandViewModel();
            var svgVersion = "";
            if (response is OkResult)
            {
                var versionJson = response as OkObjectResult;
                svgVersion = versionJson.Value.ToString();
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(svgVersion);
        }



        [HttpGet]
        [Route("/api/planxapi/GetPlanogramShelves")]
        public async Task<IActionResult> GetPlanogramShelves(int planogramId)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPlanogramShelvesCall(planogramId);
            //Get the json data from the result
            var shelves = new List<PlanxPartInfo>();
            //var response =
            if (response is OkResult)
            {
                var shelvesJson = response as OkObjectResult;
                shelves = JsonConvert.DeserializeObject<List<PlanxPartInfo>>(shelvesJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(shelves);
            //return Ok(shelves);
        }


        [HttpGet]
        [Route("/api/planxapi/GetPlanogramParts")]
        public async Task<IActionResult> GetPlanogramParts(int planogramId)
        {
            //we need to re-auth using the reauth process
            ////var accessToken = //AuthHelper.ReAuth(Authorization, client);
            var countryId = _countryService.GetCountry(UserInfo.DiamCountryId);


            var response = await proxyApi.GetPlanogramPartsCall(planogramId);
            //Get the json data from the result
            var parts = new List<PlanxPartInfo>();
            //var response =
            if (response is OkResult)
            {
                var partsJson = response as OkObjectResult;
                parts = JsonConvert.DeserializeObject<List<PlanxPartInfo>>(partsJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(parts);
            //return Ok(parts);
        }

        [HttpGet]
        [Route("/api/planxapi/GetNewPlanogramParts")]
        public async Task<IActionResult> GetNewPlanogramParts(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);
            var countryId = _countryService.GetCountry(UserInfo.DiamCountryId);


            var response = await proxyApi.GetNewPlanogramPartsCall(planogramId);
            //Get the json data from the result
            var parts = new List<PlanxPartInfo>();
            //var response =
            if (response is OkResult)
            {
                var partsJson = response as OkObjectResult;
                parts = JsonConvert.DeserializeObject<List<PlanxPartInfo>>(partsJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(parts);
            //return Ok(parts);
        }

        [HttpGet]
        [Route("/api/planxapi/GetNonMarketParts")]
        public async Task<IActionResult> GetNonMarketParts(int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetNonMarketPartsCall(planogramId);
            //Get the json data from the result
            var parts = new List<PlanogramPart>();
            //var response =
            if (response is OkResult)
            {
                var partsJson = response as OkObjectResult;
                parts = JsonConvert.DeserializeObject<List<PlanogramPart>>(partsJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(parts);
            //return Ok(parts);
        }

        [HttpGet]
        [Route("/api/planxapi/GetPart")]
        public async Task<IActionResult> GetPart(int partId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPartCall(partId);
            //Get the json data from the result
            var part = new PlanogramPartViewModel();
            //var response =
            if (response is OkResult)
            {
                var partJson = response as OkObjectResult;
                part = JsonConvert.DeserializeObject<PlanogramPartViewModel>(partJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(part);
        }


        [HttpGet]
        [Route("/api/planxapi/GetPartProducts")]
        public async Task<IActionResult> GetPartProducts(int partId, int planogramId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPartProductsCall(partId, planogramId);
            //Get the json data from the result
            var partProducts = new PartProductsViewModel();
            //var response =
            if (response is OkResult)
            {
                var partJson = response as OkObjectResult;
                partProducts = JsonConvert.DeserializeObject<PartProductsViewModel>(partJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(partProducts);
        }


        [HttpGet]
        [Route("/api/planxapi/GetProductShades")]
        public async Task<IActionResult> GetProductShades(int productId)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetProductShadesCall(productId);
            //Get the json data from the result
            var productShades = new ProductShadesViewModel();
            //var response =
            if (response is OkResult)
            {
                var partJson = response as OkObjectResult;
                productShades = JsonConvert.DeserializeObject<ProductShadesViewModel>(partJson.Value.ToString());
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }
            return Ok(productShades);
        }

        [HttpPost]
        [Route("/api/planxapi/SavePlanogramV2")]
        public async Task<IActionResult> SavePlanogramV2(PlanxPlanogramInfo planogramData)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userProfile = AuthHelper.GetUserInfo(memberIdentity);

            planogramData.UserId = userProfile.Id;
            planogramData.UserName = userProfile.DisplayName;
            planogramData.CountryId = userProfile.DiamCountryId;
            planogramData.UserRoles = userProfile.Roles;

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
        public async Task<IActionResult> SavePlanogramJpeg(PlanogramImageViewModel planoJpeg)
        {
            _logger.LogDebug("Save planogram jpg");

            try
            {
                var response = await proxyApi.SavePlanogramJpegCall(planoJpeg);
                return Ok();
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

                return BadRequest(message);
            }
        }
        [HttpPost]
        [Route("/api/planxapi/SavePlanogramSvg")]
        public async Task<IActionResult> SavePlanogramSvg(PlanogramImageViewModel planoSvg)
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
        public async Task<IActionResult> GetPlanoPDF(PlanogramImageViewModel planoSvg)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            _logger.LogDebug("Save Planogram Image ");

            var response = await proxyApi.GetPlanoPDFCall(planoSvg);

            _logger.LogDebug("Save scratchpad end ");

            //Get the json data from the result
            //var productShades = new ProductShadesViewModel();
            //var response =
            if (response is OkResult)
            {
                //IActionResult finalResponse = Ok("pdf");
                //read the pdf response stream
                var pdfStream = response as FileStreamResult;

                //var pdfStream = response.Content.ReadAsByteArrayAsync().Result;
                //var finalResponse = new StringContent(Convert.ToBase64String(pdfStream.Value));

                //finalResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return new FileStreamResult(pdfStream.FileStream, "application/pdf");
            }
            else
            {
                //Something has gone wrong, handle it here
                return response;
            }

        }

        [HttpPost]
        [Route("/api/planxapi/SaveScratchPad")]
        public async Task<IActionResult> SaveScratchPad(PlanxShelfInfoList scratchpad)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            _logger.LogDebug("Save scratchpad start ");

            var response = await proxyApi.SaveScratchPadCall(scratchpad);

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
        [Route("/api/planxapi/GetPlanoLock")]
        public async Task<IActionResult> GetPlanoLock(GetMenuParams data)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);


            var response = await proxyApi.GetPlanoLockCall(data.planogramId);
            //var response =
            if (response is OkResult)
            {
                //Something has gone wrong, handle it here
                return Ok();
            }
            else
            {
                return response;
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
            if (response is OkResult)
            {
                var result = response as OkObjectResult;
                return Ok(result.Value);
            }
            return response;
        }
        [HttpGet]
        [Route("/api/planxapi/Unlock")]
        public async Task<IActionResult> Unlock(int planogramId = 0)
        {
            //we need to re-auth using the reauth process
            //var accessToken = //AuthHelper.ReAuth(Authorization, client);

            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();

                var userProfile = AuthHelper.GetUserInfo(memberIdentity);
                var isLocked = _planogramService.IsLocked(planogramId, userProfile);
                if (isLocked)
                {
                    //lock the planogram Now
                    _planogramService.UnLockPlanogram(planogramId);
                }

                return Ok();
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex);
            }

            //var response = await proxyApi.UnlockCall(planogramId);

            //if (!response is OkResult)
            //{
            //    //Something has gone wrong, handle it here
            //    return response;
            //}
            //return Request.CreateResponse(HttpStatusCode.OK);
        }


        //[Route("api/v2/order/export/{orderId}")]
        [HttpGet]
        [Route("/api/planxapi/ExportSku")]
        public IActionResult ExportSku(int planogramId = 0)
        {
            // we can retrieve the userId from the request
            var currentUser = User;

            try
            {
                Planogram planogram = _planogramService.GetPlanogram(planogramId);
                string fileName = string.Format("planogram_{0}_{1}.xls", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
                    planogram.Name);
                string filePath = string.Format("{0}files\\ExportImport\\{1}",
                    HttpContext.Request.PathBase, fileName);

                var currentUri = new Uri(Request.GetDisplayUrl());
                string domainURI = currentUri.Scheme + "://" + currentUri.Authority;

                string wPath = string.Format("{0}/files/ExportImport/", domainURI);
                string webPath = string.Format("{0}{1}", wPath, Uri.EscapeDataString(fileName));


                var exportManager = new ExportManager(_planogramService, _orderService);
                exportManager.ExportSkuListToXls(filePath, planogram);

                //ExportManager.WriteResponseXls(filePath, fileName);

                return Ok(new FileDesc(fileName, webPath, 0));
            }
            catch (Exception Ex)
            {

                // Get stack trace for the exception with source file information
                //var st = new StackTrace(ex, true);
                // Get the top stack frame
                //var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                //var line = frame.GetFileLineNumber();
                string message;
                if (Ex.InnerException != null)
                {
                    message = (Ex.Message + Ex.InnerException);
                }
                else
                {
                    message = (Ex.Message + Ex.StackTrace);
                }
                //log an error

                return BadRequest(message);
            }

        }


        #endregion


    }
}