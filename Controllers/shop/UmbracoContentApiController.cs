using dplo_shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Net.NetworkInformation;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace CoreSystem2024.Controllers.shop
{

    public class UmbracoContentApiController : BaseApiController
    {
        private readonly IPublishedContentQuery _publishedContentQuery;
        private readonly IConfiguration _config;

        public UmbracoContentApiController(
            IPublishedContentQuery publishedContentQuery, IConfiguration config) : base(config)
        {
            _publishedContentQuery = publishedContentQuery;
            _config = config;
        }

        [System.Web.Http.HttpGet]
        [Route("umbraco/api/UmbracoContentApi/GetInformation")]

        public async Task<IActionResult> GetInformation()
        {
            var pageId = new Guid(_config["AppSettings:informationPageUmbracoNodeId"]);
            return await GetUmbracoContent(pageId, "content");
        }


        private async Task<IActionResult> GetUmbracoContent(Guid pageId, string propertyAlias)
        {
            //var content = Services.ContentService.GetById(pageId).GetValue<string>(propertyAlias);
           CMSModelBuilderModels.ContentPage content = (CMSModelBuilderModels.ContentPage)_publishedContentQuery.Content(pageId);
            var bodyText = content.BodyText; //content.Properties.Where(p => p.Alias == "BodyText");
            var richText = bodyText[0].Content;
            //var rawHtml = richText.GetProperty("richText");
            var rawHtml = ((CoreSystem2024.CMSModelBuilderModels.UmbBlockGridDemoRichTextBlock)bodyText[0].Content).RichText;
            var model = new ApiResponseModel
            {
                data = rawHtml.ToString()
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