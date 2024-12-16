using dplo.Service;
using dplo_shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace CoreSystem2024.Controllers.shop
{
    public class UmbracoContentApiController : BaseApiController
    {
        private readonly IPublishedContentQuery _publishedContentQuery;
        private readonly IRelationService _relationService;
        public UmbracoContentApiController(ICategoryService categoryService,
            ICatalogueService catalogueService,
            ICountryService countryService,
            IPlanogramService planogramService,
            IOrderService orderService,
            IStandService standService,
            IRelationService relationService,
            IPublishedContentQuery publishedContentQuery) : base(categoryService, catalogueService, countryService, planogramService, orderService, standService)
        {
            _relationService = relationService;
            _publishedContentQuery = publishedContentQuery;
        }

        [System.Web.Http.HttpGet]
        [Route("/api/UmbracoContentApi/GetInformation")]

        public async Task<IActionResult> GetInformation()
        {
            var pageId = int.Parse(ConfigurationManager.AppSettings["informationPageUmbracoNodeId"]);
            return await GetUmbracoContent(pageId, "pageContent");
        }


        private async Task<IActionResult> GetUmbracoContent(int pageId, string propertyAlias)
        {
            //var content = Services.ContentService.GetById(pageId).GetValue<string>(propertyAlias);
            var content = _publishedContentQuery.Content(pageId).Value<string>(propertyAlias);
            var model = new ApiResponseModel
            {
                data = (content)
            };

            return Ok(model);
        }



        //[[Route("api/content/{pageId}/{propertyAlias}")]
        //[HttpGet]
        //public  JsonResult<ApiResponseModel> GetContent(int pageId, string propertyAlias)
        //{
        //    return GetUmbracoContent(pageId, "pageContent");
        //}


        ////[[Route("api/content/{pageId}/{propertyAlias}")]
        //[HttpGet]
        //[Obsolete]
        //public async Task<JsonResult<ApiResponseModel>> GetRemoteContent(int pageId, string propertyAlias)
        //{
        //    var uri = "/umbraco/api/contentapi/getcontent/?pageId=" + pageId + "&propertyAlias=" + propertyAlias;
        //    return await RequestContent(uri);
        //}

        //[Obsolete]
        //private async Task<JsonResult<ApiResponseModel>> RequestContent(string uri)
        //{
        //    //var currentAuth = AuthHelper.GetAuth(HttpContext.Current.Request);
        //    //var accessToken = currentAuth.AccessToken;

        //    string domain = "";// ConfigurationManager.AppSettings["clientUrl"];

        //    uri = domain + uri 
        //        + "&token=" + AuthHelper.EscapeUriDataStringRfc3986(accessToken);

        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        var response = await httpClient.GetAsync(uri);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            throw new HttpResponseException(new HttpResponseMessage(response.StatusCode)
        //            {
        //                Content = response.Content,
        //                ReasonPhrase = response.ReasonPhrase
        //            });
        //        }

        //        var content = await response.Content.ReadAsStringAsync();

        //        var model = new ApiResponseModel
        //        {
        //            data = JsonConvert.DeserializeObject<string>(content)
        //        };

        //        return JsonContent(model);
        //    }
        //}


    }
}