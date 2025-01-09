using dplo.Service.MSGraphUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace CoreSystem2024.Controllers.shop
{
    public class ShopController : RenderController
    {

        private readonly IConfiguration Configuration;
        private string domain;
        private string brandId;
        private string readScope;
        private string writeScope;

        // GET: Shop
        public ShopController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IConfiguration configuration) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            Configuration = configuration;
            domain = Configuration["AppSettings:ApiUrl"];
            brandId = Configuration["AppSettings:ClientBrandId"];
            readScope = Configuration["AzureB2C:ReadScope"];
            writeScope = Configuration["AzureB2CWriteScope"];
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var proxySupport = new ProxyApiSupport();
                //// Retrieve the token with the specified scopes
                var result = await proxySupport.AcquireTokenForScopes(new string[]
                {
                    readScope, writeScope
                });
            }
            catch (MsalUiRequiredException)
            {
                //var proxySupport = new ProxyApiSupport();
                //var result = await proxySupport.AcquireTokenInteractive(new string[]
                //    { Globals.ReadTasksScope, Globals.WriteTasksScope });
                return new RedirectResult("/Welcome");
            }
            return CurrentTemplate(CurrentPage);
        }
    }
}