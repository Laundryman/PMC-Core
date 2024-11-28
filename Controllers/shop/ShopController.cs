using dplo.Service.MSGraphUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace CoreSystem.Controllers.shop
{
    public class ShopController : RenderController
    {

        
        // GET: Shop
        public ShopController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var proxySupport = new ProxyApiSupport();
                //// Retrieve the token with the specified scopes
                var result = await proxySupport.AcquireTokenForScopes(new string[]
                {
                    Globals.ReadTasksScope, Globals.WriteTasksScope
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